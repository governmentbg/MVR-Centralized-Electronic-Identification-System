using System.ComponentModel.Composition;
using TechnoLogica.RegiX.Common;
using TechnoLogica.RegiX.Common.TransportObjects;
using System.Linq;
using System.Reflection;
using TechnoLogica.RegiX.Adapters.Common.Attributes;
using System.IO;
using System;
using TechnoLogica.RegiX.Adapters.Common.Utils;
//using log4net;
using System.ComponentModel.Composition.Hosting;

namespace TechnoLogica.RegiX.Adapters.Common
{
    /// <summary>
    /// Базов клас за всички API услуги. Имплементира прозрачно извикване на адаптер (получената заявка се препредава към съответния адаптер както е получена)
    /// </summary>
    public abstract class BaseAPIService<IAPI> : IAPIService
        where IAPI: IAPIService
    {
        //private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        [Import]
        public CompositionContainer CompositionContainer { get; set; }

        /// <summary>
        /// Връща IAdapterClient имплементация
        /// </summary>
        public IAdapterClient AdapterClient 
        {
            get
            {
                return CompositionContainer.GetExportedValue<IAdapterClient<IAPI>>();
            }
            set 
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Изпълнява заявката
        /// </summary>
        /// <param name="request">Аргумент за изпълнение на заявка</param>
        /// <returns>Резултат от изпълнението заявката</returns>
        public ServiceResultData Execute(ServiceRequestData request)
        {
            if (AdapterClient != null)
            {
                return AdapterClient.Execute(this, request);
            }
            else
            {
                return new ServiceResultData() { HasError = true, Error = "AdapterClient is null" };
            }
        }

        /// <summary>
        /// Проверка на резултата от изпълнението на асинхронни заявки
        /// </summary>
        /// <param name="checkResult">Аргумент съдържащ идентификатор на асинхронната операция</param>
        /// <returns>Резултат от изпълнението на асинхронната заявка</returns>
        public ServiceResultData CheckResult(ServiceCheckResultArgument checkResult)
        {
            return AdapterClient.CheckResult(this, checkResult);
        }

        /// <summary>
        /// Acknowledges that the result of an operation is received
        /// </summary>
        /// <param name="checkResult">Argument containing the service call id and verification code</returns>
        public void AcknowledgeResultReceived(ServiceCheckResultArgument checkResult)
        {
            AdapterClient.AcknowledgeResultReceived(this, checkResult);
        }

        [Import]
        public IBinDirectoryLocator BinDirectoryLocator { get; set; }
        
        protected void GetOperationAndAssemblyName(string operationName, out MethodInfo operation, out string assemblyName)
        {
            operation = GetType().GetAPIServiceInterface().GetMethods().FirstOrDefault(mi => mi.Name == operationName);
            assemblyName = GetType().Assembly.GetName().Name;
        }

        protected string[] GetFile(string operationName, string directory, string defaultName, Func<InfoAttribute, string> customName, string innerDirectory = null)
        {
            GetOperationAndAssemblyName(operationName, out MethodInfo operation, out string assemblyName);
            if (operation != null)
            {
                string fileName = defaultName;
                InfoAttribute info = (InfoAttribute)operation.GetCustomAttributes().FirstOrDefault(a => a is InfoAttribute);
                if (info != null && !string.IsNullOrEmpty(customName.Invoke(info)))
                {
                    fileName = customName.Invoke(info);
                }
                string[] result = 
                    fileName
                        .Split(';')
                        .Where(
                            file => 
                            {
                                string fullPathName = GetFullFileName(directory, file, assemblyName, innerDirectory);
                                //log.Debug($"Searching for file:{fullPathName}");
                                bool fileExists = File.Exists(fullPathName);
                                //log.Debug($"File exists: {fileExists}");
                                return fileExists;
                            }
                        ).Select(
                            file =>
                            {
                                string fullPathName = GetFullFileName(directory, file, assemblyName,innerDirectory);
                                return File.ReadAllText(fullPathName);
                            }
                        ).ToArray();
                return result;
            }
            else
            {
                throw new ApplicationException(string.Format("Operation name not found: {0}", operationName));
            }
        }

        private string GetFullFileName(string directory, string file, string assemblyName, string innerDirectory)
        {
            string fullPathName;
            if (innerDirectory != null)
            {
                fullPathName =
                    BinDirectoryLocator.GetBinDirectory() +
                    Path.DirectorySeparatorChar +
                    directory +
                    Path.DirectorySeparatorChar +
                    assemblyName +
                    Path.DirectorySeparatorChar +
                    innerDirectory +
                    Path.DirectorySeparatorChar +
                    file;
            }
            else
            {
                fullPathName =
                    BinDirectoryLocator.GetBinDirectory() +
                    Path.DirectorySeparatorChar +
                    directory +
                    Path.DirectorySeparatorChar +
                    assemblyName +
                    Path.DirectorySeparatorChar +
                    file;
            }
            
            return fullPathName;
        }

        public string GetRequestXSD(string operationName)
        {
            return GetFile(operationName, "XMLSchemas", operationName + "Request.xsd", (info) => info.RequestXSD).FirstOrDefault();
        }

        public string GetResponseXSD(string operationName)
        {
            return GetFile(operationName, "XMLSchemas", operationName + "Response.xsd", (info) => info.ResponseXSD).FirstOrDefault();
        }

        public string[] GetCommonXSD(string operationName)
        {
            return GetFile(operationName, "XMLSchemas", operationName + "Common.xsd", (info) => info.CommonXSD);
        }

        public virtual string GetMetaDataXML(string operationName)
        {
            return GetFile(operationName, "XMLMetaData", operationName + ".xml", (info) => info.MetaDataXML).FirstOrDefault();
        }

        public byte[] GetXSD(string operationName)
        {
            throw new NotImplementedException();
        }

        public string GetSampleRequest(string operationName)
        {
            return GetFile(operationName, "XMLSamples", operationName + "Request.xml", (info) => info.SampleRequest).FirstOrDefault();
        }

        public string GetSampleResponse(string operationName)
        {
            return GetFile(operationName, "XMLSamples", operationName + "Response.xml", (info) => info.SampleResponse).FirstOrDefault();
        }

        public string GetRequestXslt(string operationName)
        {
            return GetFile(operationName, "XMLSchemas", operationName + "Request.xslt", (info) => info.RequestXSLT, "Transformations").FirstOrDefault();
        }

        public string GetResponseXslt(string operationName)
        {
            return GetFile(operationName, "XMLSchemas", operationName + "Response.xslt", (info) => info.ResponseXSLT, "Transformations").FirstOrDefault();
        }

        public string GetRequestXsltFOP(string operationName)
        {
            return GetFile(operationName, "XMLSchemas", operationName + "RequestFOP.xslt", (info) => info.RequestXSLTFOP, "Transformations").FirstOrDefault();
        }

        public string GetResponseXsltFOP(string operationName)
        {
            return GetFile(operationName, "XMLSchemas", operationName + "ResponseFOP.xslt", (info) => info.ResponseXSLTFOP, "Transformations").FirstOrDefault();
        }

        public byte[] GetSamples(string[] operationName)
        {
            throw new NotImplementedException();
        }
    }
}
