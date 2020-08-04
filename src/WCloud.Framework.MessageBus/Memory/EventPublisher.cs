using System;
using Lib.data;
using Lib.extension;
using Microsoft.Extensions.DependencyInjection;

namespace Lib.events
{
    public static class EventPublisherExtensions
    {
        public static void EntityInserted<T>(this IEventPublisher eventPublisher, T entity) where T : IDBTable
        {
            eventPublisher.Publish(new EntityInserted<T>(entity));
        }

        public static void EntityUpdated<T>(this IEventPublisher eventPublisher, T entity) where T : IDBTable
        {
            eventPublisher.Publish(new EntityUpdated<T>(entity));
        }

        public static void EntityDeleted<T>(this IEventPublisher eventPublisher, T entity) where T : IDBTable
        {
            eventPublisher.Publish(new EntityDeleted<T>(entity));
        }
    }

    /// <summary>
    /// Evnt publisher
    /// </summary>
    public class EventPublisher : IEventPublisher
    {
        private readonly IServiceProvider provider;
        public EventPublisher(IServiceProvider provider)
        {
            this.provider = provider;
        }

        /// <summary>
        /// Publish event
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="eventMessage">Event message</param>
        public virtual void Publish<T>(T eventMessage)
        {
            var subscriptions = this.provider.ResolveAll_<IConsumer<T>>();
            subscriptions.ForEach_(x => x.HandleEvent(eventMessage));
        }

    }
}
