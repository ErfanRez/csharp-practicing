using System.Data;
using ETL.Application.Abstractions.Repositories;
using ETL.Infrastructure.Repositories.Abstractions;

namespace ETL.Infrastructure.Repositories.StagingTables;

public sealed class CreateTableFromCsvOperation : ICreateTableFromCsv
{
    private readonly IStreamGetter _streamProvider;
    private readonly ICsvHeaderReader _headerReader;
    private readonly IIdentifierSanitizer _sanitizer;
    private readonly ITableCreator _tableCreator;
    private readonly ICsvCopyImporter _copyImporter;

    public CreateTableFromCsvOperation(
        IStreamGetter streamProvider,
        ICsvHeaderReader headerReader,
        IIdentifierSanitizer sanitizer,
        ITableCreator tableCreator,
        ICsvCopyImporter copyImporter)
    {
        _streamProvider = streamProvider ?? throw new ArgumentNullException(nameof(streamProvider));
        _headerReader = headerReader ?? throw new ArgumentNullException(nameof(headerReader));
        _sanitizer = sanitizer ?? throw new ArgumentNullException(nameof(sanitizer));
        _tableCreator = tableCreator ?? throw new ArgumentNullException(nameof(tableCreator));
        _copyImporter = copyImporter ?? throw new ArgumentNullException(nameof(copyImporter));
    }

    public async Task ExecuteAsync(string tableName, Stream csvStream, IDbTransaction? tx = null, CancellationToken cancellationToken = default)
    {
        var (seekable, owns) = await _streamProvider.GetSeekableStreamAsync(csvStream, cancellationToken);

        try
        {
            var headers = await _headerReader.ReadHeaderAsync(seekable, cancellationToken);
            if (headers == null || headers.Length == 0) throw new InvalidOperationException("CSV header is empty");

            var quotedColumns = headers.Select(h => _sanitizer.Sanitize(h)).ToArray();
            var quotedTable = _sanitizer.Sanitize(tableName);

            await _tableCreator.CreateIfNotExistsAsync(quotedTable, quotedColumns, tx, cancellationToken);

            await _copyImporter.ImportAsync(quotedTable, quotedColumns, seekable, tx, cancellationToken);
        }
        finally
        {
            if (owns)
            {
                seekable.Dispose();
            }
        }
    }
}
