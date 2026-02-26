namespace MyFirstApp.Services.Interfaces.PublisherService
{
    public interface IPublisher<T>
    {
        IDisposable Subscribe(Func<T, Task> handler);
        Task Publish(T value);
    }
}
