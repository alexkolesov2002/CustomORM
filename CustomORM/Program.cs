using CustomORM;using CustomORM.CustomEF;
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

var context = new FullTextGamesDbContext();
var docs = context.Documents.Where(x => x.Id > 1);
var list = docs.ToList();

foreach (var document in await adapterConnection.QueryAsync<Document>(sqlQuery, new CancellationToken()))
{
    Console.WriteLine(document.Id + " " + document.Content);
}

public  class FullTextGamesDbContext : CustomContext
{
    public CustomDbSet<Document> Documents { get; set; }
}