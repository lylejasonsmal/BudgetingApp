using MyFirstApp.Domain.Models;
using MyFirstApp.Domain.Values;
using MyFirstApp.Services.Implementations.PublisherService;
using MyFirstApp.Services.Interfaces;
using MyFirstApp.Services.Repositories;

namespace MyFirstApp.Services.Implementations.MonthlyBudgetFixtureService
{
    public class MonthlyBudgetFixtureService : PublisherService<MonthlyBudgetFixtureModel>, IMonthlyBudgetFixtureService
    {
        private readonly MonthlyBudgetFixtureRepository _monthlyBudgetFixtureRepository; 
        private readonly ExpenseRepository _expenseRepository;

        public MonthlyBudgetFixtureService(MonthlyBudgetFixtureRepository monthlyBudgetFixtureRepository, ExpenseRepository expenseRepository)
        {
            _monthlyBudgetFixtureRepository = monthlyBudgetFixtureRepository;
            _expenseRepository = expenseRepository;
        }

        public async Task SubscribeAsync(Func<MonthlyBudgetFixtureModel, Task> handler)
        {
            Subscribe(handler);

            var currentFixture = await GetCurrentFixtureAsync();

            await Publish(currentFixture);
        }

        public async Task<Result> RecalculateAndUpdateBudgetFixtureAsync()
        {
            var currentMonthlyBudgetFixture = await GetCurrentFixtureAsync();

            //Initialize values for calculation
            currentMonthlyBudgetFixture.BudgetedForAmount = 0;
            currentMonthlyBudgetFixture.LeftOverAmount = currentMonthlyBudgetFixture.StoredNetSalary;
            currentMonthlyBudgetFixture.NumberOfExpenses = 0;

            foreach (var expense in await _expenseRepository.GetExpensesByFixtureIdAsync(currentMonthlyBudgetFixture.Id))
            {
                currentMonthlyBudgetFixture.BudgetedForAmount += expense.BudgetedForAmount;
                currentMonthlyBudgetFixture.LeftOverAmount -= expense.ActualAmount;
                currentMonthlyBudgetFixture.NumberOfExpenses += 1;
            }

            if (currentMonthlyBudgetFixture.BudgetedForAmount > currentMonthlyBudgetFixture.StoredNetSalary)
            {
                return new Result(ResultOutcome.Failure, $"Your total budgeted for amount (R{currentMonthlyBudgetFixture.BudgetedForAmount}) is greater than your net salary (R{currentMonthlyBudgetFixture.StoredNetSalary}). Please correct either before proceeding.");
            }

            var updatedBudgetFixture = await _monthlyBudgetFixtureRepository.UpdateAsync(currentMonthlyBudgetFixture);

            await Publish(updatedBudgetFixture ?? throw new InvalidOperationException("Updated budget fixture is null"));
            return new Result(ResultOutcome.Success);
        }


        private async Task<MonthlyBudgetFixtureModel> GetCurrentFixtureAsync()
        {
            var currentId = await _monthlyBudgetFixtureRepository.GetCurrentFixtureIdAsync();

            if (currentId.HasValue)
            {
                var fixture = await _monthlyBudgetFixtureRepository.GetByIdAsync(currentId.Value);
                if (fixture != null) return fixture;
            }

            return new MonthlyBudgetFixtureModel();
        }
    }
}