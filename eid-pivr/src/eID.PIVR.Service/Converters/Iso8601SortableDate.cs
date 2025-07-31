using Newtonsoft.Json.Converters;

namespace eID.PIVR.Service.Converters
{
    class Iso8601SortableDate : IsoDateTimeConverter
    {
        public Iso8601SortableDate()
        {
            DateTimeFormat = "s";
        }
    }
}
