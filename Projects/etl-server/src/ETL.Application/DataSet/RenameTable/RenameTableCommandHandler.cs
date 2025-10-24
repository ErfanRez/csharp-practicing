using System.Data;
using ETL.Application.Abstractions.Data;
using ETL.Application.Abstractions.Repositories;
using ETL.Application.Common;
using MediatR;

namespace ETL.Application.DataSet.RenameTable;

public record RenameTableCommand(string OldTableName, string NewTableName) : IRequest<Result>;

public sealed class RenameTableCommandHandler : IRequestHandler<RenameTableCommand, Result>
{
    private readonly IUnitOfWork _uow;
    private readonly IGetDataSetByTableName _getByTableName;
    private readonly IRenameStagingTable _renameStagingTable;
    private readonly IUpdateDataSet _updateDataSet;

    public RenameTableCommandHandler(
        IUnitOfWork uow,
        IGetDataSetByTableName getByTableName,
        IRenameStagingTable renameStagingTable,
        IUpdateDataSet updateDataSet)
    {
        _uow = uow ?? throw new ArgumentNullException(nameof(uow));
        _getByTableName = getByTableName ?? throw new ArgumentNullException(nameof(getByTableName));
        _renameStagingTable = renameStagingTable ?? throw new ArgumentNullException(nameof(renameStagingTable));
        _updateDataSet = updateDataSet ?? throw new ArgumentNullException(nameof(updateDataSet));
    }

    public async Task<Result> Handle(RenameTableCommand request, CancellationToken cancellationToken)
    {
        var existingDataSet = await _getByTableName.ExecuteAsync(request.OldTableName, cancellationToken);
        if (existingDataSet == null)
        {
            return Result.Failure(
                Error.NotFound("TableRename.Failed", $"Table '{request.OldTableName}' not found!"));
        }

        var newDataSet = await _getByTableName.ExecuteAsync(request.NewTableName, cancellationToken);
        if (newDataSet != null)
        {
            return Result.Failure(Error.Conflict("TableRename.Failed",
                $"Table '{request.NewTableName}' already exists."));
        }

        IDbTransaction? tx = null;
        try
        {
            tx = _uow.BeginTransaction();

            await _renameStagingTable.ExecuteAsync(request.OldTableName, request.NewTableName, tx, cancellationToken);

            existingDataSet.Rename(request.NewTableName);
            await _updateDataSet.ExecuteAsync(existingDataSet, tx, cancellationToken);

            _uow.CommitTransaction(tx);
            tx = null;

            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure(Error.Problem("TableRename.Failed", ex.Message));
        }
        finally
        {
            _uow.RollbackTransaction(tx);
        }
    }
}