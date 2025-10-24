using ETL.Application.Abstractions.Pipelines;
using ETL.Application.Common;
using ETL.Application.Common.DTOs;
using MediatR;

namespace ETL.Application.WorkFlow.Pipelines;

public record GetPipelineByIdQuery(Guid Id) : IRequest<Result<PipelineDto>>;


public sealed class GetPipelineByIdHandler : IRequestHandler<GetPipelineByIdQuery, Result<PipelineDto>>
{
    private readonly IGetPipelineById _getPipelineById;

    public GetPipelineByIdHandler(IGetPipelineById getPipelineById)
    {
        _getPipelineById = getPipelineById ?? throw new ArgumentNullException(nameof(getPipelineById));
    }

    public async Task<Result<PipelineDto>> Handle(GetPipelineByIdQuery request, CancellationToken cancellationToken)
    {
        var pipeline = await _getPipelineById.ExecuteAsync(request.Id, cancellationToken);
        if (pipeline == null)
            return Result.Failure<PipelineDto>(
                Error.NotFound("PipelineGet.Failed", $"Pipeline {request.Id} not found!"));

        var pipelineDto = new PipelineDto(pipeline.Id, pipeline.Name, pipeline.DataSourceId, pipeline.CreatedAt);
        return Result.Success(pipelineDto);
    }
}