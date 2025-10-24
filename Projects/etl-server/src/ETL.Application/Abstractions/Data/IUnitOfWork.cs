using System.Data;

namespace ETL.Application.Abstractions.Data;

public interface IUnitOfWork
{
    IDbTransaction BeginTransaction();

    void CommitTransaction(IDbTransaction transaction);

    void RollbackTransaction(IDbTransaction transaction);
}
