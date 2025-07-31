namespace eID.PDEAU.Service.Entities;

public class NumberRegister
{
    public const string RegistrationNumberId = "REGNOID";
    public const string TableName = "Providers.NumbersRegister";
    public string Id { get; set; } = string.Empty;
    public int Current { get; set; }
    public DateOnly LastChange { get; set; }
}
