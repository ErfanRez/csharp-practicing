using System.Globalization;
using CsvHelper;
using ETL.Infrastructure.Repositories.Abstractions;

namespace ETL.Infrastructure.Repositories;

public sealed class CsvHeaderReader : ICsvHeaderReader
{
    public async Task<string[]> ReadHeaderAsync(Stream seekableStream, CancellationToken cancellationToken = default)
    {
        if (!seekableStream.CanSeek) throw new ArgumentException("Stream must be seekable.", nameof(seekableStream));

        using var sr = new StreamReader(seekableStream, leaveOpen: true);
        using var csv = new CsvReader(sr, CultureInfo.InvariantCulture);

        if (!await csv.ReadAsync())
            throw new InvalidOperationException("CSV is empty");

        csv.ReadHeader();
        var headers = csv.HeaderRecord ?? throw new InvalidOperationException("Failed to read CSV header");
        return headers;
    }
}
