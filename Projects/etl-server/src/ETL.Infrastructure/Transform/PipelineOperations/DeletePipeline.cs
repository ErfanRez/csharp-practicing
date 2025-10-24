using ETL.Application.Abstractions.Pipelines;
using ETL.Domain.Entities;
using ETL.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ETL.Infrastructure.Transform.PipelineOperations;

public class DeletePipeline : IDeletePipeline
{
    private readonly IDbContextFactory<WorkflowDbContext> _contextFactory;
    public DeletePipeline(IDbContextFactory<WorkflowDbContext> contextFactory)
    { 
        _contextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
    }

    public async Task ExecuteAsync(Pipeline pipeline, CancellationToken cancellationToken = default)
    {
        await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);
        context.Pipelines.Remove(pipeline);
        await context.SaveChangesAsync(cancellationToken);
    }
}