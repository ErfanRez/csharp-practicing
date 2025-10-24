namespace ETL.Application.Common.Options;
public class OAuthAdminOptions
{
    public const string SectionName = "OAuthAdmin";


    public string ClientId { get; init; } = string.Empty;
    public string ClientSecret { get; init; } = string.Empty;
}