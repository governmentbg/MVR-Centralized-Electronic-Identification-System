//using log4net;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.ServiceModel;
using System.Text;
using TechnoLogica.RegiX.Adapters.Common.Interfaces;
using TechnoLogica.RegiX.Adapters.Common.Utils;
using TechnoLogica.RegiX.Common;
using TechnoLogica.RegiX.Common.DataContracts;
using TechnoLogica.RegiX.Common.DataContracts.Health;
using TechnoLogica.RegiX.Common.DataContracts.Parameter;
using TechnoLogica.RegiX.Common.ObjectMapping;
using TechnoLogica.RegiX.Common.TransportObjects;
using TechnoLogica.RegiX.Common.Utils;

//[assembly: log4net.Config.XmlConfigurator(Watch = true)]

namespace TechnoLogica.RegiX.Adapters.Common
{
    /// <summary>
    /// Базов клас имплементиращ обща функционалност за всички адаптери
    /// </summary>
    public abstract class BaseAdapterService : IAdapterService, IAdapterServiceNETCore, IAdapterServiceWCF, IAsynchronousAdapterService
    {
        public static readonly string OPERATION_NAME_KEY = "OperationName";
        public static readonly string API_SERVICE_INTERFACE = "APIServiceInterface";

        /// <summary>
        /// Logger
        /// </summary>
        //private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        
        /// <summary>
        /// Функции за проверка на състоянието
        /// </summary>
        private HealthCheckFunctions HealthCheckFunctions
        {
            get
            {
                var healthInfos = new HealthCheckFunctions() { HealthInfosList = GetExportedHealthInfo() };
                return healthInfos;
            }
        }

        private List<HealthInfo> GetExportedHealthInfo()
        {
            List<HealthInfo> healthInfos = new List<HealthInfo>();
            Type currentType = GetType();
            while (currentType != typeof(BaseAdapterService))
            {
                AddExportedHealthInfo(healthInfos, currentType);
                currentType = currentType.BaseType;
            }
            AddExportedHealthInfo(healthInfos, currentType);
            return healthInfos;
        }

        private void AddExportedHealthInfo(List<HealthInfo> healthInfos, Type currentType)
        {
            healthInfos.AddRange(Composition.CompositionContainer.GetExportedValues<HealthInfo>(currentType.FullName));
        }

        /// <summary>
        /// Извлича интерфейсът на адаптера
        /// </summary>
        public Type AdapterServiceInterface
        {
            get
            {
                return GetType().GetAdapterServiceInterface();
            }
        }

        /// <summary>
        /// Извлича типа на адаптера
        /// </summary>
        public Type AdapterServiceType
        {
            get
            {
                return GetType();
            }
        }

        /// <summary>
        /// Извлича името на адаптера
        /// </summary>
        public string AdapterServiceName
        {
            get
            {
                return GetType().Name;
            }
        }

        /// <summary>
        /// Списък с всички базови класове на адаптера до BaseAdapterService
        /// </summary>
        private List<Type> AdapterServiceTypesList
        {
            get
            {
                List<Type> typesList = new List<Type>();

                Type currentType = this.GetType();
                while (currentType != typeof(BaseAdapterService))
                {
                    typesList.Add(currentType);
                    currentType = currentType.BaseType;
                }

                typesList.Add(currentType); //добавяме и BaseAdapterService
                return typesList;
            }

        }

        /// <summary>
        /// Задава стойност на параметър
        /// </summary>
        /// <param name="key">Ключ на параметър</param>
        /// <param name="value">Стойност на параметър</param>
        public void SetParameter(string key, string value)
        {
            if (ParameterStore != null)
            {
                ParameterStore.SetParameter(this, key, value);
            }
            else
            {
                throw new ApplicationException("ParameterStore is not set");
            }                
        }

        /// <summary>
        /// Ping метод
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public byte[] Ping(byte[] data)
        {
            return data;
        }

        /// <summary>
        /// Извилча параметрите
        /// </summary>
        /// <returns>Извлечените параметри</returns>
        public Parameters GetParameters()
        {
            if (ParameterStore != null)
            {
                return ParameterStore.GetParameters(this);
            }
            else
            {
                throw new ApplicationException("ParameterStore is not set");
            }
        }

        /// <summary>
        /// Извлича функциите за проверка
        /// </summary>
        /// <returns>Извлечените фунцкии за проверка</returns>
        public HealthCheckFunctions GetHealthCheckFunctions()
        {
            return CopyHealthInfos(HealthCheckFunctions);
        }
        
        /// <summary>
        /// Копира функции за порверка на състоянието
        /// </summary>
        /// <param name="checkFunctions">Дефнииции на функции за проверка на състоянието</param>
        /// <returns>Копираните функции за проверка на състоянието</returns>
        private HealthCheckFunctions CopyHealthInfos(HealthCheckFunctions checkFunctions)
        {
            return
                new HealthCheckFunctions()
                {
                    HealthInfosList =
                      checkFunctions
                      .HealthInfosList
                      .Select(pi => new HealthInfo() { Name = pi.Name, Description = pi.Description, Key = pi.Key })
                      .ToList()
                };
        }

        /// <summary>
        /// Проверява състоянието чрез функция отговаряща на подадения ключ
        /// </summary>
        /// <param name="key">Ключ на функция за проверка на сътоянието</param>
        /// <returns>Резлутат от изпълнението на функцията</returns>
        public string CheckHealthFunction(string key)
        {
            var healthFunction = HealthCheckFunctions.HealthInfosList.Where(hi => hi.Key == key).SingleOrDefault();
            if (healthFunction != null)
            {
                return healthFunction.Check(this);
            }
            else
            {
                //TODO:Fix Exception
                throw new Exception(); // string.Format("HealthCheckFunction with key:{0} does not exists", key));
            }
        }

        [System.ComponentModel.Composition.Import(typeof(IParameterStore), AllowDefault = true)]
        public IParameterStore ParameterStore { get; set; }

        /// <summary>
        /// Задава стойности на параметри
        /// </summary>
        /// <param name="parameters">Списък с ключове на параметри и техните стойности</param>
        public void SetParameters(List<ParameterInfo> parameters)
        {
            if (ParameterStore != null)
            {
                ParameterStore.SetParameters(this, parameters);
            }
            else
            {
                throw new ApplicationException("ParameterStore is not set");
            }   
        }

        /// <summary>
        /// Gets the version of the adapter
        /// </summary>
        /// <returns>The version of the adapter</returns>
        public string GetAdapterVersion()
        {
            return GetType().Assembly.GetName().Version.ToString();
        }

        //TODO: Structure the result; Add i
        public string GetAdapterVersionFull()
        {
            var appDomain = AppDomain.CurrentDomain;
            return appDomain
                .GetAssemblies()
                .Select(
                    a => new
                    {
                        AssemblyName = a.GetName().Name,
                        AssemblyFullName = a.FullName,
                        AssemblyVersion = a.GetName().Version.ToString(),
                        CodeBase = a.CodeBase,
                        Location = a.Location
                    }
                ).XmlSerialize();
            //System.Reflection.Assembly thisAssembly = this.GetType().Assembly;
            //System.Reflection.Assembly executingAssembly = System.Reflection.Assembly.GetExecutingAssembly();
            //System.Reflection.Assembly callingAssembly = System.Reflection.Assembly.GetCallingAssembly();
            //System.Reflection.Assembly entryAssembly = System.Reflection.Assembly.GetEntryAssembly();

            //StringBuilder StringBuilder1 = new StringBuilder(string.Empty);
            //StringBuilder1.AppendFormat("AdapterVersion: {0}\n", thisAssembly.GetName().Version.ToString());
            //StringBuilder1.AppendFormat("ThisAssembly\n{0}", GetAssemblyDescription(thisAssembly));
            //StringBuilder1.AppendFormat("ExecutingAssembly\n{0}", GetAssemblyDescription(executingAssembly));
            //StringBuilder1.AppendFormat("CallingAssembly\n{0}", GetAssemblyDescription(callingAssembly));
            //StringBuilder1.AppendFormat("EntryAssembly\n{0}", GetAssemblyDescription(entryAssembly));

            //return StringBuilder1.ToString();
        }

        /// <summary>
        /// Запазва подадената информация в log
        /// </summary>
        /// <param name="adapterAdditionalParameters">Допълнителни параметри за извкиване на адаптер</param>
        /// <param name="data">Допълнителни данни</param>
        public void LogData(AdapterAdditionalParameters adapterAdditionalParameters, object data)
        {
            StackTrace stackTrace = new StackTrace();
            System.Reflection.MethodBase operationMethod = stackTrace.GetFrame(1).GetMethod();
            if (adapterAdditionalParameters != null)
            {
                //log.Info(
                //    new
                //    {
                //        MethodName = operationMethod.Name,
                //        CallId = adapterAdditionalParameters.ApiServiceCallId,
                //        CallContext = adapterAdditionalParameters.CallContext,
                //        ClientIPAddress = adapterAdditionalParameters.ClientIPAddress,
                //        EIDToken = adapterAdditionalParameters.EIDToken,
                //        OrganizationUnit = adapterAdditionalParameters.OrganizationUnit,
                //        CitizenEGN = adapterAdditionalParameters.CitizenEGN,
                //        ConsumerCertificateThumbprint = adapterAdditionalParameters.ConsumerCertificateThumbprint,
                //        EmployeeEGN = adapterAdditionalParameters.EmployeeEGN,
                //        OrganizationEIK = adapterAdditionalParameters.OrganizationEIK,
                //        SignResult = adapterAdditionalParameters.SignResult,
                //        ReturnAccessMatrix = adapterAdditionalParameters.ReturnAccessMatrix,
                //        Guid = data.GetType().GetProperty(typeof(Guid).Name) != null ? data.GetType()
                //            .GetProperty(typeof(Guid).Name)
                //            .GetValue(data)
                //            .ToString() : string.Empty,
                //        OtherData = data != null ? data.ToString() : string.Empty
                //    });
            }
        }

        /// <summary>
        /// Запазва подадената информация в log
        /// </summary>
        /// <param name="message">Данни</param>
        public void LogWarn(object message)
        {
            //log.Warn(message);
        }

        /// <summary>
        /// Запазва подадената информация  за грешка в log
        /// </summary>
        /// <param name="adapterAdditionalParameters">Допълнителни параметри за извкиване на адаптер</param>
        /// <param name="data">Допълнителни данни</param>
        public void LogError(AdapterAdditionalParameters adapterAdditionalParameters, Exception ex, object data, [CallerMemberName] string callingMethodName = "")
        {
            //log.Error(
            //     new
            //     {
            //         MethodName = callingMethodName,
            //         CallContext = adapterAdditionalParameters?.CallContext,
            //         ClientIPAddress = adapterAdditionalParameters?.ClientIPAddress,
            //         EIDToken = adapterAdditionalParameters?.EIDToken,
            //         OrganizationUnit = adapterAdditionalParameters?.OrganizationUnit,
            //         CitizenEGN = adapterAdditionalParameters?.CitizenEGN,
            //         ConsumerCertificateThumbprint = adapterAdditionalParameters?.ConsumerCertificateThumbprint,
            //         EmployeeEGN = adapterAdditionalParameters?.EmployeeEGN,
            //         OrganizationEIK = adapterAdditionalParameters?.OrganizationEIK,
            //         Guid = data.GetType()
            //            .GetProperty(typeof(Guid).Name)
            //            .GetValue(data)
            //            .ToString(),
            //         OtherData = data.ToString()
            //     },
            //     ex);
        }

        /// <summary>
        /// Проверява връзката на адаптер към регистър
        /// </summary>
        /// <returns>Резултат от проверката</returns>
        public virtual string CheckRegisterConnection()
        {
            return "OK";
        }
        
        /// <summary>
        /// Gets info for the host machine
        /// </summary>
        public virtual string GetHostMachineInfo()
        {
            //StringBuilder StringBuilder1 = new StringBuilder(string.Empty);

            //StringBuilder1.AppendFormat("HOSTING ENVIRONMENT\n");

            //StringBuilder1.AppendFormat("\t Site name: {0}\n", System.Web.Hosting.HostingEnvironment.SiteName);
            //StringBuilder1.AppendFormat("\t Virtual Path: {0}\n", System.Web.Hosting.HostingEnvironment.ApplicationVirtualPath);
            //StringBuilder1.AppendFormat("\t Physical Path: {0}\n", System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath);
            //StringBuilder1.AppendFormat("\t ApplicationID: {0}\n", System.Web.Hosting.HostingEnvironment.ApplicationID);

            //string hostname = System.Net.Dns.GetHostName();
            //string hostIpAddressesList = string.Empty;
            //StringBuilder1.AppendFormat("NET DNS INFORMATION\n\t Host name: {0}\n", hostname);
            //StringBuilder1.AppendFormat("\t IP Addresses:\n");
            //int i = 0;
            //foreach (var address in System.Net.Dns.GetHostEntry(hostname).AddressList)
            //{
            //    StringBuilder1.AppendFormat("\t\t [IpAddress{0}]: [{1}];\n", i, address.ToString());
            //    i++;
            //}

            //return StringBuilder1.AppendFormat("SYSTEM INFORMATION\n{0}", SystemInformation()).ToString();
            return "";
        }

        public virtual bool SendResultToCore(ServiceResultData result, string callbackURL)
        {
            try
            {
                var binding = new BasicHttpBinding();
                binding.Security.Mode = BasicHttpSecurityMode.Transport;
                binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.None;
                var regiXServerChannel =
                    new ChannelFactory<IReceiveResult>(
                        binding,
                        new EndpointAddress(ParametersInfoDefinitions.RegiXCallbackEndpoint.Value)
                    ).CreateChannel();
                //log.Debug($"Sending result to core: {ParametersInfoDefinitions.RegiXCallbackEndpoint.Value}");
                regiXServerChannel.ReceiveResult(result, callbackURL);
                //log.Debug("Result send successfully");
                return true;
            }
            catch(Exception ex)
            {
                //log.Error("Error while sending result to core", ex);
                return false;
            }
        }
        
        /// <summary>
        /// Gets the uptime of the adapter
        /// </summary>
        /// <returns>The uptime of the adapter</returns>
        public uint GetAdapterUptime()
        {
            return Convert.ToUInt32((DateTime.Now - Process.GetCurrentProcess().StartTime).TotalSeconds);
        }

        /// <summary>
        /// Gets the uptime of the adapter
        /// </summary>
        /// <returns>The uptime of the adapter</returns>
        public uint GetSystemUptime()
        {
            long ticks = Stopwatch.GetTimestamp();
            double uptime = ((double)ticks) / Stopwatch.Frequency;
            uint uptimeTimeSeconds = Convert.ToUInt32(uptime);
            return uptimeTimeSeconds;
        }

        protected string SystemInformation()
        {
            StringBuilder StringBuilder1 = new StringBuilder(string.Empty);
            try
            {
                StringBuilder1.AppendFormat("\t Machine Name:  {0}\n", Environment.MachineName);
                StringBuilder1.AppendFormat("\t Operation System:  {0}\n", Environment.OSVersion);
                if (Environment.Is64BitOperatingSystem)
                    StringBuilder1.AppendFormat("\t\t  64 Bit Operating System\n");
                else
                    StringBuilder1.AppendFormat("\t\t  32 Bit Operating System\n");
                StringBuilder1.AppendFormat("\t WorkingSet:  {0}\n", Environment.WorkingSet.ToString());
                StringBuilder1.AppendFormat("\t SystemDirectory:  {0}\n", Environment.SystemDirectory);
                StringBuilder1.AppendFormat("\t ProcessorCount:  {0}\n", Environment.ProcessorCount);
                StringBuilder1.AppendFormat("\t UserDomainName:  {0}\n", Environment.UserDomainName);
                StringBuilder1.AppendFormat("\t UserName: {0}\n", Environment.UserName);
                //Drives
                StringBuilder1.AppendFormat("\t LogicalDrives:\n");
                foreach (System.IO.DriveInfo DriveInfo1 in System.IO.DriveInfo.GetDrives())
                {
                    try
                    {
                        StringBuilder1.AppendFormat("\t Drive: {0}\n\t\t VolumeLabel: " +
                          "{1}\n\t\t DriveType: {2}\n\t\t DriveFormat: {3}\n\t\t " +
                          "TotalSize: {4}\n\t\t AvailableFreeSpace: {5}\n",
                          DriveInfo1.Name, DriveInfo1.VolumeLabel, DriveInfo1.DriveType,
                          DriveInfo1.DriveFormat, DriveInfo1.TotalSize, DriveInfo1.AvailableFreeSpace);
                    }
                    catch
                    {
                    }
                }
                StringBuilder1.AppendFormat("\t SystemPageSize:  {0}\n", Environment.SystemPageSize);
                StringBuilder1.AppendFormat("\t Version:  {0}", Environment.Version);

            }
            catch (Exception ex)
            {
                //log.Error("Error whihle retrieving system information", ex);
            }
            return StringBuilder1.ToString();
        }
        
        /// <summary>
        /// Извлича информация за подаденото assembly
        /// </summary>
        /// <param name="a">Assembly</param>
        /// <returns>Извлечената информация</returns>
        private static string GetAssemblyDescription(System.Reflection.Assembly a)
        {
            StringBuilder StringBuilder1 = new StringBuilder(string.Empty);
            if (a != null)
            {
                StringBuilder1.AppendFormat("\t\t FullName = {0}\n", a.GetName().FullName);
                StringBuilder1.AppendFormat("\t\t Version = {0}\n", a.GetName().Version.ToString());
                StringBuilder1.AppendFormat("\t\t CodeBase = {0}\n", a.CodeBase);
                StringBuilder1.AppendFormat("\t\t Location = {0}\n", a.Location);
            }
            return StringBuilder1.ToString();
        }

        public string GetPublicKeyString()
        {
            return Composition.CompositionContainer.GetExportedValue<ISigner>().GetPublicKeyString();
        }

        public ServiceResultData Execute(RequestWrapper request)
        {
            return Execute(request.Request, request.AccessMatrix, request.AdditionalParameters.Deserialize<AdapterAdditionalParameters>());
        }

        /// <summary>
        /// Изпълнява произволна заявка използвайки информацията отполучената килентска заявка. Използва се в случай когато се налага изпълнение чрез "прозрачно" RegiX ядро.
        /// </summary>
        /// <param name="request">Клиентска заявка</param>
        /// <param name="accessMatrix">Матрица на достъп</param>
        /// <param name="additionalParameters">Допълнителни параметри</param>
        /// <returns>Резултат от изпълнение на заявката. В случай, че изпълнението е асинхронно се връща резултат с IsReady = false</returns>
        public ServiceResultData Execute(ServiceRequestData request, AccessMatrix accessMatrix, AdapterAdditionalParameters additionalParameters)
        {
            try
            {
                if (additionalParameters != null && request != null)
                {
                    additionalParameters.RequestProcessing = request.RequestProcessing;
                    additionalParameters.ResponseProcessing = request.ResponseProcessing;
                }                
                System.Reflection.MethodInfo operationMethod = GetType().GetMethod(request.OperationName);
                Type expectedType = operationMethod.GetParameters()[0].ParameterType;
                AddOperationInfo(request, additionalParameters);
                //log.Debug($"Invoking method: {operationMethod.Name}");
                IProvideServiceResultData result =
                    operationMethod.Invoke(
                        this,
                        new object[]
                        {
                            ExtractArgument(expectedType, request, additionalParameters),
                            accessMatrix,
                            additionalParameters
                        }
                    ) as IProvideServiceResultData;
                //log.Debug(result.XmlSerialize());
                ServiceResultData serviceResult = PrepareResult(result, additionalParameters);
                return serviceResult;
            }
            catch(Exception ex)
            {
                //log.Error(ex.Message, ex);
                throw ex;
            }
        }

        private void AddOperationInfo(ServiceRequestData request, AdapterAdditionalParameters additionalParameters)
        {
            additionalParameters.ProcessingData.AddRange(
                new ProcessingDataEntry[] {
                    new ProcessingDataEntry()
                    {
                        Key = OPERATION_NAME_KEY,
                        Value = request.OperationName.XmlSerialize().ToXmlElement()
                    },
                    new ProcessingDataEntry()
                    {
                        Key = API_SERVICE_INTERFACE,
                        Value = request.Contract.XmlSerialize().ToXmlElement()
                    }
                }
            );
        }

        /// <summary>
        /// Проверка за резултат от изпълнение на асинхронна заявка
        /// </summary>
        /// <param name="checkResult">Аргумент съдържащ код на подадена заявка</param>
        /// <returns>Резултат от изпълнението (възможно е резултата да не е готов)</returns>
        public ServiceResultData CheckResult(ServiceCheckResultArgument checkResult)
        {
            var persitanceProvider = Composition.CompositionContainer.GetExportedValue<IPersistanceProvider>();
            return persitanceProvider.RetrieveResult(checkResult.ServiceCallID, checkResult.VerificationCode);
        }

        public void AcknowledgeResultReceived(ServiceCheckResultArgument checkResult)
        {
            var persitanceProvider = Composition.CompositionContainer.GetExportedValue<IPersistanceProvider>();
            persitanceProvider.Remove(checkResult.ServiceCallID, checkResult.VerificationCode);
        }

        /// <summary>
        /// Извлича аргумента за съответната операция от подадената заявка
        /// </summary>
        /// <param name="expectedType">Тип на аргумент на операция</param>
        /// <param name="request">Клиентска заявка</param>
        /// <returns>Извлеченият тип</returns>
        private object ExtractArgument(Type expectedType, ServiceRequestData request, AdapterAdditionalParameters additionalParameters)
        {
            var requestProcessingFlags = 
                new RequestProcessing[] 
                {
                    RequestProcessing.Decrypt,
                    RequestProcessing.Verify,
                    RequestProcessing.TransformFromPDF
                };
            foreach(var requestProcessingFlag in requestProcessingFlags)
            {
                if (request.RequestProcessing.HasFlag(requestProcessingFlag))
                {
                    var requestProcessor = Composition.CompositionContainer.GetExportedValue<IRequestProcessor>(Enum.GetName(typeof(RequestProcessing), requestProcessingFlag));
                    request = requestProcessor.ProcessRequest(request, additionalParameters);
                }
            }
            object deserializedObject = request.Argument.Deserialize(expectedType);
            return deserializedObject;
        }

        /// <summary>
        /// Подготвя резултат, който да бъде доставен на клиента
        /// </summary>
        /// <param name="result">Получен резултат от операция</param>
        /// <param name="request">Клиентска заявка</param>
        /// <returns>Изготвения резултат спрямо изискванията посочени в клиентската заявка</returns>
        public ServiceResultData PrepareResult(IProvideServiceResultData result, AdapterAdditionalParameters additionalParameters)
        {
            ServiceResultData serviceResult = result.ToServiceResultDataSigned();
            if (!serviceResult.HasError && serviceResult.IsReady)
            {
                var requestProcessingFlags =
                    new ResponseProcessing[]
                    {
                        ResponseProcessing.TransformToPDF,
                        ResponseProcessing.Encrypt
                    };
                foreach (var responseProcessingFlag in requestProcessingFlags)
                {
                    if (additionalParameters.ResponseProcessing.HasFlag(responseProcessingFlag))
                    {
                        var requestProcessor = Composition.CompositionContainer.GetExportedValue<IResponseProcessor>(Enum.GetName(typeof(ResponseProcessing), responseProcessingFlag));
                        serviceResult = requestProcessor.ProcessResponse(serviceResult, additionalParameters);
                    }
                }
            }
            serviceResult.ServiceCallID = additionalParameters.ApiServiceCallId;
            var verificationCode = additionalParameters.ProcessingData.SingleOrDefault(pd => pd.Key == "VerificationCode");
            if (verificationCode != null) 
            {
                serviceResult.VerificationCode = verificationCode.Value.OuterXml.XmlDeserialize<string>();
            }
            return serviceResult;
        }

        public CommonSignedResponse<Req, Resp> ProcessAsynchronous<Req, Resp>(Req request, AccessMatrix accessMatrix, AdapterAdditionalParameters additionalParameters, Resp response = null, [CallerMemberName] string callingMethodName = "")
            where Req : class
            where Resp : class
        {

            var salt = new byte[64];
            using (var random = new RNGCryptoServiceProvider())
            {
                random.GetNonZeroBytes(salt);
            }
            string verificationCode = $"{additionalParameters.ApiServiceCallId}.{Convert.ToBase64String(salt)}";

            var persitanceProvider = Composition.CompositionContainer.GetExportedValue<IPersistanceProvider>();
            persitanceProvider.PersistProcessing(additionalParameters.ApiServiceCallId, verificationCode);

            if (additionalParameters.ProcessingData.Count( pd => pd.Key == "OperationName") == 0)
            {
                additionalParameters.ProcessingData.Add(
                    new ProcessingDataEntry()
                    {
                        Key = "OperationName",
                        Value = callingMethodName.XmlSerialize().ToXmlElement()
                    }
                );
            }
            additionalParameters.ProcessingData.Add(
                new ProcessingDataEntry()
                {
                    Key = "VerificationCode",
                    Value = verificationCode.XmlSerialize().ToXmlElement()
                }
            );
            var asyncProcessorType = typeof(IAsynchronousProcessor<>).MakeGenericType(new Type[] { GetType() });
            var methodInfo = typeof(CompositionContainer)
                .GetMethod(
                nameof(CompositionContainer.GetExportedValue), 
                new Type[]{}
            ).MakeGenericMethod(asyncProcessorType);
            var asynchronousProcessor = methodInfo.Invoke(Composition.CompositionContainer, new object[] { });
            var processMethod = asyncProcessorType.GetMethod(nameof(IAsynchronousProcessor<IAdapterService>.Process));
            var processMethodGeneric = processMethod.MakeGenericMethod(new Type[] { typeof(Req), typeof(Resp) });
            var result = processMethodGeneric.Invoke(
                asynchronousProcessor,
                new object[] {
                    request,
                    accessMatrix,
                    additionalParameters,
                    response
                }
            );
            var res = result as CommonSignedResponse<Req, Resp>;
            res.VerificationCode = verificationCode;
            return res;
        }

        public bool ProcessCallback<Req, Resp>(CommonSignedResponse<Req, Resp> response, AdapterAdditionalParameters additionalParameters)
            where Req : class
            where Resp : class
        {
            try
            {
                ServiceResultData resultData = PrepareResult(response, additionalParameters);
                return ProcessCallback(resultData, additionalParameters);
            }
            catch(Exception ex)
            {
                //log.Error("Error Preparing result!", ex);
                return false;
            }
        }

        public bool ProcessCallback(ServiceResultData resultData, AdapterAdditionalParameters additionalParameters)
        {
            try
            {
                //log.Debug($"Callback URL specified: {additionalParameters.CallbackURL}");
                if (string.IsNullOrEmpty(additionalParameters.CallbackURL) ||
                    !SendResultToCore(resultData, additionalParameters.CallbackURL))
                {
                    //log.Debug($"Persisting result");
                    var persistanceProvider = Composition.CompositionContainer.GetExportedValue<IPersistanceProvider>();
                    persistanceProvider.PersistResult(resultData.ServiceCallID, resultData);
                }
                return true;
            }
            catch (Exception ex)
            {
                //log.Error("Error processing callback!", ex);
                return false;
            }
        }
    }
}
