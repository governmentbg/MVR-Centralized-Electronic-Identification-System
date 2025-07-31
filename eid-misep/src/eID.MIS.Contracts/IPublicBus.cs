using MassTransit;

namespace eID.MIS.Contracts
{
    /// <summary>
    /// Must be configured to publish raw json messages for subscribed external systems.
    /// </summary>
    public interface IPublicBus : IBus { }
}
