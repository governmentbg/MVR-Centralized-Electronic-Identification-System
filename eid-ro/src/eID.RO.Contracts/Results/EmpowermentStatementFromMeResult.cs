﻿namespace eID.RO.Contracts.Results;

public interface EmpowermentStatementFromMeResult : EmpowermentStatementResult
{
    IEnumerable<EmpowermentSignatureResult> EmpowermentSignatures { get; }
}
