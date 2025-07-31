namespace eID.PAN.API.Authorization;

internal static class UserRoles
{
    private const string Delimiter = ",";

    /// <summary>
    /// Global MoI application administrator. Own full access.
    /// </summary>
    public const string AppAdmin = "rEID_App_Administrator";

    /// <summary>
    /// Системен Администратор 2 МВР System Administrator - ДКИС
    /// </summary>
    public const string DCISAdmin = "rEID_DCIS_App_Administrator";

    /// <summary>
    /// Combine <see cref="AppAdmin"/> and <see cref="DCISAdmin"/> roles.
    /// </summary>
    public const string AppAndDcisAdmins = AppAdmin + Delimiter + DCISAdmin;
}
