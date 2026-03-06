using MyFirstApp.Domain.Models;
using MyFirstApp.Services.Implementations.PublisherService;
using MyFirstApp.Services.Interfaces;
using MyFirstApp.Services.Repositories;

namespace MyFirstApp.Services.Implementations.ExpenseService
{
    public class ExpenseService : PublisherService<ExpenseModel>, IExpenseService
    {
        private readonly ExpenseRepository _expenseRepository;
        private readonly IMonthlyBudgetFixtureService _monthlyBudgetFixtureService;

        public ExpenseService(ExpenseRepository expenseRepository, IMonthlyBudgetFixtureService monthlyBudgetFixtureService)
        {
            _expenseRepository = expenseRepository;
            _monthlyBudgetFixtureService = monthlyBudgetFixtureService;
        }

        public async Task SubscribeAsync(Func<ExpenseModel, Task> handler)
        {
            Subscribe(handler);

            await Task.CompletedTask;
        }

        public async Task UpdateExpenseAsync(ExpenseModel expenseModel)
        {
            await _expenseRepository.UpdateAsync(expenseModel);
            await _monthlyBudgetFixtureService.RecalculateAndUpdateBudgetFixtureAsync();
            await Publish(expenseModel);
        }
    }
}