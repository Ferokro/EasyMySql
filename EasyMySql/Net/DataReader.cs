using MySqlConnector;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.Common;
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

        public short GetInt16(int ordinal) => dataReader.GetInt16(ordinal);
        public int GetInt32(int ordinal) => dataReader.GetInt32(ordinal);
        public long GetInt64(int ordinal) => dataReader.GetInt64(ordinal);

        public bool GetBoolean(int ordinal) => dataReader.GetBoolean(ordinal);
        public byte GetByte(int ordinal) => dataReader.GetByte(ordinal);
        public long GetBytes(int ordinal, long dataOffset, byte[]? buffer, int bufferOffset, int length) => dataReader.GetBytes(ordinal, dataOffset, buffer, bufferOffset, length);
        public sbyte GetSByte(int ordinal) => dataReader.GetSByte(ordinal);
        public Type GetFieldType(int ordinal) => dataReader.GetFieldType(ordinal);
        public T GetFieldType<T>(int ordinal) => dataReader.GetFieldValue<T>(ordinal);
        public int GetOrdinal(string name) => dataReader.GetOrdinal(name);
        public decimal GetDecimal(int ordinal) => dataReader.GetDecimal(ordinal);
        public MySqlDecimal GetMySqlDecimal(int ordinal) => dataReader.GetMySqlDecimal(ordinal);
        public char GetChar(int ordinal) => dataReader.GetChar(ordinal);
        public long GetChars(int ordinal, long dataOffset, char[]? buffer, int bufferOffset, int length) => dataReader.GetChars(ordinal, dataOffset, buffer, bufferOffset, length);
        public double GetDouble(int ordinal) => dataReader.GetDouble(ordinal);
        public DateTime GetDateTime(int ordinal) => dataReader.GetDateTime(ordinal);
        public DateOnly GetDateOnly(int ordinal) => dataReader.GetDateOnly(ordinal);
        public DateTimeOffset GetDateTimeOffset(int ordinal) => dataReader.GetDateTimeOffset(ordinal);
        public string GetDataTypeName(int ordinal) => dataReader.GetDataTypeName(ordinal);
        public MySqlDateTime GetMySqlDateTime(int ordinal) => dataReader.GetMySqlDateTime(ordinal);
        public float GetFloat(int ordinal) => dataReader.GetFloat(ordinal);
        public Guid GetGuid(int ordinal) => dataReader.GetGuid(ordinal);
        public string GetString(int ordinal) => dataReader.GetString(ordinal);
        public Stream GetStream(int ordinal) => dataReader.GetStream(ordinal);
        public TextReader GetTextReader(int ordinal) => dataReader.GetTextReader(ordinal);
        public MySqlGeometry GetMySqlGeometry(int ordinal) => dataReader.GetMySqlGeometry(ordinal);
        public ReadOnlyCollection<DbColumn> GetColumnSchema() => dataReader.GetColumnSchema();
        public async Task<ReadOnlyCollection<DbColumn>> GetColumnSchemaAsync(CancellationToken cancellationToken = default) => await dataReader.GetColumnSchemaAsync(cancellationToken);
        public DataTable? GetSchemaTable() => dataReader.GetSchemaTable();
        public async Task<DataTable?> GetSchemaTableAsync(CancellationToken cancellationToken = default) => await dataReader.GetSchemaTableAsync(cancellationToken);

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

        /// <summary>
        /// Get json of row
        /// </summary>
        public string GetJson() => JsonConvert.SerializeObject(GetNamesAndValues());

        public DataReader GetJson(out string json)
        {
            json = JsonConvert.SerializeObject(GetNamesAndValues());
            return this;
        }

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
        public DataReader ForEach(Action<int> task)
        {
            for (int i = 0; Read(); i++)
                task(i);

            return this;
        }

        /// <summary>
        /// Foreach rows for action columns with index
        /// </summary>
        public DataReader ForEachWithValues(Action<int, object[]> task)
        {
            for (int i = 0; TryRead(out object[] cols); i++)
                task(i, cols);

            return this;
        }

        /// <summary>
        /// Foreach rows for action columns and names with index
        /// </summary>
        public DataReader ForEachWithNames(Action<int, Dictionary<string, object>> task)
        {
            for (int i = 0; TryReadWithNames(out Dictionary<string, object> cols); i++)
                task(i, cols);

            return this;
        }

        public void Close()
        {
            if (!dataReader.IsClosed)
                dataReader.Close();
        }

        public void Dispose()
        {
            Close();
        }
    }
}
