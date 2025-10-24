using ETL.Application.Abstractions.Pipelines;
using ETL.Application.Common;
using ETL.Application.Common.DTOs;
using MediatR;

namespace ETL.Application.WorkFlow.Plugins;

public record GetPluginsByPipelineIdQuery(Guid PipelineId) : IRequest<Result<IEnumerable<PluginDto>>>;

public sealed class GetPluginsByPipelineIdQueryHandler 
    : IRequestHandler<GetPluginsByPipelineIdQuery, Result<IEnumerable<PluginDto>>>
{
    private readonly IGetPluginsByPipelineId _getPluginsByPipelineId;
    private readonly IGetPipelineById _getPipelineById;

    public GetPluginsByPipelineIdQueryHandler(
        IGetPluginsByPipelineId getPluginsByPipelineId,
        IGetPipelineById getPipelineById)
    {
        _getPluginsByPipelineId =
            getPluginsByPipelineId ?? throw new ArgumentNullException(nameof(getPluginsByPipelineId));
        _getPipelineById = getPipelineById ?? throw new ArgumentNullException(nameof(getPipelineById));
    }

    public async Task<Result<IEnumerable<PluginDto>>> Handle(GetPluginsByPipelineIdQuery request, CancellationToken cancellationToken)
    {
        var pipeline = await _getPipelineById.ExecuteAsync(request.PipelineId, cancellationToken);
        if (pipeline == null)
        {
            return Result.Failure<IEnumerable<PluginDto>>(
                Error.NotFound("PluginGet.Failed", $"Pipeline {request.PipelineId} not found"));
        }

        var items = await _getPluginsByPipelineId.ExecuteAsync(request.PipelineId, cancellationToken);
        var pluginDtos = items
            .Select(p => new PluginDto(p.Id, p.PipelineId, p.StepOrder, p.PluginType, p.Configuration, p.CreatedAt))
            .ToList();
        return Result.Success<IEnumerable<PluginDto>>(pluginDtos);
    }
}