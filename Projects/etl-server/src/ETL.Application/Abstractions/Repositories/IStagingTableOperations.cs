using System.Data;

namespace ETL.Application.Abstractions.Repositories;

public interface IGetStagingTableByName
{
    Task<string> ExecuteAsync(string tableName, CancellationToken cancellationToken = default);
}

public interface ICreateTableFromCsv
{
    Task ExecuteAsync(string tableName, Stream csvStream, IDbTransaction? tx = null, CancellationToken cancellationToken = default);
}

public interface IRenameStagingTable
{
    Task ExecuteAsync(string oldName, string newName, IDbTransaction? tx = null, CancellationToken cancellationToken = default);
}

public interface IRenameStagingColumn
{
    Task ExecuteAsync(string tableName, string oldColumn, string newColumn, IDbTransaction? tx = null, CancellationToken cancellationToken = default);
}

public interface IDeleteStagingTable
{
    Task ExecuteAsync(string tableName, IDbTransaction? tx = null, CancellationToken cancellationToken = default);
}

public interface IDeleteStagingColumn
{
    Task ExecuteAsync(string tableName, string columnName, IDbTransaction? tx = null, CancellationToken cancellationToken = default);
}

public interface IStagingColumnExists
{
    Task<bool> ExecuteAsync(string tableName, string columnName, CancellationToken cancellationToken = default);
}
