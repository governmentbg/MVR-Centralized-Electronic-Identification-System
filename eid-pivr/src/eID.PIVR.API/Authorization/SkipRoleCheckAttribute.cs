namespace eID.PIVR.API.Authorization;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
public class SkipRoleCheckAttribute : Attribute
{
}
