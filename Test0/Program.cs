using EasyMySql.Net;

public class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Connecting...");

        while (true)
        {
            Console.WriteLine("Write User Id");
            var id = Console.ReadLine();

            Console.WriteLine("Write User Name");
            var name = Console.ReadLine();

            Console.WriteLine("Columns or Json (c, j) ?");
            var cType = Console.ReadLine();

            using (var result = con.QueryAndExecuteParams("SELECT * FROM users WHERE userid=? OR username=?", id, name))
            {
                //Columns Type
                if (cType.ToLower().StartsWith('c'))
                    result.ForEachWithNames((i, keyValues) =>
                    {
                        Console.WriteLine("Index: {0}", i);
                        Console.WriteLine("Email: {0}", keyValues["email"]);
                        Console.WriteLine("Username: {0}", keyValues["username"]);
                    });

                //Json Type
                else if (cType.ToLower().StartsWith('j'))
                    result.ForEach((i) =>
                    {
                        Console.WriteLine("Index: {0}", i);
                        Console.WriteLine("Json: {0}", result.GetJson());
                    });

                else
                    Console.WriteLine("Undefined Type!!!");
            }
        }
    }
}