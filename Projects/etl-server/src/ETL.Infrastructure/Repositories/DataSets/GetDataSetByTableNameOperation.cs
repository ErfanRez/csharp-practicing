using System.Data.Common;
using Dapper;
using ETL.Application.Abstractions.Repositories;
using ETL.Domain.Entities;
using ETL.Infrastructure.Data.Abstractions;
using SqlKata;
using SqlKata.Compilers;

namespace ETL.Infrastructure.Repositories.DataSets;

public sealed class GetDataSetByTableNameOperation : IGetDataSetByTableName
{
    private readonly IDbConnectionFactory _connectionFactory;
    private readonly Compiler _compiler;

    public GetDataSetByTableNameOperation(IDbConnectionFactory connectionFactory, Compiler compiler)
    {
        _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
        _compiler = compiler ?? throw new ArgumentNullException(nameof(compiler));
    }

    public async Task<DataSetMetadata?> ExecuteAsync(string tableName, CancellationToken cancellationToken = default)
    {
        var query = new Query("data_sets").Where("table_name", tableName).Select("*");
        var sqlResult = _compiler.Compile(query);

        using var conn = _connectionFactory.CreateConnection();
        if (conn is DbConnection dbConn)
            await dbConn.OpenAsync(cancellationToken);
        else
            conn.Open();

        return await conn.QueryFirstOrDefaultAsync<DataSetMetadata>(sqlResult.Sql, sqlResult.NamedBindings);
    }
}
