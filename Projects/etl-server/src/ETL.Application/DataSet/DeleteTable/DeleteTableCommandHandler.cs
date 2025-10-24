// ETL.Application.DataSet/DeleteTableCommandHandler.cs
using System.Data;
using ETL.Application.Abstractions.Data;
using ETL.Application.Abstractions.Repositories;
using ETL.Application.Common;
using MediatR;

namespace ETL.Application.DataSet.DeleteTable;

public record DeleteTableCommand(string TableName) : IRequest<Result>;

public sealed class DeleteTableCommandHandler : IRequestHandler<DeleteTableCommand, Result>
{
    private readonly IUnitOfWork _uow;
    private readonly IGetDataSetByTableName _getByTableName;
    private readonly IDeleteStagingTable _deleteStagingTable;
    private readonly IDeleteDataSet _deleteDataSet;

    public DeleteTableCommandHandler(
        IUnitOfWork uow,
        IGetDataSetByTableName getByTableName,
        IDeleteStagingTable deleteStagingTable,
        IDeleteDataSet deleteDataSet)
    {
        _uow = uow ?? throw new ArgumentNullException(nameof(uow));
        _getByTableName = getByTableName ?? throw new ArgumentNullException(nameof(getByTableName));
        _deleteStagingTable = deleteStagingTable ?? throw new ArgumentNullException(nameof(deleteStagingTable));
        _deleteDataSet = deleteDataSet ?? throw new ArgumentNullException(nameof(deleteDataSet));
    }

    public async Task<Result> Handle(DeleteTableCommand request, CancellationToken cancellationToken)
    {
        var existingDataSet = await _getByTableName.ExecuteAsync(request.TableName, cancellationToken);
        if (existingDataSet == null)
        {
            return Result.Failure(
                Error.NotFound("TableRemove.Failed", $"Table '{request.TableName}' not found!"));
        }

        IDbTransaction? tx = null;
        try
        {
            tx = _uow.BeginTransaction();

            await _deleteStagingTable.ExecuteAsync(request.TableName, tx, cancellationToken);

            await _deleteDataSet.ExecuteAsync(existingDataSet, tx, cancellationToken);

            _uow.CommitTransaction(tx);
            tx = null;

            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure(Error.Problem("TableRemove.Failed", ex.Message));
        }
        finally
        {
            _uow.RollbackTransaction(tx);
        }
    }
}
