namespace eID.RO.Contracts;

public enum LogEventCode
{
    None = 0,
    CreateEmpowerment = 1,
    GetEmpowermentsToMeByFilter = 2,
    GetEmpowermentsFromMeByFilter = 3,
    WithdrawEmpowerment = 4,
    GetEmpowermentWithdrawReasons = 5,
    GetEmpowermentDisagreementReasons = 6,
    DisagreeEmpowerment = 7,
    GetEmpowermentsByDeau = 8,
    SignEmpowerment = 9,
    GetBatchesByFilter = 10,
    GetServicesByFilter = 11,
    GetAllServiceScopes = 12,
    GetEmpowermentsByEik = 13,
    DenyEmpowermentByDeau = 14,
    ApproveEmpowermentByDeau = 15
}
