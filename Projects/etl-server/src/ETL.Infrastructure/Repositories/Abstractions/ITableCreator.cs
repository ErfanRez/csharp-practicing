using System.Data;

namespace ETL.Infrastructure.Repositories.Abstractions;

public interface ITableCreator
{
    Task CreateIfNotExistsAsync(string quotedTableName, IEnumerable<string> quotedColumnNames, IDbTransaction? tx = null, CancellationToken cancellationToken = default);
}
