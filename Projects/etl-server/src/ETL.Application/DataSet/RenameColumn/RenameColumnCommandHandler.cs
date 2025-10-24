using System.Data;
using ETL.Application.Abstractions.Data;
using ETL.Application.Abstractions.Repositories;
using ETL.Application.Common;
using MediatR;

namespace ETL.Application.DataSet.RenameColumn;

public record RenameColumnCommand(string TableName, string OldColumnName, string NewColumnName) : IRequest<Result>;

public sealed class RenameColumnCommandHandler : IRequestHandler<RenameColumnCommand, Result>
{
    private readonly IUnitOfWork _uow;
    private readonly IGetDataSetByTableName _getByTableName;
    private readonly IStagingColumnExists _columnExists;
    private readonly IRenameStagingColumn _renameStagingColumn;

    public RenameColumnCommandHandler(
        IUnitOfWork uow,
        IGetDataSetByTableName getByTableName,
        IStagingColumnExists columnExists,
        IRenameStagingColumn renameStagingColumn)
    {
        _uow = uow ?? throw new ArgumentNullException(nameof(uow));
        _getByTableName = getByTableName ?? throw new ArgumentNullException(nameof(getByTableName));
        _columnExists = columnExists ?? throw new ArgumentNullException(nameof(columnExists));
        _renameStagingColumn = renameStagingColumn ?? throw new ArgumentNullException(nameof(renameStagingColumn));
    }

    public async Task<Result> Handle(RenameColumnCommand request, CancellationToken cancellationToken)
    {
        var existing = await _getByTableName.ExecuteAsync(request.TableName, cancellationToken);
        if (existing == null)
        {
            return Result.Failure(
                Error.NotFound("ColumnRename.Failed", $"Table '{request.TableName}' not found!"));
        }

        var oldColumnExist = await _columnExists.ExecuteAsync(request.TableName, request.OldColumnName, cancellationToken);
        if (!oldColumnExist)
        {
            return Result.Failure(
                Error.NotFound("ColumnRename.Failed", $"Column '{request.OldColumnName}' not found!"));
        }

        var newColumnExist = await _columnExists.ExecuteAsync(request.TableName, request.NewColumnName, cancellationToken);
        if (newColumnExist)
        {
            return Result.Failure(
                Error.Conflict("ColumnRename.Failed", $"Column '{request.NewColumnName}' already exists."));
        }

        IDbTransaction? tx = null;
        try
        {
            tx = _uow.BeginTransaction();

            await _renameStagingColumn.ExecuteAsync(request.TableName, request.OldColumnName, request.NewColumnName, tx, cancellationToken);

            _uow.CommitTransaction(tx);
            tx = null;

            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure(Error.Problem("ColumnRename.Failed", ex.Message));
        }
        finally
        {
            _uow.RollbackTransaction(tx);
        }
    }
}
