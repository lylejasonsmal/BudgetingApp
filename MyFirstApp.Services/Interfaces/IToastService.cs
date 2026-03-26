using MyFirstApp.Domain.Values;
using MyFirstApp.Services.Interfaces.PublisherService;

namespace MyFirstApp.Services.Interfaces
{
    public interface IToastService : IPublisher<Result>
    {
        Task SubscribeAsync(Func<Result, Task> handler);
        Task ShowSuccessToastAsync(string message);
    }
}
