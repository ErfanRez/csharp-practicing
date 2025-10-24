using ETL.Application.Abstractions.Repositories;
using ETL.Application.Common;
using ETL.Application.Common.DTOs;
using MediatR;

namespace ETL.Application.DataSet;

public record GetAllDataSetsQuery() : IRequest<Result<IEnumerable<DataSetDto>>>;

public sealed class GetAllDataSetsQueryHandler : IRequestHandler<GetAllDataSetsQuery, Result<IEnumerable<DataSetDto>>>
{
    private readonly IGetAllDataSets _getAllDataSets;

    public GetAllDataSetsQueryHandler(IGetAllDataSets getAllDataSets)
    {
        _getAllDataSets = getAllDataSets ?? throw new ArgumentNullException(nameof(getAllDataSets));
    }

    public async Task<Result<IEnumerable<DataSetDto>>> Handle(GetAllDataSetsQuery request, CancellationToken cancellationToken)
    {
        var items = await _getAllDataSets.ExecuteAsync(cancellationToken);

        var dataSetDtos = items
            .Select(d => new DataSetDto(d.Id, d.TableName, d.UploadedByUserId, d.CreatedAt))
            .ToList();

        return Result.Success<IEnumerable<DataSetDto>>(dataSetDtos);
    }
}
