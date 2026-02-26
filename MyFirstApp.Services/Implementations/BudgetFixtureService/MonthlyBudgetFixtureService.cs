using MyFirstApp.Domain.Models;
using MyFirstApp.Services.Implementations.PublisherService;
using MyFirstApp.Services.Interfaces;
using MyFirstApp.Services.Repositories;

namespace MyFirstApp.Services.Implementations.BudgetFixtureService
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

        public async Task RecalculateAndUpdateBudgetFixtureAsync()
        {
            var currentMonthlyBudgetFixture = await GetCurrentFixtureAsync();

            //Initialize values for calculation
            currentMonthlyBudgetFixture.BudgetedForAmount = 0;
            currentMonthlyBudgetFixture.LeftOverAmount = currentMonthlyBudgetFixture.StoredNetSalary;

            foreach (var expense in await _expenseRepository.GetExpensesByFixtureIdAsync(currentMonthlyBudgetFixture.Id))
            {
                currentMonthlyBudgetFixture.BudgetedForAmount += expense.BudgetedForAmount;
                currentMonthlyBudgetFixture.LeftOverAmount -= expense.ActualAmount;
            }

            var updatedBudgetFixture = await _monthlyBudgetFixtureRepository.UpdateAsync(currentMonthlyBudgetFixture);

            await Publish(updatedBudgetFixture ?? throw new InvalidOperationException("Updated budget fixture is null"));
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