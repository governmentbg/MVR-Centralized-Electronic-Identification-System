using eID.RO.Contracts.Enums;
using eID.RO.Contracts.Results;
using MassTransit;

namespace eID.RO.Contracts.Commands;

public interface GetEmpowermentsFromMeByFilter : CorrelatedBy<Guid>
{
    public string Number { get; set; }
    public EmpowermentsFromMeFilterStatus? Status { get; set; }
    public string Authorizer { get; set; }
    public string ProviderName { get; set; }
    public string ServiceName { get; set; }
    public DateTime? ValidToDate { get; set; }
    public bool? ShowOnlyNoExpiryDate { get; set; }
    public EmpowermentsFromMeSortBy? SortBy { get; set; }
    public SortDirection SortDirection { get; set; }
    public OnBehalfOf? OnBehalfOf { get; set; }
    public List<UserIdentifierData> EmpoweredUids { get; set; }
    public string Uid { get; set; }
    public IdentifierType UidType { get; set; }
    public string Eik { get; set; }

    public int PageSize { get; set; }
    public int PageIndex { get; set; }
}
