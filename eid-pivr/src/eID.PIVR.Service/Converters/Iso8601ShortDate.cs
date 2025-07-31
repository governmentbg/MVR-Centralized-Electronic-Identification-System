using Newtonsoft.Json.Converters;

namespace eID.PIVR.Service.Converters
{
    class Iso8601ShortDate : IsoDateTimeConverter
    {
        public Iso8601ShortDate()
        {
            DateTimeFormat = "yyyy-MM-dd";
        }
    }
}
