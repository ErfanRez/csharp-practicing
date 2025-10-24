using System.Data;
using ETL.Application.Abstractions.Data;
using ETL.Application.Abstractions.Repositories;
using ETL.Application.Common;
using MediatR;

namespace ETL.Application.DataSet.DeleteColumn;

public record DeleteColumnCommand(string TableName, string ColumnName) : IRequest<Result>;

public sealed class DeleteColumnCommandHandler : IRequestHandler<DeleteColumnCommand, Result>
{
    private readonly IUnitOfWork _uow;
    private readonly IGetDataSetByTableName _getByTableName;
    private readonly IStagingColumnExists _columnExists;
    private readonly IDeleteStagingColumn _deleteStagingColumn;

    public DeleteColumnCommandHandler(
        IUnitOfWork uow,
        IGetDataSetByTableName getByTableName,
        IStagingColumnExists columnExists,
        IDeleteStagingColumn deleteStagingColumn)
    {
        _uow = uow ?? throw new ArgumentNullException(nameof(uow));
        _getByTableName = getByTableName ?? throw new ArgumentNullException(nameof(getByTableName));
        _columnExists = columnExists ?? throw new ArgumentNullException(nameof(columnExists));
        _deleteStagingColumn = deleteStagingColumn ?? throw new ArgumentNullException(nameof(deleteStagingColumn));
    }

    public async Task<Result> Handle(DeleteColumnCommand request, CancellationToken cancellationToken)
    {
        var existing = await _getByTableName.ExecuteAsync(request.TableName, cancellationToken);
        if (existing == null)
        {
            return Result.Failure(
                Error.NotFound("ColumnDelete.Failed", $"Table '{request.TableName}' not found!"));
        }

        var columnExist = await _columnExists.ExecuteAsync(request.TableName, request.ColumnName, cancellationToken);
        if (!columnExist)
        {
            return Result.Failure(
                Error.NotFound("ColumnDelete.Failed", $"Column '{request.ColumnName}' not found!"));
        }

        IDbTransaction? tx = null;
        try
        {
            tx = _uow.BeginTransaction();

            await _deleteStagingColumn.ExecuteAsync(request.TableName, request.ColumnName, tx, cancellationToken);

            _uow.CommitTransaction(tx);
            tx = null;

            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure(Error.Problem("ColumnDelete.Failed", ex.Message));
        }
        finally
        {
            _uow.RollbackTransaction(tx);
        }
    }
}
