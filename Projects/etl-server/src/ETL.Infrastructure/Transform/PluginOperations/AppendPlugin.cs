using ETL.Application.Abstractions.Pipelines;
using ETL.Domain.Entities;
using ETL.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ETL.Infrastructure.Transform.PluginOperations;

public sealed class AppendPlugin : IAppendPlugin
{
    private readonly IDbContextFactory<WorkflowDbContext> _contextFactory;

    public AppendPlugin(IDbContextFactory<WorkflowDbContext> contextFactory)
    {
        _contextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
    }

    public async Task<Guid> ExecuteAsync(Guid pipelineId, string pluginType, string configuration, CancellationToken cancellationToken = default)
    {
        await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);

        var lastStepOrder = await context.Plugins
            .Where(p => p.PipelineId == pipelineId)
            .MaxAsync(p => (int?)p.StepOrder, cancellationToken) ?? 0;

        var plugin = new Plugin(pipelineId, lastStepOrder + 1, pluginType, configuration);
        await context.Plugins.AddAsync(plugin, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        return plugin.Id;
    }
}