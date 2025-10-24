namespace ETL.Application.Common.Constants;

public static class Role
{
    public const string SystemAdmin = "system-admin";
    public const string DataAdmin = "data-admin";
    public const string Analyst = "analyst";

    public static List<string> GetAllRoles()
    {
        return
        [
            SystemAdmin,
            DataAdmin,
            Analyst
        ];
    }
}