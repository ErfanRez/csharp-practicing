using System.Text.Json;
using ETL.Application.Abstractions.Pipelines;
using ETL.Application.Common;
using ETL.Application.Common.Constants;
using ETL.Application.Common.DTOs.PluginConfigurations;
using MediatR;

namespace ETL.Application.WorkFlow.Plugins.UpdatePlugin;

public record UpdateAggregationPluginCommand(Guid PluginId, AggregationPluginConfiguration Configuration) : IRequest<Result>;

public sealed class UpdateAggregationPluginCommandHandler : IRequestHandler<UpdateAggregationPluginCommand, Result>
{
    private readonly IUpdatePlugin _updatePlugin;
    private readonly IGetPluginById _getPluginById;

    public UpdateAggregationPluginCommandHandler(IUpdatePlugin updatePlugin, IGetPluginById getPluginById)
    {
        _updatePlugin = updatePlugin ?? throw new ArgumentNullException(nameof(updatePlugin));
        _getPluginById = getPluginById ?? throw new ArgumentNullException(nameof(getPluginById));
    }

    public async Task<Result> Handle(UpdateAggregationPluginCommand request, CancellationToken cancellationToken)
    {
        var plugin = await _getPluginById.ExecuteAsync(request.PluginId, cancellationToken);
        if (plugin == null)
            return Result.Failure(Error.NotFound("PluginUpdate.Failed", $"Plugin {request.PluginId} not found"));

        if (plugin.PluginType != PluginTypes.Aggregation)
            return Result.Failure(Error.Validation("PluginUpdate.Failed",
                $"Plugin {request.PluginId} is not {PluginTypes.Aggregation}"));

        var configJson = JsonSerializer.Serialize(request.Configuration);

        plugin.ChangeConfiguration(configJson);

        await _updatePlugin.ExecuteAsync(plugin, cancellationToken);

        return Result.Success();
    }
}