using eID.PDEAU.Contracts.Commands;
using eID.PDEAU.Contracts.Enums;

namespace eID.PDEAU.Service;

internal static class CacheKeyHelper
{
    private const string BaseKey = "eID:PDEAU:";

    public const string ProvidersInfoBase = BaseKey + "ProvidersInfo:";

    private const string ProviderOfficesBase = BaseKey + "ProviderOffices:";
    private const string ProviderServicesBase = BaseKey + "ProviderServices:";

    public const string OpenDataActiveProvidersData = BaseKey + "OpenDataActiveProviders";

    public static string GetProvidersInfo(int pageIndex, int pageSize, string name, string identificationNumber, 
        string bulstat, GetProvidersInfoSortBy sortBy, SortDirection sortDirection)
    {
        var hash = HashCode.Combine(pageIndex, pageSize, name, identificationNumber, bulstat, sortBy, sortDirection);
        return $"{ProvidersInfoBase}{hash}";
    }

    public static string GetProviderOffices(Guid id)
        =>$"{ProviderOfficesBase}{id}";

    public static string GetProviderServices(Guid id)
        => $"{ProviderServicesBase}{id}";
}
