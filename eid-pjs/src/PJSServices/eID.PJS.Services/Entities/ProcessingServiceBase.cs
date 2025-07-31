using eID.PJS.Services.Archiving;
using eID.PJS.Services.Verification;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;

namespace eID.PJS.Services.OpenSearch
{
    public abstract class ProcessingServiceBase<TResult> : IProcessingService<TResult>
    {
        public abstract TResult? Process();

        public void SaveState(TResult state, string serviceName, StatefulServiceSettingsBase settings)
        {
            if (string.IsNullOrWhiteSpace(settings.StatePath))
                throw new ArgumentException(nameof(settings));

            var jsonSettings = new JsonSerializerSettings();
            jsonSettings.Converters.Add(new StringEnumConverter());
            
            var jsonState = JsonConvert.SerializeObject(state, Formatting.Indented, jsonSettings);

            string? savePath = Path.GetDirectoryName(settings.StatePath);

            if (!string.IsNullOrWhiteSpace(savePath) && !Directory.Exists(savePath))
                Directory.CreateDirectory(savePath);

            string fileName = Path.Combine(Path.GetFullPath(settings.StatePath), $"{serviceName.ToLower()}-svc.json");

            if (settings.KeepStateHistory)
                fileName = IOExtensions.GetRollingFileNameFromPath(Path.Combine(settings.StatePath, $"{serviceName.ToLower()}-svc.json"));

            File.WriteAllText(Path.GetFullPath(fileName), jsonState);
        }




    }
}
