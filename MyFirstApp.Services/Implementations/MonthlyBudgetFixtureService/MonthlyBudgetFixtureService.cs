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

            await PublishAsync(currentFixture);
        }

        public async Task<bool> CalculateIfExpensesAreWithinBudgetAsync(ExpenseModel expenseModel, ResultBuilder? resultBuilder)
        {
            var currentMonthlyBudgetFixture = await GetCurrentFixtureAsync();

            currentMonthlyBudgetFixture.BudgetedForAmount = 0;
            currentMonthlyBudgetFixture.LeftOverAmount = currentMonthlyBudgetFixture.StoredNetSalary;
            currentMonthlyBudgetFixture.NumberOfExpenses = 0;

            var expenses = await _expenseRepository.GetExpensesByFixtureIdAsync(currentMonthlyBudgetFixture.Id);

            var existingMatchingExpense = expenses.FirstOrDefault(x=>x.Id == expenseModel.Id);

            if (existingMatchingExpense is not null)
            {
                expenses.Remove(existingMatchingExpense);
            }

            foreach (var expense in expenses)
            {
                currentMonthlyBudgetFixture.BudgetedForAmount += expense.BudgetedForAmount;
                currentMonthlyBudgetFixture.LeftOverAmount -= expense.ActualAmount;
                currentMonthlyBudgetFixture.NumberOfExpenses += 1;
            }

            currentMonthlyBudgetFixture.BudgetedForAmount += expenseModel.BudgetedForAmount;
            currentMonthlyBudgetFixture.LeftOverAmount -= expenseModel.ActualAmount;
            currentMonthlyBudgetFixture.NumberOfExpenses += 1;

            if (currentMonthlyBudgetFixture.BudgetedForAmount > currentMonthlyBudgetFixture.StoredNetSalary)
            {
                resultBuilder?.WithError(
                    $"Your total budgeted for amount (R{currentMonthlyBudgetFixture.BudgetedForAmount}) is greater than your net salary (R{currentMonthlyBudgetFixture.StoredNetSalary}). Please correct either before proceeding.");
                return false;
            }

            return true;
        }

        public async Task RecalculateAndUpdateBudgetFixtureAsync()
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

            var updatedBudgetFixture = await _monthlyBudgetFixtureRepository.UpdateAsync(currentMonthlyBudgetFixture);

            await PublishAsync(updatedBudgetFixture ?? throw new InvalidOperationException("Updated budget fixture is null"));
        }

        public async Task<Result> TryUpdateBudget(MonthlyBudgetFixtureModel updatedBudgetFixtureModel)
        {
            var resultBuilder = Result.Builder();

            var currentMonthlyBudgetFixture = await GetCurrentFixtureAsync();

            if (currentMonthlyBudgetFixture is null)
            {
                await _monthlyBudgetFixtureRepository.UpdateAsync(updatedBudgetFixtureModel);
                await RecalculateAndUpdateBudgetFixtureAsync();
            }
            else
            {
                if (updatedBudgetFixtureModel.StoredNetSalary < currentMonthlyBudgetFixture?.BudgetedForAmount)
                {
                    resultBuilder.WithError("The net salary you've inputted is below your budgeted for amount. Please update your expenses before proceeding.");
                }
                else
                {
                    currentMonthlyBudgetFixture.StoredNetSalary = updatedBudgetFixtureModel.StoredNetSalary;
                    await _monthlyBudgetFixtureRepository.UpdateAsync(currentMonthlyBudgetFixture);
                    await RecalculateAndUpdateBudgetFixtureAsync();
                }
            }


            return resultBuilder.Create();
        }


        private async Task<MonthlyBudgetFixtureModel?> GetCurrentFixtureAsync()
        {
            var currentId = await _monthlyBudgetFixtureRepository.GetCurrentFixtureIdAsync();

            if (currentId.HasValue)
            {
                var fixture = await _monthlyBudgetFixtureRepository.GetByIdAsync(currentId.Value);
                if (fixture != null) return fixture;
            }

            return null;
        }
    }
}