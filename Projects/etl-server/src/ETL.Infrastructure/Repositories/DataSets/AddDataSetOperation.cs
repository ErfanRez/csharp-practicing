using System.Data;
using System.Data.Common;
using Dapper;
using ETL.Application.Abstractions.Repositories;
using ETL.Domain.Entities;
using ETL.Infrastructure.Data.Abstractions;
using SqlKata;
using SqlKata.Compilers;

namespace ETL.Infrastructure.Repositories.DataSets;

public sealed class AddDataSetOperation : IAddDataSet
{
    private readonly IDbConnectionFactory _connectionFactory;
    private readonly Compiler _compiler;

    public AddDataSetOperation(IDbConnectionFactory connectionFactory, Compiler compiler)
    {
        _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
        _compiler = compiler ?? throw new ArgumentNullException(nameof(compiler));
    }

    public async Task ExecuteAsync(DataSetMetadata dataSet, IDbTransaction? tx = null, CancellationToken cancellationToken = default)
    {
        var query = new Query("data_sets").AsInsert(new
        {
            id = dataSet.Id,
            table_name = dataSet.TableName,
            uploaded_by_user_id = dataSet.UploadedByUserId,
            uploaded_at = dataSet.CreatedAt,
        });

        var sqlResult = _compiler.Compile(query);

        if (tx != null)
        {
            await tx.Connection.ExecuteAsync(sqlResult.Sql, sqlResult.NamedBindings, tx);
            return;
        }

        using var conn = _connectionFactory.CreateConnection();
        if (conn is DbConnection dbConn)
            await dbConn.OpenAsync(cancellationToken);
        else
            conn.Open();

        await conn.ExecuteAsync(sqlResult.Sql, sqlResult.NamedBindings);
    }
}
