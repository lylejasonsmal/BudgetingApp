using MyFirstApp.Domain.Models;
using MyFirstApp.Domain.Values;
using MyFirstApp.Services.Interfaces.PublisherService;

namespace MyFirstApp.Services.Interfaces
{
    public interface IExpenseService : IPublisher<ExpenseModel>
    {
        Task SubscribeAsync(Func<ExpenseModel, Task> handler);
        Task<Result> UpdateExpenseAsync(ExpenseModel expenseModel);
        Task<Result> CreateExpenseAsync(ExpenseModel expenseModel);
        Task DeleteExpenseAsync(ExpenseModel expenseModel);
    }
}
