namespace ETL.Infrastructure.Repositories.Abstractions;

public interface IStreamGetter
{
    Task<(Stream Stream, bool OwnsStream)> GetSeekableStreamAsync(Stream input, CancellationToken cancellationToken = default);
}
