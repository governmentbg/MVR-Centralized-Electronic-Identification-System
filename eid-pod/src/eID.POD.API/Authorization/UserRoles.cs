namespace eID.POD.API.Authorization;

/// <summary>
/// User role descriptions
/// </summary>
internal static class UserRoles
{
    private const string Delimiter = ",";

    /// <summary>
    /// Global application administrator. Own full access.
    /// </summary>
    public const string AppAdmin = "rEID_App_Administrator";

    /// <summary>
    /// Роля Системен Администратор 2 МВР System Administrator - ДКИС
    /// </summary>
    public const string DCISAdmin = "rEID_DCIS_App_Administrator";

    /// <summary>
    /// Combine "rEID_App_Administrator" and "rEID_DCIS_App_Administrator" roles.
    /// </summary>
    public const string AllAdmins = AppAdmin + Delimiter + DCISAdmin;
}
