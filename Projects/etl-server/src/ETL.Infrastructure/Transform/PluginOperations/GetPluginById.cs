using ETL.Application.Abstractions.Pipelines;
using ETL.Domain.Entities;
using ETL.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ETL.Infrastructure.Transform.PluginOperations;

public sealed class GetPluginById : IGetPluginById
{
    private readonly IDbContextFactory<WorkflowDbContext> _contextFactory;

    public GetPluginById(IDbContextFactory<WorkflowDbContext> contextFactory)
    {
        _contextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
    }

    public async Task<Plugin?> ExecuteAsync(Guid pluginId, CancellationToken cancellationToken = default)
    {
        await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);
        return await context.Plugins.FirstOrDefaultAsync(p => p.Id == pluginId, cancellationToken);
    }
}