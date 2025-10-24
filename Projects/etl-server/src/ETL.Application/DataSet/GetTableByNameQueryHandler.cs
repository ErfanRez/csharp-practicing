using ETL.Application.Abstractions.Repositories;
using ETL.Application.Common;
using MediatR;

namespace ETL.Application.DataSet;

public record GetTableByNameQuery(string TableName) : IRequest<Result<string>>;

public sealed class GetTableByNameQueryHandler : IRequestHandler<GetTableByNameQuery, Result<string>>
{
    private readonly IGetStagingTableByName _getStagingTableByName;
    private readonly IGetDataSetByTableName _getByTableName;


    public GetTableByNameQueryHandler(IGetStagingTableByName getStagingTableByName, IGetDataSetByTableName getByTableName)
    {
        _getStagingTableByName = getStagingTableByName ?? throw new ArgumentNullException(nameof(getStagingTableByName));
        _getByTableName = getByTableName ?? throw new ArgumentNullException(nameof(getByTableName));
    }

    public async Task<Result<string>> Handle(GetTableByNameQuery request, CancellationToken cancellationToken)
    {
        var dataset = await _getByTableName.ExecuteAsync(request.TableName, cancellationToken);
        if (dataset == null)
        {
            return Result.Failure<string>(
                Error.NotFound("Table.Get.Failed", $"Table '{request.TableName}' not found!"));
        }

        return await _getStagingTableByName.ExecuteAsync(request.TableName, cancellationToken);
    }
}