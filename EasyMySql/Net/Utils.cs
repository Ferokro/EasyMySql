using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using EasyMySql.Net.Attributes;

namespace EasyMySql.Net
{
    public static class Utils
    {
        public static async Task Waiter(Func<bool> control, int wait = 1)
        {
            while(!control())
            {
                await Task.Delay(wait);
            }

            return;
        }

        public static object GetObjectOfInfoAndValues(Type type, Dictionary<string, (MemberTypes, object)> namesAndInfos, Dictionary<string, object> namesAndValues)
        {
            //var obj = Activator.CreateInstance(type);
            var obj = FormatterServices.GetUninitializedObject(type);

            foreach (var nm in namesAndValues)
            {
                if (namesAndInfos.ContainsKey(nm.Key))
                {
                    var info = namesAndInfos[nm.Key];

                    if (info.Item1 == MemberTypes.Field)
                    {
                        var f = ((FieldInfo)info.Item2);
                        f.SetValue(obj, Convert.ChangeType(nm.Value, f.FieldType));
                    }
                    else if (info.Item1 == MemberTypes.Property)
                    {
                        var p = ((PropertyInfo)info.Item2);
                        p.SetValue(obj, Convert.ChangeType(nm.Value, p.PropertyType));
                    }
                }
            }

            return obj;
        }

        public static T GetObjectOfInfoAndValues<T>(Dictionary<string, (MemberTypes, object)> namesAndInfos, Dictionary<string, object> namesAndValues)
        {
            return (T)GetObjectOfInfoAndValues(typeof(T), namesAndInfos, namesAndValues);
        }

        public static Dictionary<string, (MemberTypes, object)> GetVariableInfosOfType(Type type)
        {
            var namesAndInfos = new Dictionary<string, (MemberTypes, object)>();

            var fields = type.GetFields();
            foreach (var f in fields)
            {
                var at = f.GetCustomAttribute<DBColumn>(false);
                namesAndInfos.Add(at != null ? at.Name : f.Name, (f.MemberType, f));
            }

            var props = type.GetProperties();
            foreach (var p in props)
            {
                var at = p.GetCustomAttribute<DBColumn>(false);
                if (p.CanRead)
                    namesAndInfos.Add(at != null ? at.Name : p.Name, (p.MemberType, p));
            }

            return namesAndInfos;
        }

        public static Dictionary<string, (MemberTypes, object)> GetVariableInfosOfType<T>()
        {
            return GetVariableInfosOfType(typeof(T));
        }

        public static object GetObjectOfDictionary(Dictionary<string, object> keyAndValues, Type type)
        {
            //var obj = Activator.CreateInstance(type);
            var obj = FormatterServices.GetUninitializedObject(type);

            foreach (var kvp in keyAndValues)
            {
                var fields = obj.GetType().GetFields();
                foreach (var f in fields)
                {
                    var at = f.GetCustomAttribute<DBColumn>(false);
                    if ((at != null && at.Name == kvp.Key) || f.Name == kvp.Key)
                        f.SetValue(obj, Convert.ChangeType(kvp.Value, f.FieldType));
                }

                var props = obj.GetType().GetProperties();
                foreach (var p in props)
                {
                    var at = p.GetCustomAttribute<DBColumn>(false);
                    if (p.CanWrite && ((at != null && at.Name == kvp.Key) || p.Name == kvp.Key))
                        p.SetValue(obj, Convert.ChangeType(kvp.Value, p.PropertyType));
                }
            }
            return obj;
        }

        public static Dictionary<string, object> GetDictionaryOfObject(object obj)
        {
            var keyAndValues = new Dictionary<string, object>();

            var fields = obj.GetType().GetFields();
            foreach (var f in fields)
            {
                var at = f.GetCustomAttribute<DBColumn>(false);
                keyAndValues.Add(at != null ? at.Name : f.Name, f.GetValue(obj));
            }

            var props = obj.GetType().GetProperties();
            foreach (var p in props)
            {
                var at = p.GetCustomAttribute<DBColumn>(false);
                if (p.CanRead)
                    keyAndValues.Add(at != null ? at.Name : p.Name, p.GetValue(obj));
            }

            return keyAndValues;
        }

        public static T GetObjectOfDictionary<T>(Dictionary<string, object> keyAndValues)
        {
            return (T)GetObjectOfDictionary(keyAndValues, typeof(T));
        }

        public static Dictionary<string, object> GetDictionaryOfObject<T>(T obj)
        {
            return GetDictionaryOfObject(obj);
        }
    }
}
