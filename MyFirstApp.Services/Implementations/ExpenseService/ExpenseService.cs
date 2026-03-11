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
            var originalExpense = await _expenseRepository.GetByIdAsync(expenseModel.Id);

            await _expenseRepository.UpdateAsync(expenseModel);
            var result = await _monthlyBudgetFixtureService.RecalculateAndUpdateBudgetFixtureAsync();

            if (result.Unsuccessful)
            {
                await _expenseRepository.UpdateAsync(originalExpense!);
                return new Result(ResultOutcome.Failure, result.Error);
            }

            await Publish(expenseModel);
            return new Result(ResultOutcome.Success);
        }

        public async Task<Result> CreateExpenseAsync(ExpenseModel expenseModel)
        {
            await _expenseRepository.AddAsync(expenseModel);
            var result = await _monthlyBudgetFixtureService.RecalculateAndUpdateBudgetFixtureAsync();

            if (result.Unsuccessful)
            {
                var originalExpense = await _expenseRepository.GetByIdAsync(expenseModel.Id);
                await _expenseRepository.DeleteAsync(originalExpense!);
                return new Result(ResultOutcome.Failure, result.Error);
            }

            await Publish(expenseModel);
            return new Result(ResultOutcome.Success);
        }

    }
}