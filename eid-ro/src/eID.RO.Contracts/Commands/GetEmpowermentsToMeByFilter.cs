﻿using eID.RO.Contracts.Enums;
using MassTransit;

namespace eID.RO.Contracts.Commands;

public interface GetEmpowermentsToMeByFilter : CorrelatedBy<Guid>
{
    public EmpowermentsToMeFilterStatus? Status { get; set; }
    public string Authorizer { get; set; }
    public string SupplierName { get; set; }
    public string ServiceName { get; set; }
    public DateTime? ValidToDate { get; set; }
    public bool? ShowOnlyNoExpiryDate { get; set; }
    public EmpowermentsToMeSortBy? SortBy { get; set; }
    public SortDirection SortDirection { get; set; }
    public string Uid { get; set; }
    public IdentifierType UidType { get; set; }

    public int PageSize { get; set; }
    public int PageIndex { get; set; }
}
