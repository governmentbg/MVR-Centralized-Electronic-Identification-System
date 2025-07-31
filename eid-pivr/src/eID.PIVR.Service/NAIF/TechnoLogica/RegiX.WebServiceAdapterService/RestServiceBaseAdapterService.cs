using System;
using System.Linq;
using TechnoLogica.RegiX.Common;
using System.Data;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using TechnoLogica.RegiX.Adapters.Common.Utils;
using TechnoLogica.RegiX.Adapters.Common;

namespace TechnoLogica.RegiX.WebServiceAdapterService
{
    public class RestServiceBaseAdapterService : BaseAdapterService
    {
        protected string _serverCertThumbPrint;
        //public string ServerCertThumbPrint { get; set; }
       
        protected X509Certificate2 GetMyX509Certificate(string certThumbPrint)
        {
            if (certThumbPrint != null)
            {
                certThumbPrint = certThumbPrint.Replace(" ", "").ToUpper();
            }

            X509Certificate2 certSelected = null;
            X509Store x509Store = new X509Store(StoreName.My, StoreLocation.LocalMachine);
            x509Store.Open(OpenFlags.ReadOnly);

            X509Certificate2Collection certificateCollection = x509Store.Certificates;

            foreach (var cer in certificateCollection)
            {
                if (cer.Thumbprint.CompareTo(certThumbPrint) == 0)//cer.Thumbprint == sslThumbPrint)
                {
                    certSelected = cer;
                    break;
                }
            }

            x509Store.Close();

            return certSelected;
        }

        protected bool Verify_Certificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            if (string.IsNullOrEmpty(_serverCertThumbPrint))
            {
                return true;
            }
            string thumbPrint = _serverCertThumbPrint.Replace(" ", "").ToUpper();

            string configurationThumbPrint = certificate.GetCertHashString();
            configurationThumbPrint = configurationThumbPrint.Replace(" ", "").ToUpper();

            if (thumbPrint.CompareTo(configurationThumbPrint) == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        protected HttpClient GetClient(string webServiceUrl, string clientCertThumbPrint, string serverCertThumbPrint)
        {
            _serverCertThumbPrint = serverCertThumbPrint;
            HttpClientHandler handler = new HttpClientHandler();
            if (!string.IsNullOrEmpty(clientCertThumbPrint))
            {
                X509Certificate2 certificate = GetMyX509Certificate(clientCertThumbPrint);
                if (certificate != null)
                {
                    handler.ClientCertificates.Add(certificate);

                }
            }
            if (!string.IsNullOrEmpty(serverCertThumbPrint))
            {
                handler.ServerCertificateCustomValidationCallback += Verify_Certificate;
            }
            var serviceClient = new HttpClient(handler);
            //  System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

            serviceClient.BaseAddress = new Uri(webServiceUrl);
            serviceClient.DefaultRequestHeaders.Accept.Clear();
            serviceClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml"));
            return serviceClient;
        }

        protected async Task<string> CheckConnectionStatus(string webServiceWithMethodUrl, string webServiceUrl, string clientCertThumbPrint, string serverCertThumbPrint)
        {
            var serviceClient = GetClient(webServiceUrl: webServiceUrl, clientCertThumbPrint: clientCertThumbPrint, serverCertThumbPrint: serverCertThumbPrint);
            HttpResponseMessage response = await serviceClient.GetAsync(webServiceWithMethodUrl);

            if (response.IsSuccessStatusCode || response.StatusCode == HttpStatusCode.MethodNotAllowed)
            {
                return Constants.ConnectionOk;
            }
            else
            {
                string content = await response.Content.ReadAsStringAsync();
                return "StatusCode: " + response.StatusCode + "; Content: " + content;
            }
        }

        public override string CheckRegisterConnection()
        {
            try
            {
                var webServiceUrl = this.GetParameters().ParameterList.Where(p => p.Key == Constants.WebServiceUrlParameterKeyName).FirstOrDefault();
                if (webServiceUrl != null)
                {
                    return CheckConnectionsUtils.CheckConnection(webServiceUrl.CurrentValue);
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
