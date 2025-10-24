using ETL.Application.Abstractions.Pipelines;
using ETL.Application.Common;
using ETL.Application.Common.DTOs;
using MediatR;

namespace ETL.Application.WorkFlow.Pipelines;
public record GetAllPipelinesQuery() : IRequest<Result<IEnumerable<PipelineDto>>>;

public sealed class GetAllPipelinesHandler : IRequestHandler<GetAllPipelinesQuery, Result<IEnumerable<PipelineDto>>>
{
    private readonly IGetAllPipelines _getAllPipelines;

    public GetAllPipelinesHandler(IGetAllPipelines getAllPipelines)
    {
        _getAllPipelines = getAllPipelines ?? throw new ArgumentNullException(nameof(getAllPipelines));
    }

    public async Task<Result<IEnumerable<PipelineDto>>> Handle(GetAllPipelinesQuery request, CancellationToken cancellationToken)
    {
        var items = await _getAllPipelines.ExecuteAsync(cancellationToken);
        var pipelineDtos = items
            .Select(p => new PipelineDto(p.Id, p.Name, p.DataSourceId, p.CreatedAt))
            .ToList();

        return Result.Success<IEnumerable<PipelineDto>>(pipelineDtos);

    }
}