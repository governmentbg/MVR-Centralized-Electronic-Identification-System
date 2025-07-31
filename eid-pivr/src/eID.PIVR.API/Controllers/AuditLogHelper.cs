using eID.PIVR.API.Requests;

namespace eID.PIVR.API.Controllers;

public static class AuditLogHelper
{
    public const string Source = "Source";
    public const string DatabaseNaif = "database, NAIF";
    public const string Naif = "http://general.service.ict.mvr.bg/, NAIF.IntSyncPortType";

    public static string BuildRegiXSource(RegiXSearchRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        return $"{request.Argument.Xmlns}, {request.Operation}";
    }
}
