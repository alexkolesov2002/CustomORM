using System.Data;

namespace CustomORM;

public interface ICustomCommand : IAsyncDisposable
{
    Task<IDataReader> ExecuteReaderAsync(CancellationToken cancellationToken);
}