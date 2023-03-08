// See https://aka.ms/new-console-template for more information

using System.Reflection.Metadata;
using CustomORM;
using CustomORM.Npsql;
using Npgsql;
using Document = CustomORM.Document;

await using var connection = new NpgsqlConnection("Host=localhost;Database=FullTextGames;User Id = postgres; Password=qwerty1234");
await connection.OpenAsync();
var adapterConnection = connection.Adapt();
int id = 1;
FormattableString sqlQuery = $"""
Select * from  "Documents" WHERE "Id" > {id};
""";

foreach (var document in await adapterConnection.QueryAsync<Document>(sqlQuery, new CancellationToken()))
{
    Console.WriteLine(document.Id + " " + document.Content);
}