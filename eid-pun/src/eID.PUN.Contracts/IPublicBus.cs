using MassTransit;

namespace eID.PUN.Contracts
{
    /// <summary>
    /// Must be configured to publish raw json messages for subscribed external systems.
    /// </summary>
    public interface IPublicBus : IBus { }
}
