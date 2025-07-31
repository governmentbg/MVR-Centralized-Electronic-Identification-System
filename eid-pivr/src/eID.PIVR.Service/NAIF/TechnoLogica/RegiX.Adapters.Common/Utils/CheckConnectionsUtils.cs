using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TechnoLogica.RegiX.Common;

namespace TechnoLogica.RegiX.Adapters.Common.Utils
{
    public class CheckConnectionsUtils
    {
        private static async Task<string> CheckConnectionStatus(string connectionStringUrl)
        {
            var serviceClient = new HttpClient();
            HttpResponseMessage response = await serviceClient.GetAsync(connectionStringUrl);

            if (response.IsSuccessStatusCode || response.StatusCode == HttpStatusCode.MethodNotAllowed)
            {
                return Constants.ConnectionOk;
            }
            else
            {
                string content = await response.Content.ReadAsStringAsync();
                throw new Exception("StatusCode: " + response.StatusCode + "; Content: " + content);
            }
        }

        public static string CheckConnection(string connectionStringUrl)
        {
            try
            {
                if (!String.IsNullOrEmpty(connectionStringUrl))
                {
                    return CheckConnectionStatus(connectionStringUrl).Result;
                }
                else
                {
                    return Constants.WebServiceUrlNotSet;
                }
            }
            catch (Exception ex)
            {
                return string.Format(Constants.ConnectionException, ex.Message);
            }
        }

        public static string CheckSoapConnection(string connectionStringUrl)
        {
            try
            {
                if (!String.IsNullOrEmpty(connectionStringUrl))
                {
                    ServicePointManager.ServerCertificateValidationCallback += delegate
                    {
                        return true;
                    };
                    var myRequest = (HttpWebRequest)WebRequest.Create(connectionStringUrl + "?wsdl");
                    var response = (HttpWebResponse)myRequest.GetResponse();
                    return Constants.ConnectionOk;
                }
                else
                {
                    return Constants.WebServiceUrlNotSet;
                }
            }
            catch (Exception ex)
            {
                return string.Format(Constants.ConnectionException, ex.Message);
            }
        }
    }
}
