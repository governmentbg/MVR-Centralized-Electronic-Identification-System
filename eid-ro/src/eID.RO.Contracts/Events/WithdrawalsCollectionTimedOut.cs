﻿using MassTransit;

namespace eID.RO.Contracts.Events;

public interface WithdrawalsCollectionTimedOut : CorrelatedBy<Guid>
{
    Guid EmpowermentId { get; }
}
