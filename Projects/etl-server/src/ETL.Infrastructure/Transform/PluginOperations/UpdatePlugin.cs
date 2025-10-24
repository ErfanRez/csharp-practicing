using ETL.Application.Abstractions.Pipelines;
using ETL.Domain.Entities;
using ETL.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ETL.Infrastructure.Transform.PluginOperations;

public sealed class UpdatePlugin : IUpdatePlugin
{
    private readonly IDbContextFactory<WorkflowDbContext> _contextFactory;

    public UpdatePlugin(IDbContextFactory<WorkflowDbContext> contextFactory)
    {
        _contextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
    }

    public async Task ExecuteAsync(Plugin plugin, CancellationToken cancellationToken = default)
    {
        await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);

        context.Plugins.Update(plugin);
        await context.SaveChangesAsync(cancellationToken);
    }
}