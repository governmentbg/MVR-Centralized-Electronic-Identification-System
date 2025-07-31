namespace eID.MIS.API.Options;

public class ExchangeRatesOptions
{
    public decimal EUR_TO_BGN { get; set; }
    public decimal BGN_TO_EUR { get; set; }
    public void Validate()
    {
        if (EUR_TO_BGN <= 0)
        {
            throw new ArgumentException("Invalid EUR_TO_BGN", nameof(EUR_TO_BGN));
        }
        if (BGN_TO_EUR <= 0)
        {
            throw new ArgumentException("Invalid BGN_TO_EUR", nameof(BGN_TO_EUR));
        }
    }
}

