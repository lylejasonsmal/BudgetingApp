using MyFirstApp.Domain.Models;
using MyFirstApp.Domain.Values;
using MyFirstApp.Services.Interfaces.PublisherService;

namespace MyFirstApp.Services.Interfaces
{
    public interface IMonthlyBudgetFixtureService : IPublisher<MonthlyBudgetFixtureModel>
    {
        Task SubscribeAsync(Func<MonthlyBudgetFixtureModel, Task> handler);
        Task<Result> RecalculateAndUpdateBudgetFixtureAsync();
    }
}
