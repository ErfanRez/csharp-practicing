using System.Data;

namespace ETL.Infrastructure.Repositories.Abstractions;

public interface ICsvCopyImporter
{
    Task ImportAsync(string quotedTableName, IEnumerable<string> quotedColumnNames, Stream csvSeekableStream, IDbTransaction? tx = null, CancellationToken cancellationToken = default);
}
