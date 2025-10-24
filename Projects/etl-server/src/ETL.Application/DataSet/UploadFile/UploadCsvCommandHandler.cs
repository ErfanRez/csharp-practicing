using System.Data;
using ETL.Application.Abstractions.Data;
using ETL.Application.Abstractions.Repositories;
using ETL.Application.Common;
using ETL.Domain.Entities;
using MediatR;

namespace ETL.Application.DataSet.UploadFile;

public record UploadCsvCommand(string TableName, Stream FileStream, string UserId) : IRequest<Result>;

public sealed class UploadCsvCommandHandler : IRequestHandler<UploadCsvCommand, Result>
{
    private readonly IUnitOfWork _uow;
    private readonly ICreateTableFromCsv _createTableOp;
    private readonly IAddDataSet _addDataSetOp;
    private readonly IGetDataSetByTableName _getByTableNameOp;

    public UploadCsvCommandHandler(
        IUnitOfWork uow,
        ICreateTableFromCsv createTableOp,
        IAddDataSet addDataSetOp,
        IGetDataSetByTableName getByTableNameOp)
    {
        _uow = uow ?? throw new ArgumentNullException(nameof(uow));
        _createTableOp = createTableOp ?? throw new ArgumentNullException(nameof(createTableOp));
        _addDataSetOp = addDataSetOp ?? throw new ArgumentNullException(nameof(addDataSetOp));
        _getByTableNameOp = getByTableNameOp ?? throw new ArgumentNullException(nameof(getByTableNameOp));
    }

    public async Task<Result> Handle(UploadCsvCommand request, CancellationToken cancellationToken)
    {
        var existing = await _getByTableNameOp.ExecuteAsync(request.TableName, cancellationToken);
        if (existing is not null)
            return Result.Failure(Error.Conflict("FileUpload.Failed", $"Table '{request.TableName}' already exists."));

        IDbTransaction? tx = null;
        try
        {
            tx = _uow.BeginTransaction();

            var dataSet = new DataSetMetadata(request.TableName, request.UserId);

            await _createTableOp.ExecuteAsync(request.TableName, request.FileStream, tx, cancellationToken);

            await _addDataSetOp.ExecuteAsync(dataSet, tx, cancellationToken);

            _uow.CommitTransaction(tx);
            tx = null;

            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure(Error.Problem("FileUpload.Failed", ex.Message));
        }
        finally
        {
            _uow.RollbackTransaction(tx);
        }
    }
}
