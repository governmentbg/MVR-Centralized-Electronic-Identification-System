namespace eID.PDEAU.API.Public.Admin.Authorization;

/// <summary>
/// This attribute is used to cancel the effect of <see cref="RoleAuthorizationAttribute"/> forbiddenRoles
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
public class AllowForbiddenRolesAttribute : Attribute
{
    public string[] AllowedRoles { get; }

    public AllowForbiddenRolesAttribute(string allowedRoles)
    {
        AllowedRoles = allowedRoles.Split(",", options: StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
    }
}
