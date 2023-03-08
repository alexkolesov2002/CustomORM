using System.Text.RegularExpressions;
using Npgsql;

namespace CustomORM.Npsql;

public class NpgsqlConnectionAdapter : ICustomConnection
{
    private readonly NpgsqlConnection _npgsqlConnection;

    public NpgsqlConnectionAdapter(NpgsqlConnection npgsqlConnection)
    {
        _npgsqlConnection = npgsqlConnection;
    }

    public ICustomCommand CreateCommand(FormattableString querySql)
    {
        var command = _npgsqlConnection.CreateCommand();
        command.CommandText = ReplaceParameters(querySql.Format);
        for (var i = 0; i < querySql.ArgumentCount; i++)
        {
            command.Parameters.AddWithValue($"@p{i}", querySql.GetArgument(i)!);
        }
       
        
        return new NpgsqlCommandAdapter(command);
    }

    private static string ReplaceParameters(string query)
    {
        return Regex.Replace(query, @"\{(\d+)\}", x => $"@p{x.Groups[1].Value}"); // {0} --> @p1
    }
}