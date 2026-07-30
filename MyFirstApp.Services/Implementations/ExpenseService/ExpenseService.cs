using MyFirstApp.Domain.Helpers;
using MyFirstApp.Domain.Models;
using MyFirstApp.Domain.Values;
using MyFirstApp.Services.Implementations.PublisherService;
using MyFirstApp.Services.Interfaces;
using MyFirstApp.Services.Repositories;

namespace MyFirstApp.Services.Implementations.ExpenseService
{
    public class ExpenseService(
        ExpenseRepository expenseRepository,
        IMonthlyBudgetFixtureService monthlyBudgetFixtureService)
        : PublisherService<ExpenseModel>, IExpenseService
    {

        public async Task<Result> UpdateExpenseAsync(ExpenseModel expenseModel)
        {
            return await TaskRunner.ExecuteAsync("Update Expense", async () =>
            {
                var resultBuilder = Result.Builder();
                var isValid = true;
                if (expenseModel.TryValidate(out var error) is false)
                {
                    resultBuilder.WithError(error);
                    isValid = false;
                }

                var success =
                    await monthlyBudgetFixtureService.CalculateIfExpensesAreWithinBudgetAsync(expenseModel,
                        resultBuilder);

                if (success && isValid)
                {
                    await expenseRepository.UpdateAsync(expenseModel);
                    await monthlyBudgetFixtureService.RecalculateAndUpdateBudgetFixtureAsync();
                    await PublishAsync(expenseModel);
                }

                return resultBuilder.Create();
            });
        }

        public async Task<Result> CreateExpenseAsync(ExpenseModel expenseModel)
        {
            return await TaskRunner.ExecuteAsync("Create Expense", async () =>
            {
                var resultBuilder = Result.Builder();
                var isValid = true;
                if (expenseModel.TryValidate(out var error) is false)
                {
                    resultBuilder.WithError(error);
                    isValid = false;
                }

                var success =
                    await monthlyBudgetFixtureService.CalculateIfExpensesAreWithinBudgetAsync(expenseModel,
                        resultBuilder);

                if (success && isValid)
                {
                    await expenseRepository.AddAsync(expenseModel);
                    await monthlyBudgetFixtureService.RecalculateAndUpdateBudgetFixtureAsync();
                    await PublishAsync(expenseModel);
                }

                return resultBuilder.Create();
            });
        }

        public async Task DeleteExpenseAsync(ExpenseModel expenseModel)
        {
            await TaskRunner.ExecuteAsync("Delete Expense", async () =>
            {
                await expenseRepository.DeleteAsync(expenseModel);
                await monthlyBudgetFixtureService.RecalculateAndUpdateBudgetFixtureAsync();
            });
        }
    }
}