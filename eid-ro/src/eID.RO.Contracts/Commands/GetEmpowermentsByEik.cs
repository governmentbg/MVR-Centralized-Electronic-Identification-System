using eID.RO.Contracts.Enums;
using eID.RO.Contracts.Results;
using MassTransit;

namespace eID.RO.Contracts.Commands;

public interface GetEmpowermentsByEik : CorrelatedBy<Guid>
{
    public string Eik { get; set; }
    public string IssuerUid { get; set; }
    public IdentifierType IssuerUidType { get; set; }
    public string IssuerName { get; set; }
    public EmpowermentsByEikFilterStatus? Status { get; set; }
    public string SupplierName { get; set; }
    public string ServiceName { get; set; }
    public DateTime? ValidToDate { get; set; }
    public bool? ShowOnlyNoExpiryDate { get; set; }
    public List<UserIdentifierData> AuthorizerUids { get; set; }
    public EmpowermentsByEikSortBy? SortBy { get; set; }
    public SortDirection SortDirection { get; set; }

    public int PageSize { get; set; }
    public int PageIndex { get; set; }
}
