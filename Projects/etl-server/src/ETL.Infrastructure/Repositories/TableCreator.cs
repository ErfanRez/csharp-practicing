using System.Data;
using System.Data.Common;
using Dapper;
using ETL.Infrastructure.Data.Abstractions;
using ETL.Infrastructure.Repositories.Abstractions;

namespace ETL.Infrastructure.Repositories;

public sealed class TableCreator : ITableCreator
{
    private readonly IDbConnectionFactory _connectionFactory;

    public TableCreator(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
    }

    public async Task CreateIfNotExistsAsync(string quotedTableName, IEnumerable<string> quotedColumnNames, IDbTransaction? tx = null, CancellationToken cancellationToken = default)
    {
        var columnsWithTypes = quotedColumnNames.Select(q => $"{q} TEXT");
        var createSql = $"CREATE TABLE IF NOT EXISTS {quotedTableName} ({string.Join(", ", columnsWithTypes)});";

        if (tx != null)
        {
            await tx.Connection.ExecuteAsync(createSql, null, tx);
            return;
        }

        using var conn = _connectionFactory.CreateConnection();
        if (conn is DbConnection dbConn)
            await dbConn.OpenAsync(cancellationToken);
        else
            conn.Open();

        await conn.ExecuteAsync(createSql);
    }
}
