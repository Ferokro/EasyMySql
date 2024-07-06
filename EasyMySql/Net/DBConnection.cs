using MySqlConnector;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
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
        public MySqlConnection connection;

        public DBConnection(string conString, bool open = true)
        {
            connection = new MySqlConnection(conString);
            if(open)
                connection.Open();
        }

        public string ConnectionString
        {
            get => connection.ConnectionString;
            set => connection.ConnectionString = value;
        }
        public int ServerThread => connection.ServerThread;
        public string ServerVersion => connection.ServerVersion;
        public bool CanCreateBatch => connection.CanCreateBatch;
        public string DataSource => connection.DataSource;
        public string DataBase => connection.Database;
        public ConnectionState State => connection.State;
        public ISite? Site
        {
            get => connection.Site;
            set => connection.Site = value;
        }
        public int ConnectionTimeout => connection.ConnectionTimeout;
        public IContainer? Container => connection.Container;

        //Events
        public Func<X509CertificateCollection, ValueTask>? ProvideClientCertificatesCallback
        {
            get => connection.ProvideClientCertificatesCallback;
            set => connection.ProvideClientCertificatesCallback = value;
        }
        public Func<MySqlProvidePasswordContext, string>? ProvidePasswordCallback
        {
            get => connection.ProvidePasswordCallback;
            set => connection.ProvidePasswordCallback = value;
        }
        public RemoteCertificateValidationCallback? RemoteCertificateValidationCallback
        {
            get => connection.RemoteCertificateValidationCallback;
            set => connection.RemoteCertificateValidationCallback = value;
        }

        public bool Ping() => connection.Ping();
        public async Task<bool> PingAsync(CancellationToken cancellationToken = default) => await connection.PingAsync(cancellationToken);
        public DataTable GetSchema() => connection.GetSchema();
        public async Task<DataTable> GetSchemaAsync(CancellationToken cancellationToken = default) => await connection.GetSchemaAsync(cancellationToken);
        public MySqlTransaction BeginTransaction(IsolationLevel isolationLevel, bool isReadOnly) => connection.BeginTransaction(isolationLevel, isReadOnly);
        public async Task<MySqlTransaction> BeginTransactionAsync(IsolationLevel isolationLevel, bool isReadOnly, CancellationToken cancellationToken = default) => await connection.BeginTransactionAsync(isolationLevel, isReadOnly, cancellationToken);
        public MySqlBatch CreateBatch() => connection.CreateBatch();
        public MySqlCommand CreateCommand() => connection.CreateCommand();
        public async ValueTask ResetConnectionAsync(CancellationToken cancellationToken = default) => connection.ResetConnectionAsync(cancellationToken);

        public void ChangeDatabase(string databaseName) => connection.ChangeDatabase(databaseName);
        public async Task ChangeDatabaseAsync(string databaseName, CancellationToken cancellationToken = default) => 
            await connection.ChangeDatabaseAsync(databaseName, cancellationToken);

        /// <summary>
        /// Create query with parameter array
        /// </summary>
        public MySqlCommand CreateQuery(string cmd, object[] parameters = null)
        {
            var command = new MySqlCommand(cmd, connection);
            
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
        public DataReader QueryAndExecute(string cmd, object[] parameters = null)
        {
            return new DataReader(CreateQuery(cmd, parameters).ExecuteReader());
        }

        /// <summary>
        /// Create query and execute with parameter array
        /// </summary>
        public DBConnection QueryAndExecute(string cmd, out DataReader reader, object[] parameters = null)
        {
            reader = new DataReader(CreateQuery(cmd, parameters).ExecuteReader());
            return this;
        }

        /// <summary>
        /// Create query and execute with parameters
        /// </summary>
        public DataReader QueryAndExecuteParams(string cmd, params object[] parameters)
        {
            return new DataReader(CreateQuery(cmd, parameters).ExecuteReader());
        }

        /// <summary>
        /// Create query and execute with parameters
        /// </summary>
        public DBConnection QueryAndExecuteParams(string cmd, out DataReader reader, params object[] parameters)
        {
            reader = new DataReader(CreateQuery(cmd, parameters).ExecuteReader());
            return this;
        }

        public DataReader this[string command, object[] parameters] => QueryAndExecute(command, parameters);
        public DataReader this[string command] => QueryAndExecute(command);

        public long GetRowCount(string tableName) => this[$"SELECT COUNT(*) FROM {tableName}"].GetInt32(0);

        public void Open() => connection.Open();
        public async Task OpenAsync(CancellationToken cancellationToken = default) => await connection.OpenAsync(cancellationToken);

        public void Close() => connection.Close();
        public async Task CloseAsync() => await connection.CloseAsync();

        public void Dispose() => connection.Dispose();
        public async ValueTask DisposeAsync() => await connection.DisposeAsync();
    }
}
