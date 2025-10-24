using ETL.Infrastructure.Repositories.Abstractions;

namespace ETL.Infrastructure.Repositories;

public sealed class StreamGetter : IStreamGetter
{
    public async Task<(Stream Stream, bool OwnsStream)> GetSeekableStreamAsync(Stream input, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(input);

        if (input.CanSeek)
        {
            input.Position = 0;
            return (input, false);
        }

        var ms = new MemoryStream();
        await input.CopyToAsync(ms, 81920, cancellationToken);
        ms.Position = 0;
        return (ms, true);
    }
}
