using CustomORM;
using CustomORM.CustomEF;
using CustomORM.Npsql;

public  class FullTextGamesDbContext : CustomContext
{
    public CustomDbSet<Document> Documents { get; set; }

    public FullTextGamesDbContext(ICustomConnection connection) : base(connection)
    {
    }
}