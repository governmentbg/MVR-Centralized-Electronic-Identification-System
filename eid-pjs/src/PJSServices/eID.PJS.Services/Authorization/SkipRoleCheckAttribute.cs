namespace eID.PJS.Services.Authorization;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
public class SkipRoleCheckAttribute : Attribute
{
}
