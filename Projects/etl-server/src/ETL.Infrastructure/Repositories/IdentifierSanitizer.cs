using System.Text.RegularExpressions;
using ETL.Infrastructure.Repositories.Abstractions;

namespace ETL.Infrastructure.Repositories;

public sealed class IdentifierSanitizer : IIdentifierSanitizer
{
    public string Sanitize(string identifier)
    {
        if (string.IsNullOrWhiteSpace(identifier)) throw new ArgumentException("Identifier cannot be empty.", nameof(identifier));

        var sanitized = Regex.Replace(identifier, @"[^\w]", "");
        if (sanitized.Length > 63) sanitized = sanitized.Substring(0, 63);
        if (string.IsNullOrWhiteSpace(sanitized)) throw new ArgumentException("Invalid identifier format.", nameof(identifier));
        return $"\"{sanitized}\"";
    }
}

