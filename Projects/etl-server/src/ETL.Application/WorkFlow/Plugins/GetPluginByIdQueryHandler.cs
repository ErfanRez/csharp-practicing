using ETL.Application.Abstractions.Pipelines;
using ETL.Application.Common;
using ETL.Application.Common.DTOs;
using ETL.Domain.Entities;
using MediatR;

namespace ETL.Application.WorkFlow.Plugins;

public record GetPluginByIdQuery(Guid PluginId) : IRequest<Result<PluginDto>>;

public sealed class GetPluginByIdQueryHandler : IRequestHandler<GetPluginByIdQuery, Result<PluginDto>>
{
    private readonly IGetPluginById _getPluginById;

    public GetPluginByIdQueryHandler(IGetPluginById getPluginById)
    {
        _getPluginById = getPluginById ?? throw new ArgumentNullException(nameof(getPluginById));
    }

    public async Task<Result<PluginDto>> Handle(GetPluginByIdQuery request, CancellationToken cancellationToken)
    {
        var plugin = await _getPluginById.ExecuteAsync(request.PluginId, cancellationToken);
        if (plugin is null)
        {
            return Result.Failure<PluginDto>(
                Error.NotFound("PluginGet.Failed", $"Plugin {request.PluginId} not found"));
        }

        var pluginDto = new PluginDto(plugin.Id, plugin.PipelineId, plugin.StepOrder, plugin.PluginType,
            plugin.Configuration, plugin.CreatedAt);

        return Result.Success(pluginDto);
    }
}