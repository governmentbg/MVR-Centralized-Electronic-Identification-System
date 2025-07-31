using System;
using System.Linq;
using System.Net;
using TechnoLogica.RegiX.Common;
using TechnoLogica.RegiX.Adapters.Common;

namespace TechnoLogica.RegiX.WebServiceAdapterService
{
    public class WebServiceBaseAdapterService : BaseAdapterService
    {
        //[Export(typeof(ParameterInfo))]
        //public static ParameterInfo<string> WebServiceUrl { get; set; }

        public override string CheckRegisterConnection()
        {
            try
            {
                var webServiceUrl = this.GetParameters().ParameterList.Where(p => p.Key == Constants.WebServiceUrlParameterKeyName).FirstOrDefault();
                if (webServiceUrl != null && !String.IsNullOrEmpty(webServiceUrl.CurrentValue))
                {
                    var myRequest = (HttpWebRequest)WebRequest.Create(webServiceUrl.CurrentValue);

                    var response = (HttpWebResponse)myRequest.GetResponse();

                    //if (response.StatusCode == HttpStatusCode.OK)
                    //{
                    //  it's at least in some way responsive
                    //  but may be internally broken
                    //  as you could find out if you called one of the methods for real
                    return Constants.ConnectionOk;
                    //}
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
