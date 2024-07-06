# EasyMySql

EasyMySql aims to provide an easy, fast writable, and comfortable usage for MySql.

## Getting Started

### Connect to MySql
```csharp
Console.WriteLine("Connecting...");
var con = new DBConnection(@"Server=localhost,3306;Database=voicenet;User=root;", true);
Console.WriteLine("Connected!");
```

### Query Command
```csharp
using var result = con.QueryAndExecuteParams("SELECT * FROM users WHERE userid=? OR username=?", id, name);
result.ForEachWithNames((i, keyValues) =>
{
  Console.WriteLine("Index: {0}", i);
  Console.WriteLine("Email: {0}", keyValues["email"]);
  Console.WriteLine("Username: {0}", keyValues["username"]);
});
```

### Query Command With JSON
```csharp
using var result = con.QueryAndExecuteParams("SELECT * FROM users WHERE userid=? OR username=?", id, name);
result.ForEach((i) =>
{
    Console.WriteLine("Index: {0}", i);
    Console.WriteLine("Json: {0}", result.GetJson());
});
```

### Requirements

Software and tools required to use the project:

- MySqlConnector
- Newtonsoft.Json

## Usage

Examples of how to use EasyMySql:

### Sample Code

```csharp
using EasyMySql.Net;

Console.WriteLine("Connecting...");
var con = new DBConnection(@"Server=localhost,3306;Database=voicenet;User=root;", true);
Console.WriteLine("Connected!");

Console.WriteLine("Write User Id");
var id = Console.ReadLine();

Console.WriteLine("Write User Name");
var name = Console.ReadLine();

using (var result = con.QueryAndExecuteParams("SELECT * FROM users WHERE userid=? OR username=?", id, name))
{
    result.ForEachWithNames((i, keyValues) =>
    {
        Console.WriteLine("Index: {0}", i);
        Console.WriteLine("Email: {0}", keyValues["email"]);
        Console.WriteLine("Username: {0}", keyValues["username"]);
    });
}
```
