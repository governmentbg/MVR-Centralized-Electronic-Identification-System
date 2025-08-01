namespace eID.PDEAU.API.Public.Admin.Authorization;

internal static class UserRoles
{
    public const string Delimiter = ",";
    
    /// <summary>
    /// Specific external system administrator.
    /// </summary>
    public const string ExternalSystemAdministrator = "EID_External_System_Admin";

    /// <summary>
    /// Global MoI application administrator. Own full access.
    /// </summary>
    public const string ApplicationAdministrator = "rEID_App_Administrator";
}
