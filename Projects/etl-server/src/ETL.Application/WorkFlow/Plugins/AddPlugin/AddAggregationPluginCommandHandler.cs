using System.Text.Json;
using ETL.Application.Abstractions.Pipelines;
using ETL.Application.Common;
using ETL.Application.Common.Constants;
using ETL.Application.Common.DTOs.PluginConfigurations;
using MediatR;

namespace ETL.Application.WorkFlow.Plugins.AddPlugin;

public record AddAggregationPluginCommand(Guid PipelineId, AggregationPluginConfiguration Configuration)
    : IRequest<Result<Guid>>;

public sealed class AddAggregationPluginCommandHandler : IRequestHandler<AddAggregationPluginCommand, Result<Guid>>
{
    private readonly IAppendPlugin _appendPlugin;
    private readonly IGetPipelineById _getPipelineById;


    public AddAggregationPluginCommandHandler(IAppendPlugin appendPlugin, IGetPipelineById getPipelineById)
    {
        _appendPlugin = appendPlugin ?? throw new ArgumentNullException(nameof(appendPlugin));
        _getPipelineById = getPipelineById ?? throw new ArgumentNullException(nameof(getPipelineById));
    }

    public async Task<Result<Guid>> Handle(AddAggregationPluginCommand request, CancellationToken ct)
    {
        var pipeline = await _getPipelineById.ExecuteAsync(request.PipelineId, ct);
        if (pipeline == null)
        {
            return Result.Failure<Guid>(Error.NotFound("PluginCreate.Failed",
                $"pipeline {request.PipelineId} not found"));
        }
        
        var configJson = JsonSerializer.Serialize(request.Configuration);

        var pluginId =
            await _appendPlugin.ExecuteAsync(request.PipelineId, PluginTypes.Aggregation, configJson, ct);

        return Result.Success(pluginId);
    }
}