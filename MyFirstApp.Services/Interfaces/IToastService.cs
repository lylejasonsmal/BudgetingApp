using MyFirstApp.Domain.Values;
using MyFirstApp.Services.Interfaces.PublisherService;

namespace MyFirstApp.Services.Interfaces
{
    public interface IToastService : IPublisher<Result>
    {
        Task ShowSuccessToastAsync(string message);
        Task ShowFailureToastAsync(string message);
    }
}
