#nullable disable

namespace eID.PDEAU.Service.Entities;

/// <summary>
/// Here we will store predefined values of service scope names
/// </summary>
public class DefaultServiceScope
{
    public Guid Id { get; set; }
    private string _name = string.Empty;
    public string Name
    {
        get { return _name; }
        set
        {
            if (value.Length > DBConstraints.DefaultServiceScope.NameMaxLength)
            {
                _name = value[..DBConstraints.DefaultServiceScope.NameMaxLength];
            }
            else
            {
                _name = value;
            }
        }
    }
}
#nullable restore
