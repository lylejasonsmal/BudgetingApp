using MyFirstApp.Domain.Models;
using MyFirstApp.Domain.Values;
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

        public async Task<Result> UpdateExpenseAsync(ExpenseModel expenseModel)
        {
            var resultBuilder = Result.Builder();
            var isValid = true;
            if (expenseModel.TryValidate(out var error) is false)
            {
                resultBuilder.WithError(error);
                isValid = false;
            }

            var success = await _monthlyBudgetFixtureService.CalculateIfExpensesAreWithinBudgetAsync(expenseModel, resultBuilder);

            if (success && isValid)
            {
                await _expenseRepository.UpdateAsync(expenseModel);
                await _monthlyBudgetFixtureService.RecalculateAndUpdateBudgetFixtureAsync();
                await PublishAsync(expenseModel);
            }

            return resultBuilder.Create();
        }

        public async Task<Result> CreateExpenseAsync(ExpenseModel expenseModel)
        {
            var resultBuilder = Result.Builder();
            var isValid = true;
            if (expenseModel.TryValidate(out var error) is false)
            {
                resultBuilder.WithError(error);
                isValid = false;
            }
            var success = await _monthlyBudgetFixtureService.CalculateIfExpensesAreWithinBudgetAsync(expenseModel, resultBuilder);

            if (success && isValid)
            {
                await _expenseRepository.AddAsync(expenseModel);
                await _monthlyBudgetFixtureService.RecalculateAndUpdateBudgetFixtureAsync();
                await PublishAsync(expenseModel);
            }

            return resultBuilder.Create();
        }

        public async Task DeleteExpenseAsync(ExpenseModel expenseModel)
        {
            await _expenseRepository.DeleteAsync(expenseModel);
            await _monthlyBudgetFixtureService.RecalculateAndUpdateBudgetFixtureAsync();
        }
    }
}