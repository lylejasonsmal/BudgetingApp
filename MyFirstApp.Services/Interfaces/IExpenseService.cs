using MyFirstApp.Domain.Models;
using MyFirstApp.Services.Interfaces.PublisherService;

namespace MyFirstApp.Services.Interfaces
{
    public interface IExpenseService : IPublisher<ExpenseModel>
    {
        Task SubscribeAsync(Func<ExpenseModel, Task> handler);
        Task UpdateExpenseAsync(ExpenseModel expenseModel);
    }
}
