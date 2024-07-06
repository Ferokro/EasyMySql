using System.Data.Common;
using static System.Math;
using EasyMySql.Net;

Console.WriteLine("Connecting...");
var con = new DBConnection(@"Server=localhost,3306;Database=voicenet;User=root;");
Console.WriteLine("Connected!");

while (true)
{
    using var result = con.QueryAndExecuteParams("SELECT * FROM users WHERE userid=? OR username=?",
        Console.ReadLine(),
        Console.ReadLine()
    );

    result.dataReader.Cast<MySqlConnector.MySqlAttribute>();
    result.ForEach(i => Console.WriteLine(result.GetValuesJson()));
}