using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#nullable disable

namespace eID.PJS.Services
{
    public class StatefulServiceSettingsBase
    {
        public string StatePath { get; set; }
        public bool KeepStateHistory { get; set; }
    }
}
