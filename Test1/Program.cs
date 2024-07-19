using EasyMySql.Net;
using EasyMySql.Net.Attributes;
using EasyMySql.Net.Test;
using System.Data.Common;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Data;
using MySqlConnector;

namespace Test1;

[method: DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(User))]
public class User()
{
    [DBColumn("userid")] public int UserId { get; set; }
    [DBColumn("username")] public string UserName { get; set; }
    [DBColumn("email")] public string Email { get; set; }
}

public class Program
{
    static void Main(string[] args)
    {
        Test();
    }

    static void Test()
    {
        Console.WriteLine("Connecting...");
        var con = new DBConnection(@"Server=localhost,3306;Database=test;User=root;", true);
        Console.WriteLine("Connected!");


        var r = con.CreateQuery("SELECT * FROM users");
        //r.ExecuteNonQuery();

        var rdr = r.ExecuteReader();
        var tbl = rdr.GetSchemaTable();
        rdr.Close();

        foreach (DataRow row in tbl.Rows)
        {
            var r1 = con.CreateQuery("SELECT * FROM users");
            /*
            var rdr1 = r1.ExecuteReader();
            while (rdr1.Read())
            {
                Console.WriteLine("     " + rdr1["username"]);
            }*/
        }


        /*
        Console.ReadLine();


        var r = con.CreateQuery("SELECT * FROM users");
        //r.ExecuteNonQuery();

        var rdr = r.ExecuteReader();

        while (rdr.Read())
        {
            Console.WriteLine(rdr["username"]);

            var conn = con;//new DBConnection(@"Server=localhost,3306;Database=test;User=root;", true);

            var r1 = conn.CreateQuery("SELECT * FROM users");

            var rdr1 = r1.ExecuteReader();
            while (rdr1.Read())
            {
                Console.WriteLine("     " + rdr1["username"]);
            }
            rdr1.Close();
            conn.Close();
        }*/

        /*
        var watch = Stopwatch.StartNew();
        using var result = con["SELECT * FROM users"];

        result.ForEachCustom<User>((i, u) => {
            if(i == 0)
                Console.WriteLine("User: ({0},{1},{2})", u.UserId, u.UserName, u.Email);
        });

        watch.Stop();
        Console.WriteLine("Completed in {0}", watch.Elapsed);*/
        Console.ReadLine();
    }
}