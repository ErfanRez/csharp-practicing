using ETL.Application.Abstractions.Pipelines;
using ETL.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ETL.Infrastructure.Transform.PipelineOperations;

public class UpdatePipeline : IUpdatePipeline
{
    private readonly IDbContextFactory<WorkflowDbContext> _contextFactory;
    public UpdatePipeline(IDbContextFactory<WorkflowDbContext> contextFactory)
    { 
        _contextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
    }

    public async Task ExecuteAsync(Guid id, string newName, CancellationToken cancellationToken = default)
    {
        await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);
        var pipeline = await context.Pipelines.FindAsync(new object[] { id }, cancellationToken);
        if (pipeline == null)
            throw new KeyNotFoundException($"Pipeline {id} not found");

        pipeline.Rename(newName);
        await context.SaveChangesAsync(cancellationToken);
    }
}