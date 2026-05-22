using MyFirstApp.Services.Interfaces.PublisherService;

namespace MyFirstApp.Services.Implementations.PublisherService
{
    public abstract class PublisherService<T> : IPublisher<T>
    {
        // Store async handlers instead of synchronous actions
        private readonly List<Func<T, Task>> _subscribers = new();

        // Subscribe method now takes a Func<T, Task>
        public IDisposable Subscribe(Func<T, Task> handler)
        {
            _subscribers.Add(handler);
            return new Unsubscriber(_subscribers, handler);
        }

        // Publish awaits all subscribers
        public async Task PublishAsync(T? value)
        {
            var tasks = _subscribers.Select(sub => sub(value)).ToList();
            await Task.WhenAll(tasks); // ensures all async handlers complete
        }

        private class Unsubscriber : IDisposable
        {
            private readonly List<Func<T, Task>> _subs;
            private readonly Func<T, Task> _handler;

            public Unsubscriber(List<Func<T, Task>> subs, Func<T, Task> handler)
            {
                _subs = subs;
                _handler = handler;
            }

            public void Dispose()
            {
                if (_subs.Contains(_handler))
                    _subs.Remove(_handler);
            }
        }
    }
}