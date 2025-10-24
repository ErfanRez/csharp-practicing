using System.Data.Common;
using System.Text.Json;
using Dapper;
using ETL.Application.Abstractions.Repositories;
using ETL.Infrastructure.Data.Abstractions;
using ETL.Infrastructure.Repositories.Abstractions;
using SqlKata;
using SqlKata.Compilers;

namespace ETL.Infrastructure.Repositories.StagingTables;

public class GetStagingTableByName : IGetStagingTableByName
{
    private readonly IDbConnectionFactory _connectionFactory;
    private readonly Compiler _compiler;


    public GetStagingTableByName(IDbConnectionFactory connectionFactory, IIdentifierSanitizer sanitizer,
        Compiler compiler)
    {
        _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
        _compiler = compiler ?? throw new ArgumentNullException(nameof(compiler));
    }

    public async Task<string> ExecuteAsync(string tableName, CancellationToken cancellationToken = default)
    {
        var query = new Query(tableName)
            .Select("*");

        var sqlResult = _compiler.Compile(query);

        using var conn = _connectionFactory.CreateConnection();
        if (conn is DbConnection dbConn)
            await dbConn.OpenAsync(cancellationToken);
        else
            conn.Open();

        var result = await conn.QueryAsync(sqlResult.Sql, sqlResult.NamedBindings);
        return JsonSerializer.Serialize(result);
   
    }
}