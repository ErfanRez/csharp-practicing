using ETL.Application.Abstractions.Pipelines;
using ETL.Domain.Entities;
using ETL.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ETL.Infrastructure.Transform.PipelineOperations;

public class GetPipelinesByCreatorId: IGetPipelinesByCreatorId
{
    private readonly IDbContextFactory<WorkflowDbContext> _contextFactory;

    public GetPipelinesByCreatorId(IDbContextFactory<WorkflowDbContext> contextFactory)
    {
        _contextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
    }

    public async Task<IEnumerable<Pipeline>> ExecuteAsync(string createdById, CancellationToken cancellationToken = default)
    {
        await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);

        return await context.Pipelines
            .Where(p => p.CreatedByUserId == createdById)
            .ToListAsync(cancellationToken);
    }

}
