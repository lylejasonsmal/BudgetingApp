using MyFirstApp.Domain.Values;
using MyFirstApp.Services.Implementations.PublisherService;
using MyFirstApp.Services.Interfaces;

namespace MyFirstApp.Services.Implementations.ToastService
{
    public class ToastService: PublisherService<Result>, IToastService
    {
        public async Task SubscribeAsync(Func<Result, Task> handler)
        {
            Subscribe(handler);
            await Task.CompletedTask;
        }

        public async Task ShowSuccessToastAsync(string message)
        {
            await PublishAsync(new Result(ResultOutcome.Success, null, message));
        }

        public async Task ShowFailureToastAsync(string message)
        {
            await PublishAsync(new Result(ResultOutcome.Failure, null, message));
        }
    }
}
