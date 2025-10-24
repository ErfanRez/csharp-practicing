using System.Data;
using System.Data.Common;
using ETL.Infrastructure.Data.Abstractions;
using ETL.Infrastructure.Repositories.Abstractions;
using Npgsql;

namespace ETL.Infrastructure.Repositories;

public sealed class CsvCopyImporter : ICsvCopyImporter
{
    private readonly IDbConnectionFactory _connectionFactory;

    public CsvCopyImporter(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
    }

    public async Task ImportAsync(string quotedTableName, IEnumerable<string> quotedColumnNames, Stream csvSeekableStream, IDbTransaction? tx = null, CancellationToken cancellationToken = default)
    {
        if (!csvSeekableStream.CanSeek) throw new ArgumentException("csvSeekableStream must be seekable", nameof(csvSeekableStream));

        var copyColumns = string.Join(", ", quotedColumnNames);
        var copySql = $"COPY {quotedTableName} ({copyColumns}) FROM STDIN (FORMAT CSV, HEADER true)";

        IDbConnection? localConnToDispose = null;
        var connectionForCopy = tx?.Connection;
        try
        {
            if (connectionForCopy == null)
            {
                localConnToDispose = _connectionFactory.CreateConnection();
                if (localConnToDispose is DbConnection dbConn)
                    await dbConn.OpenAsync(cancellationToken);
                else
                    localConnToDispose.Open();

                connectionForCopy = localConnToDispose;
            }

            if (connectionForCopy is not NpgsqlConnection npgsql)
                throw new InvalidOperationException("COPY requires NpgsqlConnection.");

            csvSeekableStream.Position = 0;
            using var reader = new StreamReader(csvSeekableStream, leaveOpen: true);
            using var writer = await npgsql.BeginTextImportAsync(copySql);

            var buffer = new char[81920];
            while (!cancellationToken.IsCancellationRequested)
            {
                var read = await reader.ReadAsync(buffer, 0, buffer.Length);
                if (read == 0) break;
                await writer.WriteAsync(buffer, 0, read);
            }

            await writer.FlushAsync(cancellationToken);
        }
        finally
        {
            localConnToDispose?.Dispose();
        }
    }
}
