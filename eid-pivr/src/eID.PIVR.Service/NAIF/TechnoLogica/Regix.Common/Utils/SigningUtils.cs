using System;
using System.Configuration;
using System.Reflection;
using System.Xml;
using TechnoLogica.RegiX.Common.DataContracts;
using TechnoLogica.RegiX.Common.ObjectMapping;
using System.Linq;

namespace TechnoLogica.RegiX.Common.Utils
{
    /// <summary>
    /// Signing utils
    /// </summary>
    public class SigningUtils
    {
        /// <summary>
        /// Method info for the CreateAndSign method
        /// </summary>
        private static readonly MethodInfo CreateAndSignMethod =
            typeof(SigningUtils)
                .GetMethods(BindingFlags.Static | BindingFlags.Public)
                .Where(
                    mi =>
                        mi.Name == "CreateAndSign" &&
                        mi.GetParameters().Count() == 5
                )
                .First();

        /// <summary>
        /// Converts data container to XMLElement
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static XmlElement DataContainerToXmlElement(DataContainer data)
        {
            XmlElement result = data.XmlSerialize().ToXmlElement();
            return result;
        }

        /// <summary>
        /// Creates XML from provided data
        /// </summary>
        /// <typeparam name="R">Request type</typeparam>
        /// <typeparam name="T">Response type</typeparam>
        /// <param name="req">Request</param>
        /// <param name="resp">Response</param>
        /// <param name="am">Access matrix</param>
        /// <param name="signature">Signature </param>
        /// <returns>Constructed XML</returns>
        public static XmlElement GetXmlWithSignature<R, T>(R req, T resp, DataAccessMatrix am, XmlElement signature)
        {
            return GetXmlWithSignature(req.XmlSerialize().ToXmlElement(), resp.XmlSerialize().ToXmlElement(), am, signature);
        }

        /// <summary>
        /// Creates XML from provided data (not typed data)
        /// </summary>
        /// <param name="req">Request string</param>
        /// <param name="resp">Response string</param>
        /// <param name="am">Access matrix</param>
        /// <param name="signature">Signature</param>
        /// <returns>Constrcuted XML</returns>
        public static XmlElement GetCommonSignedResponseWithSignature(string req, string resp, DataAccessMatrix am, XmlElement signature)
        {
            CommonSignedResponse result = new CommonSignedResponse();
            result.Data = GetDataContainer(req.ToXmlElement(), resp.ToXmlElement(), am);
            var resultDocument = result.XmlSerialize().ToXmlElement();
            if (signature != null)
            {
                resultDocument.AppendChild(resultDocument.OwnerDocument.ImportNode(signature, true));
            }
            return resultDocument;
        }

        /// <summary>
        /// Constructs data container from provided data
        /// </summary>
        /// <param name="req">Request</param>
        /// <param name="resp">Response</param>
        /// <param name="am">Access matrix</param>
        /// <returns>Constructed data container</returns>
        public static DataContainer GetDataContainer(XmlElement req, XmlElement resp, DataAccessMatrix am)
        {
            var data = new DataContainer();
            data.Request = new RequestContainer { Request = req };
            data.Response = new ResponseContainer { Response = resp };
            data.Matrix = am;
            return data;
        }

        /// <summary>
        /// Creates a CommonSignedResponse object from provided data
        /// </summary>
        /// <typeparam name="A">Request type</typeparam>
        /// <typeparam name="R">Response type</typeparam>
        /// <param name="req">Request</param>
        /// <param name="resp">Response</param>
        /// <param name="am">Access matrix</param>
        /// <returns>Constructed CommonSignedResponse</returns>
        public static CommonSignedResponse<A, R> GetCommonSignedResponse<A, R>(A req, R resp, AccessMatrix am)
        {
            CommonSignedResponse<A, R> result = new CommonSignedResponse<A, R>();
            result.Data = new DataContainer<A, R>();
            result.Data.Request = req;
            if (am != null)
            {
                result.Data.Matrix = new DataAccessMatrix(am, typeof(R));
            }
            result.Data.Response = resp;
            return result;
        }
        
        /// <summary>
        /// Validates XMLElement
        /// </summary>
        /// <param name="doc">XMLElement to validate</param>
        /// <returns>If the verification is successful</returns>
        public static bool ValidateXmlDocumentWithX509Certificate(XmlElement doc)
        {
            return Composition.CompositionContainer.GetExportedValue<ISigner>().Validate(doc);
        }

        /// <summary>
        /// Validates XMLDocument
        /// </summary>
        /// <param name="doc">XMLDocument to validate</param>
        /// <returns>If the verification is successful</returns>
        public static bool ValidateXmlDocumentWithX509Certificate(XmlDocument doc)
        {
            return Composition.CompositionContainer.GetExportedValue<ISigner>().Validate(doc);
        }

        /// <summary>
        /// Checks if the provided document has a signature element
        /// </summary>
        /// <param name="doc">XML Document</param>
        /// <returns>If the Signature element is found</returns>
        public static bool HasSignature(XmlDocument doc)
        {
            return doc.GetElementsByTagName("Signature").Count > 0;
        }

        /// <summary>
        /// Constructs a CommonSignedResponse object
        /// </summary>
        /// <typeparam name="R">Request type</typeparam>
        /// <typeparam name="T">Response type</typeparam>
        /// <param name="argument">Request</param>
        /// <param name="result">Response</param>
        /// <param name="accessMatrix">Access matrix</param>
        /// <param name="aditionalParameters">Additional parameters</param>
        /// <param name="signResult">If the object should include a signature</param>
        /// <returns>Constructed CommonSignedResponse object</returns>
        public static CommonSignedResponse<R, T> CreateAndSign<R, T>(R argument, T result, AccessMatrix accessMatrix, AdapterAdditionalParameters aditionalParameters, bool signResult)
        {
            AccessMatrix returnedAccessMatrix = null;
            bool? returnAccessMatrix = aditionalParameters?.ReturnAccessMatrix;
            if (returnAccessMatrix.HasValue && returnAccessMatrix.Value)
            {
                returnedAccessMatrix = accessMatrix;
            }
            CommonSignedResponse<R, T> commonResult = GetCommonSignedResponse(argument, result, returnedAccessMatrix);
            bool? aditionalParametersSignResult = aditionalParameters?.SignResult;
            if (signResult && aditionalParametersSignResult.HasValue && aditionalParametersSignResult.Value)
            {
                var signer = Composition.CompositionContainer.GetExportedValue<ISigner>();
                signer.SignXmlDocumentWithCertificate(commonResult);
            }
            return commonResult;
        }

        /// <summary>
        /// Constructs a CommonSignedResponse object
        /// </summary>
        /// <typeparam name="R">Request type</typeparam>
        /// <typeparam name="T">Response type</typeparam>
        /// <param name="argument">Request</param>
        /// <param name="result">Response</param>
        /// <param name="accessMatrix">Access matrix</param>
        /// <param name="aditionalParameters">Additional parameters</param>
        /// <returns>Constructed CommonSignedResponse object</returns>
        public static CommonSignedResponse<R, T> CreateAndSign<R, T>(R argument, T result, AccessMatrix accessMatrix, AdapterAdditionalParameters aditionalParameters)
        {
            bool shouldSign = true;
            return CreateAndSign<R, T>(argument, result, accessMatrix, aditionalParameters, shouldSign);
        }
        
        /// <summary>
        /// Constructs a CommonSignedResponse object - non  generic version
        /// </summary>
        /// <param name="argument">Request</param>
        /// <param name="argumentType">Request type</param>
        /// <param name="result">Response</param>
        /// <param name="resultType">Response type</param>
        /// <param name="accessMatrix">Access matrix</param>
        /// <param name="aditionalParameters">Additional parameters</param>
        /// <param name="signResult">If the object should include a signature</param>
        /// <returns>Constructed CommonSignedResponse object</returns>
        public static object CreateAndSignNonGeneric(object argument, Type argumentType, object result, Type resultType, AccessMatrix accessMatrix, AdapterAdditionalParameters aditionalParameters, bool signResult)
        {
            var method = CreateAndSignMethod.MakeGenericMethod(argumentType, resultType);
            object commonResult = method.Invoke(null, new object[] { argument, result, accessMatrix, aditionalParameters, signResult });
            return commonResult;
        }

        /// <summary>
        /// Constructs a CommonSignedResponse object - non  generic version. 
        /// Information if the response should be signed is obatined from the SignResponse application setting.
        /// </summary>
        /// <param name="argument">Request</param>
        /// <param name="argumentType">Request type</param>
        /// <param name="result">Response</param>
        /// <param name="resultType">Response type</param>
        /// <param name="accessMatrix">Access matrix</param>
        /// <param name="aditionalParameters">Additional parameters</param>
        /// <returns>Constructed CommonSignedResponse object</returns>
        public static object CreateAndSignNonGeneric(object argument, Type argumentType, object result, Type resultType, AccessMatrix accessMatrix, AdapterAdditionalParameters aditionalParameters)
        {
            var signResponse = ConfigurationManager.AppSettings["SignResponse"];
            bool shouldSign = (!string.IsNullOrEmpty(signResponse)) ? Convert.ToBoolean(signResponse) : true;
            return CreateAndSignNonGeneric(argument, argumentType, result, resultType, accessMatrix, aditionalParameters, shouldSign);
        }
    }
}
