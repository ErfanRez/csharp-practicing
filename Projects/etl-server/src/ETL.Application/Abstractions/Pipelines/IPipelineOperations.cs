using ETL.Domain.Entities;

namespace ETL.Application.Abstractions.Pipelines;

public interface IGetAllPipelines
{
    Task<IEnumerable<Pipeline>> ExecuteAsync(CancellationToken cancellationToken = default);
}

public interface IGetPipelinesByCreatorId
{
    Task<IEnumerable<Pipeline>> ExecuteAsync(string createdById, CancellationToken cancellationToken = default);
}

public interface IGetPipelineById
{
    Task<Pipeline?> ExecuteAsync(Guid id, CancellationToken cancellationToken = default);
}

public interface ICreatePipeline
{
    Task<Guid> ExecuteAsync(Pipeline pipeline, CancellationToken cancellationToken = default);
}

public interface IUpdatePipeline
{
    Task ExecuteAsync(Guid id, string newName, CancellationToken cancellationToken = default);
}

public interface IDeletePipeline
{
    Task ExecuteAsync(Pipeline pipeline, CancellationToken cancellationToken = default);
}