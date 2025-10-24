using System.Data.Common;
using Dapper;
using ETL.Application.Abstractions.Repositories;
using ETL.Domain.Entities;
using ETL.Infrastructure.Data.Abstractions;
using SqlKata;
using SqlKata.Compilers;

namespace ETL.Infrastructure.Repositories.DataSets;

public sealed class GetDataSetByIdOperation : IGetDataSetById
{
    private readonly IDbConnectionFactory _connectionFactory;
    private readonly Compiler _compiler;

    public GetDataSetByIdOperation(IDbConnectionFactory connectionFactory, Compiler compiler)
    {
        _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
        _compiler = compiler ?? throw new ArgumentNullException(nameof(compiler));
    }

    public async Task<DataSetMetadata?> ExecuteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var query = new Query("data_sets").Where("id", id).Select("*");
        var sqlResult = _compiler.Compile(query);

        using var conn = _connectionFactory.CreateConnection();
        if (conn is DbConnection dbConn)
            await dbConn.OpenAsync(cancellationToken);
        else
            conn.Open();

        return await conn.QuerySingleOrDefaultAsync<DataSetMetadata>(sqlResult.Sql, sqlResult.NamedBindings);
    }
}
