
namespace Lib.events
{
    /// <summary>
    /// Evnt publisher_原来这个叫中介者模式~~
    /// </summary>
    public interface IEventPublisher
    {
        /// <summary>
        /// Publish event
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="eventMessage">Event message</param>
        void Publish<T>(T eventMessage);
    }
}
