using System;
using System.Linq;
using TechnoLogica.RegiX.Adapters.Common;
using TechnoLogica.RegiX.Adapters.Common.Utils;
using TechnoLogica.RegiX.Common;

namespace TechnoLogica.RegiX.WebServiceAdapterService
{
    public class SoapServiceBaseAdapterService : BaseAdapterService
    {
        public override string CheckRegisterConnection()
        {
            try
            {
                var webServiceUrl = this.GetParameters().ParameterList.Where(p => p.Key == Constants.WebServiceUrlParameterKeyName).FirstOrDefault();
                if (webServiceUrl != null && !String.IsNullOrEmpty(webServiceUrl.CurrentValue))
                {
                    return CheckConnectionsUtils.CheckSoapConnection(webServiceUrl.CurrentValue);
                }
                else
                {
                    return Constants.WebServiceUrlNotSet;
                }
            }
            catch (Exception ex)
            {
                return String.Format(Constants.ConnectionException, ex.Message);
            }
        }
    }
}
