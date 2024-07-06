using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Math;

namespace EasyMySql.Net
{
    public class DBConnection
    {
        public MySqlConnection connection;

        public DBConnection(string conString, bool open = true)
        {
            connection = new MySqlConnection(conString);
            if(open)
                connection.Open();
        }

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

        public MySqlCommand CreateQueryParams(string cmd, params object[] parameters)
        {
            return CreateQuery(cmd, parameters);
        }

        public DataReader QueryAndExecute(string cmd, object[] parameters)
        {
            return new DataReader(CreateQuery(cmd, parameters).ExecuteReader());
        }

        public DataReader QueryAndExecuteParams(string cmd, params object[] parameters)
        {
            return new DataReader(CreateQuery(cmd, parameters).ExecuteReader());
        }

        public void Open() => connection.Open();

        public void Close() => connection.Close();

        public void Dispose() => connection.Dispose();
    }
}
