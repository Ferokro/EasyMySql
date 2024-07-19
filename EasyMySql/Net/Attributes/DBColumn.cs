using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyMySql.Net.Attributes
{
    //<summary>
    //Names the columns in the table
    //</summary>
    [method: DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(DBColumn))]
    public class DBColumn(string name) : Attribute
    {
        public string Name { get; init; } = name;

        ~DBColumn()
        {
            GC.SuppressFinalize(this);
        }
    }
}
