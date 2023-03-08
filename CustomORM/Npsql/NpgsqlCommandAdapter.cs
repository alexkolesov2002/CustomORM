using System.Data;
using Npgsql;

namespace CustomORM.Npsql;

public class NpgsqlCommandAdapter : ICustomCommand
{
    private readonly NpgsqlCommand _commandAdapter;


    public NpgsqlCommandAdapter(NpgsqlCommand commandAdapter)
    {
        _commandAdapter = commandAdapter;
    }

    public async Task<IDataReader> ExecuteReaderAsync(CancellationToken cancellationToken)
    {
        return await _commandAdapter.ExecuteReaderAsync(cancellationToken);
    }

    public async ValueTask DisposeAsync()
    {
        await _commandAdapter.DisposeAsync();
    }
}