using Microsoft.VisualBasic;
using MySqlConnector;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EasyMySql.Net
{
    public class DataReader : IDisposable
    {
        public MySqlDataReader SqlDataReader;

        public DataReader(MySqlDataReader reader)
        {
            SqlDataReader = reader;
        }

        public int Depth => SqlDataReader.Depth;
        public int FieldCount => SqlDataReader.FieldCount;
        public int VisibleFieldCount => SqlDataReader.VisibleFieldCount;
        public int RecordsAffected => SqlDataReader.RecordsAffected;

        public bool HasRows => SqlDataReader.HasRows;
        public bool IsClosed => SqlDataReader.IsClosed;

        public object this[string s] => SqlDataReader[s];

        public bool Read() => SqlDataReader.Read();
        public async Task<bool> ReadAsync(CancellationToken cancellationToken = default) => await SqlDataReader.ReadAsync(cancellationToken);
        public bool NextResult() => SqlDataReader.NextResult();
        public async Task<bool> NextResultAsync(CancellationToken cancellationToken = default) => await SqlDataReader.NextResultAsync(cancellationToken);
        public int GetValues(object[] values) => SqlDataReader.GetValues(values);
        public string GetName(int ordinal) => SqlDataReader.GetName(ordinal);
        public object GetValue(int ordinal) => SqlDataReader.GetValue(ordinal);

        public short GetInt16(int ordinal) => SqlDataReader.GetInt16(ordinal);
        public int GetInt32(int ordinal) => SqlDataReader.GetInt32(ordinal);
        public long GetInt64(int ordinal) => SqlDataReader.GetInt64(ordinal);

        public bool GetBoolean(int ordinal) => SqlDataReader.GetBoolean(ordinal);
        public byte GetByte(int ordinal) => SqlDataReader.GetByte(ordinal);
        public long GetBytes(int ordinal, long dataOffset, byte[]? buffer, int bufferOffset, int length) => SqlDataReader.GetBytes(ordinal, dataOffset, buffer, bufferOffset, length);
        public sbyte GetSByte(int ordinal) => SqlDataReader.GetSByte(ordinal);
        public Type GetFieldType(int ordinal) => SqlDataReader.GetFieldType(ordinal);
        public T GetFieldType<T>(int ordinal) => SqlDataReader.GetFieldValue<T>(ordinal);
        public int GetOrdinal(string name) => SqlDataReader.GetOrdinal(name);
        public decimal GetDecimal(int ordinal) => SqlDataReader.GetDecimal(ordinal);
        public MySqlDecimal GetMySqlDecimal(int ordinal) => SqlDataReader.GetMySqlDecimal(ordinal);
        public char GetChar(int ordinal) => SqlDataReader.GetChar(ordinal);
        public long GetChars(int ordinal, long dataOffset, char[]? buffer, int bufferOffset, int length) => SqlDataReader.GetChars(ordinal, dataOffset, buffer, bufferOffset, length);
        public double GetDouble(int ordinal) => SqlDataReader.GetDouble(ordinal);
        public DateTime GetDateTime(int ordinal) => SqlDataReader.GetDateTime(ordinal);
        public DateOnly GetDateOnly(int ordinal) => SqlDataReader.GetDateOnly(ordinal);
        public DateTimeOffset GetDateTimeOffset(int ordinal) => SqlDataReader.GetDateTimeOffset(ordinal);
        public string GetDataTypeName(int ordinal) => SqlDataReader.GetDataTypeName(ordinal);
        public MySqlDateTime GetMySqlDateTime(int ordinal) => SqlDataReader.GetMySqlDateTime(ordinal);
        public float GetFloat(int ordinal) => SqlDataReader.GetFloat(ordinal);
        public Guid GetGuid(int ordinal) => SqlDataReader.GetGuid(ordinal);
        public string GetString(int ordinal) => SqlDataReader.GetString(ordinal);
        public Stream GetStream(int ordinal) => SqlDataReader.GetStream(ordinal);
        public TextReader GetTextReader(int ordinal) => SqlDataReader.GetTextReader(ordinal);
        public MySqlGeometry GetMySqlGeometry(int ordinal) => SqlDataReader.GetMySqlGeometry(ordinal);
        public ReadOnlyCollection<DbColumn> GetColumnSchema() => SqlDataReader.GetColumnSchema();
        public async Task<ReadOnlyCollection<DbColumn>> GetColumnSchemaAsync(CancellationToken cancellationToken = default) => await SqlDataReader.GetColumnSchemaAsync(cancellationToken);
        public DataTable? GetSchemaTable() => SqlDataReader.GetSchemaTable();
        public async Task<DataTable?> GetSchemaTableAsync(CancellationToken cancellationToken = default) => await SqlDataReader.GetSchemaTableAsync(cancellationToken);

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
        /// Foreach rows for custom types with index
        /// </summary>
        public DataReader ForEachCustom<T>(Action<int, T> task)
        {
            return ForEachCustom(typeof(T), (i, o) => task(i, (T)o));
        }

        /// <summary>
        /// Foreach rows for custom types with index
        /// </summary>
        public DataReader ForEachCustom(Type type, Action<int, object> task)
        {
            var infos = Utils.GetVariableInfosOfType(type);

            for (int i = 0; Read(); i++)
                task(i, GetObject(type, infos));

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

        /// <summary>
        /// Get object for custom types
        /// </summary>
        public object GetObject(Type type)
        {
            return Utils.GetObjectOfDictionary(GetNamesAndValues(), type);
        }

        /// <summary>
        /// Get object for custom types
        /// </summary>
        public object GetObject(Type type, Dictionary<string, (MemberTypes, object)> infos)
        {
            return Utils.GetObjectOfInfoAndValues(type, infos, GetNamesAndValues());
        }

        /// <summary>
        /// Get object for custom types
        /// </summary>
        public T GetObject<T>()
        {
            return (T)GetObject(typeof(T));
        }

        /// <summary>
        /// Get object for custom types
        /// </summary>
        public T GetObject<T>(Dictionary<string, (MemberTypes, object)> infos)
        {
            return (T)GetObject(typeof(T), infos);
        }

        /// <summary>
        /// Read object for custom types
        /// </summary>
        public object ReadObject(Type type)
        {
            if (Read())
                return GetObject(type);

            return default;
        }

        /// <summary>
        /// Read object for custom types
        /// </summary>
        public object ReadObject(Type type, Dictionary<string, (MemberTypes, object)> infos)
        {
            if (Read())
                return GetObject(type, infos);

            return default;
        }

        /// <summary>
        /// Read object for custom types
        /// </summary>
        public T ReadObject<T>()
        {
            return (T)ReadObject(typeof(T));
        }

        /// <summary>
        /// Read object for custom types
        /// </summary>
        public T ReadObject<T>(Dictionary<string, (MemberTypes, object)> infos)
        {
            return (T)ReadObject(typeof(T), infos);
        }

        ~DataReader()
        {
            SqlDataReader = null;
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Read object for custom types
        /// </summary>
        public void Close()
        {
            if (!SqlDataReader.IsClosed)
                SqlDataReader.Close();
        }

        /// <summary>
        /// Read object for custom types
        /// </summary>
        public void End()
        {
            if (!SqlDataReader.IsClosed)
                SqlDataReader.Close();
        }

        public void Dispose()
        {
            Close();
        }
    }
}
