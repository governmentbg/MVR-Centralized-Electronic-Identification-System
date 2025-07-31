using eID.RO.Contracts.Enums;
using MassTransit;

namespace eID.RO.Contracts.Commands;

public interface GetEmpowermentsByDeau : CorrelatedBy<Guid>
{
    public OnBehalfOf OnBehalfOf { get; set; }
    public string AuthorizerUid { get; set; }
    public IdentifierType AuthorizerUidType { get; set; }
    public string EmpoweredUid { get; set; }
    public IdentifierType EmpoweredUidType { get; set; }
    public string ProviderId { get; set; }
    public string RequesterUid { get; set; }
    public int ServiceId { get; set; }
    /// <summary>
    /// Holds a pre-defined valid values of VolumeOfRepresentation codes. If passed it will filter Empowerments by Volume of Representation.
    /// </summary>
    public List<string> VolumeOfRepresentation { get; set; }
    public DateTime StatusOn { get; set; }
    public int PageSize { get; set; }
    public int PageIndex { get; set; }
    public EmpowermentsByDeauSortBy? SortBy { get; set; }
    public SortDirection SortDirection { get; set; }
}
