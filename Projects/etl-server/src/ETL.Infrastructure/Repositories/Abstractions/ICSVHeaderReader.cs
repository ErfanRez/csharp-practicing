namespace ETL.Infrastructure.Repositories.Abstractions;

public interface ICsvHeaderReader
{
    Task<string[]> ReadHeaderAsync(Stream seekableStream, CancellationToken cancellationToken = default);
}
