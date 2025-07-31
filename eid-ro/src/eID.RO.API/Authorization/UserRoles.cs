namespace eID.RO.API.Authorization;

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
    /// Роля Администратор на АЕИ/ЦЕИ/ДЕАУ в МВР BACK OFFICE - КИС
    /// </summary>
    public const string CISAdmin = "rEID_CIS_Back_Office";

    /// <summary>
    /// Combine <see cref="AppAdmin"/> and <see cref="RUAdmin"/> roles.
    /// </summary>
    public const string AppRUAndCISAdmins = AppAdmin + Delimiter + RUAdmin + Delimiter + CISAdmin;
}
