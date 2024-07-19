using EasyMySql.Net.Attributes;
using MySqlConnector;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using static System.Math;

namespace EasyMySql.Net
{
    public class DBConnection : IDisposable
    {
        public MySqlConnection Connection;
        public static BlockingCollection<(MySqlCommand, Task)> WaitingCommands = new BlockingCollection<(MySqlCommand, Task)>();
        public bool CanExecuteCommand = true;

        public DBConnection(string conString, bool open = true)
        {
            Connection = new MySqlConnection(conString);
            if(open)
                Open();
        }

        public string ConnectionString
        {
            get => Connection.ConnectionString;
            set => Connection.ConnectionString = value;
        }
        public int ServerThread => Connection.ServerThread;
        public string ServerVersion => Connection.ServerVersion;
        public bool CanCreateBatch => Connection.CanCreateBatch;
        public string DataSource => Connection.DataSource;
        public string DataBase => Connection.Database;
        public ConnectionState State => Connection.State;
        public ISite? Site
        {
            get => Connection.Site;
            set => Connection.Site = value;
        }
        public int ConnectionTimeout => Connection.ConnectionTimeout;
        public IContainer? Container => Connection.Container;

        //Events
        public Func<X509CertificateCollection, ValueTask>? ProvideClientCertificatesCallback
        {
            get => Connection.ProvideClientCertificatesCallback;
            set => Connection.ProvideClientCertificatesCallback = value;
        }
        public Func<MySqlProvidePasswordContext, string>? ProvidePasswordCallback
        {
            get => Connection.ProvidePasswordCallback;
            set => Connection.ProvidePasswordCallback = value;
        }
        public RemoteCertificateValidationCallback? RemoteCertificateValidationCallback
        {
            get => Connection.RemoteCertificateValidationCallback;
            set => Connection.RemoteCertificateValidationCallback = value;
        }

        public bool Ping() => Connection.Ping();
        public async Task<bool> PingAsync(CancellationToken cancellationToken = default) => await Connection.PingAsync(cancellationToken);
        public DataTable GetSchema() => Connection.GetSchema();
        public async Task<DataTable> GetSchemaAsync(CancellationToken cancellationToken = default) => await Connection.GetSchemaAsync(cancellationToken);
        public MySqlTransaction BeginTransaction(IsolationLevel isolationLevel, bool isReadOnly) => Connection.BeginTransaction(isolationLevel, isReadOnly);
        public async Task<MySqlTransaction> BeginTransactionAsync(IsolationLevel isolationLevel, bool isReadOnly, CancellationToken cancellationToken = default) => await Connection.BeginTransactionAsync(isolationLevel, isReadOnly, cancellationToken);
        public MySqlBatch CreateBatch() => Connection.CreateBatch();
        public MySqlCommand CreateCommand() => Connection.CreateCommand();
        public async ValueTask ResetConnectionAsync(CancellationToken cancellationToken = default) => await Connection.ResetConnectionAsync(cancellationToken);

        public void ChangeDatabase(string databaseName) => Connection.ChangeDatabase(databaseName);
        public async Task ChangeDatabaseAsync(string databaseName, CancellationToken cancellationToken = default) => 
            await Connection.ChangeDatabaseAsync(databaseName, cancellationToken);

        /// <summary>
        /// Create query with parameter array
        /// </summary>
        public MySqlCommand CreateQuery(string cmd, object[] parameters = null)
        {
            var command = new MySqlCommand(cmd, Connection);
            
            if (parameters != null && parameters.Length > 0)
            {
                var cmParams = command.Parameters;
                for (int i = 0; i < parameters.Length; i++)
                    cmParams.AddWithValue($"@p{Max(cmParams.Count, 0)}", parameters[i]);
            }

            return command;
        }

        public DBConnection CreateQuery(string cmd, out MySqlCommand command, object[] parameters = null)
        {
            command = CreateQuery(cmd, parameters);
            return this;
        }

        public async Task<int> ExecuteNonQuery(MySqlCommand command)
        {
            if (!CanExecuteCommand)
            {
                int val = 0;
                var evnt = new Task(() =>
                {
                    val = command.ExecuteNonQuery();
                });
                WaitingCommands.Add((command, evnt));

                await evnt;//Utils.Waiter(() => reader != null);

                return val;
            }
            else
            {
                CanExecuteCommand = false;
                int val = command.ExecuteNonQuery();
                CanExecuteCommand = true;
                return val;
            }
        }

        /// <summary>
        /// Create query with parameters
        /// </summary>
        public MySqlCommand CreateQueryParams(string cmd, params object[] parameters)
        {
            return CreateQuery(cmd, parameters);
        }

        /// <summary>
        /// Create query with parameters
        /// </summary>
        public DBConnection CreateQueryParams(string cmd, out MySqlCommand command, params object[] parameters)
        {
            command = CreateQuery(cmd, parameters);
            return this;
        }

        /// <summary>
        /// Create query and execute with parameter array
        /// </summary>
        public async Task<DataReader> QueryAndExecuteAsync(string cmd, object[] parameters = null)
        {
            var q = CreateQuery(cmd, parameters);
            if (!CanExecuteCommand)
            {
                DataReader reader = null;
                var evnt = new Task(() =>
                {
                    reader = new DataReader(q.ExecuteReader(), this);
                });
                WaitingCommands.Add((q, evnt));

                await evnt;//Utils.Waiter(() => reader != null);

                return reader;
            }
            else
            {
                CanExecuteCommand = false;
                return new DataReader(q.ExecuteReader(), this);
            }
        }

        /// <summary>
        /// Create query and execute with parameter array
        /// </summary>
        public DataReader QueryAndExecute(string cmd, object[] parameters = null)
        {
            var rdr = QueryAndExecuteAsync(cmd, parameters);
            rdr.Wait();
            return rdr.Result;
        }

        /// <summary>
        /// Create query and execute with parameters
        /// </summary>
        public async Task<DataReader> QueryAndExecuteParamsAsync(string cmd, params object[] parameters)
        {
            return await QueryAndExecuteAsync(cmd, parameters);
        }


        public Task<DataReader> this[string command, object[] parameters] => QueryAndExecuteAsync(command, parameters);
        public Task<DataReader> this[string command] => QueryAndExecuteAsync(command);

        public long GetRowCount(string tableName)
        {
            using var r = (this[$"SELECT COUNT(*) FROM {tableName}"]);
            r.Wait();

            if (r.Result.Read())
                return r.Result.GetInt32(0);
            else
                return -1;
        }

        private async void Loop()
        {
            while(Connection.State != ConnectionState.Closed && Connection.State != ConnectionState.Broken)
            {
                if (CanExecuteCommand)
                {
                    if(WaitingCommands.TryTake(out var cnfg))
                    {
                        CanExecuteCommand = false;
                        cnfg.Item2.Start();
                    }
                }

                await Task.Delay(1);
            }
        }

        public void Open() { 
            Connection.Open();
            Loop();
        }
        public async Task OpenAsync(CancellationToken cancellationToken = default) => await Connection.OpenAsync(cancellationToken);

        public void Close() => Connection.Close();
        public async Task CloseAsync() => await Connection.CloseAsync();

        public void Dispose() => Connection.Dispose();
        public async ValueTask DisposeAsync() => await Connection.DisposeAsync();
    }
}
