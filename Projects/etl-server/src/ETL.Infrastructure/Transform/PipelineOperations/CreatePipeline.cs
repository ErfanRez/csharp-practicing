using ETL.Application.Abstractions.Pipelines;
using ETL.Domain.Entities;
using ETL.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ETL.Infrastructure.Transform.PipelineOperations;

public class CreatePipeline : ICreatePipeline
{
    private readonly IDbContextFactory<WorkflowDbContext> _contextFactory;
    public CreatePipeline(IDbContextFactory<WorkflowDbContext> contextFactory)
    {
        _contextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
    }

    public async Task<Guid> ExecuteAsync(Pipeline pipeline, CancellationToken cancellationToken = default)
    {
        await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);
        await context.Pipelines.AddAsync(pipeline, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        return pipeline.Id;
    }
}