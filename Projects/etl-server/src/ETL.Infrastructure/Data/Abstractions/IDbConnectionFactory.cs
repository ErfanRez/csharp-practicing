using System.Data;

namespace ETL.Infrastructure.Data.Abstractions;

public interface IDbConnectionFactory
{
    IDbConnection CreateConnection();
}
