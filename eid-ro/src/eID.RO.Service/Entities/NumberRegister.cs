namespace eID.RO.Service.Entities;

public class NumberRegister
{
    public const string EmpowermentNumberId = "EMPSTAT";
    public const string TableName = "EmpowermentStatements.NumbersRegister";
    public string Id { get; set; } = string.Empty;
    public int Current { get; set; }
    public DateOnly LastChange { get; set; }
}
