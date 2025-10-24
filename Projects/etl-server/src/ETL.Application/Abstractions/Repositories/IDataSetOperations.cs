using System.Data;
using ETL.Domain.Entities;

namespace ETL.Application.Abstractions.Repositories;

public interface IGetAllDataSets
{
    Task<IEnumerable<DataSetMetadata>> ExecuteAsync(CancellationToken cancellationToken = default);
}

public interface IGetDataSetById
{
    Task<DataSetMetadata?> ExecuteAsync(Guid id, CancellationToken cancellationToken = default);
}

public interface IGetDataSetByTableName
{
    Task<DataSetMetadata?> ExecuteAsync(string tableName, CancellationToken cancellationToken = default);
}

public interface IAddDataSet
{
    Task ExecuteAsync(DataSetMetadata dataSet, IDbTransaction? tx = null, CancellationToken cancellationToken = default);
}

public interface IUpdateDataSet
{
    Task ExecuteAsync(DataSetMetadata dataSet, IDbTransaction? tx = null, CancellationToken cancellationToken = default);
}

public interface IDeleteDataSet
{
    Task ExecuteAsync(DataSetMetadata dataSet, IDbTransaction? tx = null, CancellationToken cancellationToken = default);
}
