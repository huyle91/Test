using InfertilityTreatment.Business.Interfaces;
using System.Collections.Concurrent;

namespace InfertilityTreatment.Business.Services
{
    public class NotificationEventService : INotificationEventService
    {
        private readonly ConcurrentQueue<INotificationEvent> _eventQueue = new();
        private readonly List<Func<INotificationEvent, Task>> _handlers = new();

        public Task PublishNotificationEventAsync(INotificationEvent notificationEvent)
        {
            _eventQueue.Enqueue(notificationEvent);
            
            // Process handlers asynchronously
            return Task.Run(async () =>
            {
                var tasks = _handlers.Select(handler => handler(notificationEvent));
                await Task.WhenAll(tasks);
            });
        }

        public void RegisterHandler(Func<INotificationEvent, Task> handler)
        {
            _handlers.Add(handler);
        }

        public void ClearHandlers()
        {
            _handlers.Clear();
        }
    }
}
