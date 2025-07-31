using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;

namespace TechnoLogica.RegiX.Adapters.Common.Utils
{
    public class ConfigurationUtils
    {
        public static bool GetSignResponse()
        {
            var signResponse = ConfigurationManager.AppSettings["SignResponse"];
            if (signResponse != null)
            {
                return Boolean.Parse(signResponse);
            }
            else
            {
                return false;
            }
        }
    }
}
