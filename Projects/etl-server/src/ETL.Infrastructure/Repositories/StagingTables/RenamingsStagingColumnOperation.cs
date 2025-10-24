using System.Data;
using System.Data.Common;
using Dapper;
using ETL.Application.Abstractions.Repositories;
using ETL.Infrastructure.Data.Abstractions;
using ETL.Infrastructure.Repositories.Abstractions;

namespace ETL.Infrastructure.Repositories.StagingTables;

public sealed class RenameStagingColumnOperation : IRenameStagingColumn
{
    private readonly IDbConnectionFactory _connectionFactory;
    private readonly IIdentifierSanitizer _sanitizer;

    public RenameStagingColumnOperation(IDbConnectionFactory connectionFactory, IIdentifierSanitizer sanitizer)
    {
        _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
        _sanitizer = sanitizer ?? throw new ArgumentNullException(nameof(sanitizer));
    }

    public async Task ExecuteAsync(string tableName, string oldColumn, string newColumn, IDbTransaction? tx = null, CancellationToken cancellationToken = default)
    {
        var sanitizedTable = _sanitizer.Sanitize(tableName);
        var sanitizedOldCol = _sanitizer.Sanitize(oldColumn);
        var sanitizedNewCol = _sanitizer.Sanitize(newColumn);

        var sql = $"ALTER TABLE {sanitizedTable} RENAME COLUMN {sanitizedOldCol} TO {sanitizedNewCol};";

        if (tx != null)
        {
            await tx.Connection.ExecuteAsync(sql, null, tx);
            return;
        }

        using var conn = _connectionFactory.CreateConnection();
        if (conn is DbConnection dbConn)
            await dbConn.OpenAsync(cancellationToken);
        else
            conn.Open();

        await conn.ExecuteAsync(sql);
    }
}
