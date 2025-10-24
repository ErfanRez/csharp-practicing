using ETL.Application.Abstractions.Pipelines;
using ETL.Domain.Entities;
using ETL.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ETL.Infrastructure.Transform.PluginOperations;

public sealed class GetPluginsByPipelineId : IGetPluginsByPipelineId
{
    private readonly IDbContextFactory<WorkflowDbContext> _contextFactory;

    public GetPluginsByPipelineId(IDbContextFactory<WorkflowDbContext> contextFactory)
    {
        _contextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
    }

    public async Task<IEnumerable<Plugin>> ExecuteAsync(Guid pipelineId, CancellationToken cancellationToken = default)
    {
        await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);
        return await context.Plugins
            .Where(p => p.PipelineId == pipelineId)
            .OrderBy(p => p.StepOrder)
            .ToListAsync(cancellationToken);
    }
}