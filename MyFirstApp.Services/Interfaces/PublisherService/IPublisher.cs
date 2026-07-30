namespace MyFirstApp.Services.Interfaces.PublisherService
{
    public interface IPublisher<T>
    {
        IDisposable Subscribe(Func<T, Task> handler);
        Task<IDisposable> SubscribeAsync(Func<T, Task> handler);
        Task PublishAsync(T value);
    }
}
