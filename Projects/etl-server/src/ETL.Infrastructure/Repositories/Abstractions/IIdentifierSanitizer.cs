namespace ETL.Infrastructure.Repositories.Abstractions;

public interface IIdentifierSanitizer
{
    string Sanitize(string identifier);
}

