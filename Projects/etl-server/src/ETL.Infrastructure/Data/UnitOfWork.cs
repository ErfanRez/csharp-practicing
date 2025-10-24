using System.Data;
using System.Data.Common;
using ETL.Application.Abstractions.Data;
using ETL.Infrastructure.Data.Abstractions;

namespace ETL.Infrastructure.Data;

public sealed class UnitOfWork : IUnitOfWork
{
    private readonly IDbConnectionFactory _connectionFactory;

    public UnitOfWork(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
    }
    public IDbTransaction BeginTransaction()
    {
        var conn = _connectionFactory.CreateConnection();

        if (conn.State != ConnectionState.Open)
        {
            if (conn is DbConnection dbConn)
                dbConn.Open();
            else
                conn.Open();
        }

        return conn.BeginTransaction();
    }

    public void CommitTransaction(IDbTransaction transaction)
    {
        if (transaction == null) return;

        try
        {
            transaction.Commit();
        }
        finally
        {
            var conn = transaction.Connection;
            transaction?.Dispose();
            conn?.Dispose();
        }
    }

    public void RollbackTransaction(IDbTransaction transaction)
    {
        if (transaction == null) return;

        try
        {
            transaction.Rollback();
        }
        finally
        {
            var conn = transaction.Connection;
            transaction?.Dispose();
            conn?.Dispose();
        }
    }
}
