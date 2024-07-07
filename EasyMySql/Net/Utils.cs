using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EasyMySql.Net
{
    public static class Utils
    {
        public static T GetObjectOfDictionary<T>(Dictionary<string, object> keyAndValues)
        {
            var obj = Activator.CreateInstance<T>();
            foreach (var kvp in keyAndValues)
            {
                var field = obj.GetType().GetField(kvp.Key);
                if (field != null)
                {
                    field.SetValue(obj, Convert.ChangeType(kvp.Value, field.FieldType));
                    continue;
                }

                var prop = obj.GetType().GetProperty(kvp.Key);
                if (prop != null && prop.CanWrite)
                {
                    prop.SetValue(obj, Convert.ChangeType(kvp.Value, prop.PropertyType), null);
                    continue;
                }
            }
            return obj;
        }
    }
}
