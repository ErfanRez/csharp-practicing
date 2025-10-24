using System.Data.Common;
using Dapper;
using ETL.Application.Abstractions.Repositories;
using ETL.Infrastructure.Data.Abstractions;
using SqlKata;
using SqlKata.Compilers;

namespace ETL.Infrastructure.Repositories.StagingTables;

public sealed class StagingColumnExistsOperation : IStagingColumnExists
{
    private readonly IDbConnectionFactory _connectionFactory;
    private readonly Compiler _compiler;

    public StagingColumnExistsOperation(IDbConnectionFactory connectionFactory, Compiler compiler)
    {
        _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
        _compiler = compiler ?? throw new ArgumentNullException(nameof(compiler));
    }

    public async Task<bool> ExecuteAsync(string tableName, string columnName, CancellationToken cancellationToken = default)
    {
        var query = new Query("information_schema.columns")
            .Where("table_name", tableName)
            .Where("column_name", columnName)
            .Where("table_schema", "public")
            .AsCount();

        var sqlResult = _compiler.Compile(query);

        using var conn = _connectionFactory.CreateConnection();
        if (conn is DbConnection dbConn)
            await dbConn.OpenAsync(cancellationToken);
        else
            conn.Open();

        var count = await conn.ExecuteScalarAsync<int>(new CommandDefinition(sqlResult.Sql, sqlResult.NamedBindings, cancellationToken: cancellationToken));
        return count > 0;
    }
}
