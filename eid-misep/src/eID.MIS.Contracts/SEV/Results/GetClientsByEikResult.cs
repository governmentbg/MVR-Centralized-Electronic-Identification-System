namespace eID.MIS.Contracts.SEV.Results;

public class GetClientsByEikDTO : GetClientsByEikResult
{
    public Department Department { get; set; }
    public List<Unite> Unites { get; set; }
}
public interface GetClientsByEikResult
{
    public Department Department { get; set; }
    public List<Unite> Unites { get; set; }
}

public class Department
{
    public string Type { get; set; }
    public Uid Uid { get; set; }
    public string Name { get; set; }
}

public class Uid
{
    public string Type { get; set; }
    public string Value { get; set; }
}

public class Unite
{
    public string Type { get; set; }
    public Uid Uid { get; set; }
    public string Name { get; set; }
    public Info Info { get; set; }
    public bool IsActive { get; set; }
}

public class Info
{
    public BankAccount BankAccount { get; set; }
}

public class BankAccount
{
    public string Name { get; set; }
    public string Bank { get; set; }
    public string Bic { get; set; }
    public string Iban { get; set; }
}
