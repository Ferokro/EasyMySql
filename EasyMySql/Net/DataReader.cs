using MySqlConnector;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyMySql.Net
{
    public class DataReader : IDisposable
    {
        public MySqlDataReader dataReader;

        public DataReader(MySqlDataReader reader)
        {
            dataReader = reader;
        }

        public int Depth => dataReader.Depth;
        public int FieldCount => dataReader.FieldCount;
        public int VisibleFieldCount => dataReader.VisibleFieldCount;
        public int accors => dataReader.RecordsAffected;

        public bool HasRows => dataReader.HasRows;
        public bool IsClosed => dataReader.IsClosed;

        public object this[string s] => dataReader[s];

        public bool Read() => dataReader.Read();
        public async Task<bool> ReadAsync(CancellationToken cancellationToken = default) => await dataReader.ReadAsync(cancellationToken);
        public bool NextResult() => dataReader.NextResult();
        public async Task<bool> NextResultAsync(CancellationToken cancellationToken = default) => await dataReader.NextResultAsync(cancellationToken);
        public int GetValues(object[] values) => dataReader.GetValues(values);
        public string GetName(int ordinal) => dataReader.GetName(ordinal);
        public object GetValue(int ordinal) => dataReader.GetValue(ordinal);

        /// <summary>
        /// Get columns names and values
        /// </summary>
        public Dictionary<string, object> GetNamesAndValues()
        {
            var values = new object[FieldCount];
            GetValues(values);
            var valuesAndNames = new Dictionary<string, object>();

            for (int i = 0; i < values.Length; i++)
                valuesAndNames.Add(GetName(i), values[i]);

            return valuesAndNames;
        }

        public string GetJson() => JsonConvert.SerializeObject(GetNamesAndValues());

        /// <summary>
        /// Try read values
        /// </summary>
        public bool TryRead(out object[] values)
        {
            values = null;
            var rVal = Read();
            if (!rVal)
                return false;

            values = new object[FieldCount];
            GetValues(values);

            return true;
        }

        /// <summary>
        /// Try read columns names and values
        /// </summary>
        public bool TryReadWithNames(out Dictionary<string, object> namesAndValues)
        {
            namesAndValues = null;
            var rVal = Read();
            if (!rVal)
                return false;

            namesAndValues = GetNamesAndValues();

            return true;
        }

        /// <summary>
        /// Foreach rows for action index
        /// </summary>
        public void ForEach(Action<int> task)
        {
            for (int i = 0; Read(); i++)
                task(i);
        }

        /// <summary>
        /// Foreach rows for action columns with index
        /// </summary>
        public void ForEachWithValues(Action<int, object[]> task)
        {
            for (int i = 0; TryRead(out object[] cols); i++)
                task(i, cols);
        }

        /// <summary>
        /// Foreach rows for action columns and names with index
        /// </summary>
        public void ForEachWithNames(Action<int, Dictionary<string, object>> task)
        {
            for (int i = 0; TryReadWithNames(out Dictionary<string, object> cols); i++)
                task(i, cols);
        }


        public void Dispose()
        {
            if(!dataReader.IsClosed)
                dataReader.Close();
        }
    }
}
