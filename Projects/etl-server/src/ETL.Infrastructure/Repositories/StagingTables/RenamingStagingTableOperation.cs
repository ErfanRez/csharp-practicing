using System.Data;
using System.Data.Common;
using Dapper;
using ETL.Application.Abstractions.Repositories;
using ETL.Infrastructure.Data.Abstractions;
using ETL.Infrastructure.Repositories.Abstractions;

namespace ETL.Infrastructure.Repositories.StagingTables;

public sealed class RenameStagingTableOperation : IRenameStagingTable
{
    private readonly IDbConnectionFactory _connectionFactory;
    private readonly IIdentifierSanitizer _sanitizer;

    public RenameStagingTableOperation(IDbConnectionFactory connectionFactory, IIdentifierSanitizer sanitizer)
    {
        _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
        _sanitizer = sanitizer ?? throw new ArgumentNullException(nameof(sanitizer));
    }

    public async Task ExecuteAsync(string oldName, string newName, IDbTransaction? tx = null, CancellationToken cancellationToken = default)
    {
        var sanitizedOld = _sanitizer.Sanitize(oldName);
        var sanitizedNew = _sanitizer.Sanitize(newName);

        var sql = $"ALTER TABLE {sanitizedOld} RENAME TO {sanitizedNew};";

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
