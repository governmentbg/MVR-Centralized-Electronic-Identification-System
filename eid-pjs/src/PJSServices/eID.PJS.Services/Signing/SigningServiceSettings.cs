using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


#nullable disable

namespace eID.PJS.Services.Signing
{
    public class SigningServiceSettings : StatefulServiceSettingsBase
    {
        public List<MonitoredFolderSettings> MonitoredFolders { get; set; }

        public string AuditLogFileExtension { get; set; }
        public string HashFileExtension { get; set; }

    }
}
