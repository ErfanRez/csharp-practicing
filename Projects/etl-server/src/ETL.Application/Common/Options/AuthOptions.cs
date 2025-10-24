namespace ETL.Application.Common.Options;
public class AuthOptions
{
    public const string SectionName = "Authentication";


    public string Authority { get; init; } = string.Empty;
    public string ClientId { get; init; } = string.Empty;
    public string ClientSecret { get; set; } = string.Empty;
    public string RedirectUri { get; set; } = string.Empty;
    public string BaseUrl { get; set; } = string.Empty;
    public string Realm { get; set; } = string.Empty;
}
