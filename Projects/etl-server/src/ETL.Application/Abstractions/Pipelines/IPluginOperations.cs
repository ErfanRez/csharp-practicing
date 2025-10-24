using ETL.Domain.Entities;

namespace ETL.Application.Abstractions.Pipelines;

public interface IGetPluginsByPipelineId
{
    Task<IEnumerable<Plugin>> ExecuteAsync(Guid pipelineId, CancellationToken cancellationToken = default);
}

public interface IGetPluginById
{
    Task<Plugin?> ExecuteAsync(Guid pluginId, CancellationToken cancellationToken = default);
}

public interface IAppendPlugin
{
    Task<Guid> ExecuteAsync(Guid pipelineId, string pluginType, string configuration, CancellationToken cancellationToken = default);
}

public interface IUpdatePlugin
{
    Task ExecuteAsync(Plugin plugin, CancellationToken cancellationToken = default);
}

public interface IDeletePlugin
{
    Task ExecuteAsync(Plugin plugin, CancellationToken cancellationToken = default);
}