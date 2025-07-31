namespace eID.MIS.API.Authorization;

internal static class UserRoles
{
    private const string Delimiter = ",";

    /// <summary>
    /// Global MoI application administrator. Own full access.
    /// </summary>
    public const string AppAdmin = "rEID_App_Administrator";

    /// <summary>
    /// Администратор в РУ/ОДМВР в МВР администратор на МВР
    /// </summary>
    public const string RUAdmin = "rEID_RU_MVR_Administrator";

    /// <summary>
    /// Combine <see cref="AppAdmin"/> and <see cref="RUAdmin"/> roles.
    /// </summary>
    public const string AllAdmins = AppAdmin + Delimiter + RUAdmin;
}
