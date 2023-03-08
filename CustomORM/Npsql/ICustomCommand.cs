using System.Data;

namespace CustomORM.Npsql;

public interface ICustomCommand : IAsyncDisposable
{
    Task<IDataReader> ExecuteReaderAsync(CancellationToken cancellationToken);
}