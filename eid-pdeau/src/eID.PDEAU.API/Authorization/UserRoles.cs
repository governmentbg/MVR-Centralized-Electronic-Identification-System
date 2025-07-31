namespace eID.PDEAU.API.Authorization;

internal static class UserRoles
{
    private const string Delimiter = ",";

    public const string PLSAdministrator = "rEID_PDEAU_PLE";

    /// <summary>
    /// Global MoI application administrator. Own full access.
    /// </summary>
    public const string AppAdmin = "rEID_App_Administrator";

    /// <summary>
    /// Роля Администратор на АЕИ/ЦЕИ/ДЕАУ в МВР BACK OFFICE - КИС
    /// </summary>
    public const string CISAdmin = "rEID_CIS_Back_Office";

    /// <summary>
    /// Combine <see cref="AppAdmin"/> and <see cref="CISAdmin"/> roles.
    /// </summary>
    public const string AppAndCisAdmins = AppAdmin + Delimiter + CISAdmin;
}
