namespace eID.Signing.API.Authorization;

internal static class UserRoles
{
    private const string Delimiter = ",";

    /// <summary>
    /// Global MoI application administrator. Own full access.
    /// </summary>
    public const string AppAdmin = "rEID_App_Administrator";
}
