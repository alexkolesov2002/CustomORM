using Npgsql;

namespace CustomORM.Npsql;

public static class NpgsqlConnectionExtensions
{
    public static ICustomConnection Adapt(this NpgsqlConnection connection)
    {
        return new NpgsqlConnectionAdapter(connection);
    }
}