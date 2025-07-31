using Newtonsoft.Json.Converters;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;


namespace eID.PJS.Services
{
    public class ServiceStatusBase<TState> where TState : class, IPerformanceMetrics
    {
        public string? ServiceName { get; set; }
        public DateTime? LastProcessingStart { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public WorkingStatus CurrentStatus { get; set; }
        public TState? LastState { get; set; }
        public Dictionary<string, string> Properties { get; set; } = new Dictionary<string, string>();

    }
}
