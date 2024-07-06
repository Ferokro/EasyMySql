using MySqlConnector;
using SqlKata;
using SqlKata.Compilers;
using TestConsole0.Databasae;
using static System.Math;

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