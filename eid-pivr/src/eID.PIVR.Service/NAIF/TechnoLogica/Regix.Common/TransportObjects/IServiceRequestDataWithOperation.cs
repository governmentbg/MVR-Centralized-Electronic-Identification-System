
namespace TechnoLogica.RegiX.Common.TransportObjects
{
    /// <summary>
    /// Extends the IServiceRequestData  interface by adding additional contract and operation name
    /// </summary>
    public interface IServiceRequestDataWithOperation : IServiceRequestData
    {
        /// <summary>
        /// Fullname of the contract's interface describing the operations part of a given register
        /// </summary>
        string Contract { get; }

        /// <summary>
        /// Name of the operation. Defined by the Operation property
        /// </summary>
        string OperationName { get; }
    }
}
