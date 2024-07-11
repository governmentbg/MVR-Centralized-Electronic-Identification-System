namespace eID.RO.Service.Options;

public class ApplicationOptions
{
    public int CollectResolutionsDeadlinePeriodInDays { get; set; } = 7;
    public void Validate()
    {
        if (CollectResolutionsDeadlinePeriodInDays <= 0)
        {
            throw new ArgumentException("Must be greater 0", nameof(CollectResolutionsDeadlinePeriodInDays));
        }
    }
}
