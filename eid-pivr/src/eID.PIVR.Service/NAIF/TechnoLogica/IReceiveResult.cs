using System.ServiceModel;
using TechnoLogica.RegiX.Common.TransportObjects;

namespace TechnoLogica.RegiX.Common
{    
    /// <summary>
    /// Defines service for receiving results of operation execution
    /// </summary>
    [ServiceContract]
    [XmlSerializerFormat]
    public interface IReceiveResult
    {
        /// <summary>
        /// Receives the result of an operation's execution and the callback URL used
        /// for delivering the result
        /// </summary>
        /// <param name="argument">Operation execution result</param>
        /// <param name="callbackURL">Callback URL</param>
        [OperationContract]
        void ReceiveResult(ServiceResultData argument, string callbackURL);
    }
}
