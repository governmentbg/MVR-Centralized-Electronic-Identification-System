using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eID.PJS.Services
{
    public interface IProcessingService<TResult>
    {
        TResult? Process();
        void SaveState(TResult state, string serviceName, StatefulServiceSettingsBase settings);
    }
}
