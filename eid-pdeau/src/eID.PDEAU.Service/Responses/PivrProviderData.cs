namespace eID.PDEAU.Service.Responses;

public class PivrProviderData
{
   public Guid Id { get; set; }
   public string IdentificationNumber { get; set; }
   public string Name { get; set; }
   public bool IsExternal { get; }
   public IISDAProviderDetailsStatus Status { get; set; }
   public string UIC { get; set; }
}

public enum IISDAProviderDetailsStatus
{
    /// <summary>
    /// Default value
    /// </summary>
    None = 0,
    /// <summary>
    /// It allows to show and synchronize data
    /// </summary>
    Active = 1
}
