using MyFirstApp.Domain.Helpers;
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
        private MonthlyBudgetFixtureModel? _budgetFixtureModel;

        public MonthlyBudgetFixtureService(MonthlyBudgetFixtureRepository monthlyBudgetFixtureRepository, ExpenseRepository expenseRepository)
        {
            _monthlyBudgetFixtureRepository = monthlyBudgetFixtureRepository;
            _expenseRepository = expenseRepository;
        }

        protected override async Task OnSubscribedAsync(Func<MonthlyBudgetFixtureModel, Task> handler)
        {
            await GetCurrentFixtureAsync();
        }

        public async Task<bool> CalculateIfExpensesAreWithinBudgetAsync(ExpenseModel expenseModel, ResultBuilder? resultBuilder)
        {
            return await TaskRunner.ExecuteAsync("Calculate If Expenses Are Within Budget", async () =>
            {
                _budgetFixtureModel!.BudgetedForAmount = 0;
                _budgetFixtureModel.LeftOverAmount = _budgetFixtureModel.StoredNetSalary;
                _budgetFixtureModel.NumberOfExpenses = 0;

                var expenses = await _expenseRepository.GetExpensesByFixtureIdAsync(_budgetFixtureModel.Id);

                var existingMatchingExpense = expenses.FirstOrDefault(x => x.Id == expenseModel.Id);

                if (existingMatchingExpense is not null)
                {
                    expenses.Remove(existingMatchingExpense);
                }

                foreach (var expense in expenses)
                {
                    _budgetFixtureModel.BudgetedForAmount += expense.BudgetedForAmount;
                    _budgetFixtureModel.LeftOverAmount -= expense.ActualAmount;
                    _budgetFixtureModel.NumberOfExpenses += 1;
                }

                _budgetFixtureModel.BudgetedForAmount += expenseModel.BudgetedForAmount;
                _budgetFixtureModel.LeftOverAmount -= expenseModel.ActualAmount;
                _budgetFixtureModel.NumberOfExpenses += 1;

                if (_budgetFixtureModel.BudgetedForAmount > _budgetFixtureModel.StoredNetSalary)
                {
                    resultBuilder?.WithError(
                        $"Your total budgeted for amount (R{_budgetFixtureModel.BudgetedForAmount}) is greater than your net salary (R{_budgetFixtureModel.StoredNetSalary}). Please correct either before proceeding.");
                    return false;
                }

                return true;
            });
        }

        public async Task RecalculateAndUpdateBudgetFixtureAsync()
        {
            await TaskRunner.ExecuteAsync("Recalculate & Update Budget", async () =>
            {
                if (_budgetFixtureModel is null) return;
                //Initialize values for calculation
                _budgetFixtureModel!.BudgetedForAmount = 0;
                _budgetFixtureModel.LeftOverAmount = _budgetFixtureModel.StoredNetSalary;
                _budgetFixtureModel.NumberOfExpenses = 0;

                foreach (var expense in await _expenseRepository.GetExpensesByFixtureIdAsync(_budgetFixtureModel.Id))
                {
                    _budgetFixtureModel.BudgetedForAmount += expense.BudgetedForAmount;
                    _budgetFixtureModel.LeftOverAmount -= expense.ActualAmount;
                    _budgetFixtureModel.NumberOfExpenses += 1;
                }

                var updatedBudgetFixture = await _monthlyBudgetFixtureRepository.UpdateAsync(_budgetFixtureModel);

                await PublishAsync(updatedBudgetFixture ??
                                   throw new InvalidOperationException("Updated budget fixture is null"));
            });
        }

        public async Task<Result> TryUpdateBudget(MonthlyBudgetFixtureModel updatedBudgetFixtureModel)
        {
            return await TaskRunner.ExecuteAsync("Update Budget", async () =>
            {
                var resultBuilder = Result.Builder();

                if (_budgetFixtureModel is null)
                {
                    await _monthlyBudgetFixtureRepository.UpdateAsync(updatedBudgetFixtureModel);
                    await RecalculateAndUpdateBudgetFixtureAsync();
                }
                else
                {
                    if (updatedBudgetFixtureModel.StoredNetSalary < _budgetFixtureModel?.BudgetedForAmount)
                    {
                        resultBuilder.WithError(
                            "The net salary you've inputted is below your budgeted for amount. Please update your expenses before proceeding.");
                    }
                    else
                    {
                        _budgetFixtureModel!.StoredNetSalary = updatedBudgetFixtureModel.StoredNetSalary;
                        await _monthlyBudgetFixtureRepository.UpdateAsync(_budgetFixtureModel);
                        await RecalculateAndUpdateBudgetFixtureAsync();
                    }
                }

                return resultBuilder.Create();
            });
        }


        public async Task<MonthlyBudgetFixtureModel?> GetCurrentFixtureAsync()
        {
            return await TaskRunner.ExecuteAsync("Get Current Budget Fixture", async () =>
            {
                if (_budgetFixtureModel is not null) return _budgetFixtureModel;

                var currentId = await _monthlyBudgetFixtureRepository.GetCurrentFixtureIdAsync();

                if (currentId.HasValue)
                {
                    _budgetFixtureModel = await _monthlyBudgetFixtureRepository.GetByIdAsync(currentId.Value);
                    if (_budgetFixtureModel != null) return _budgetFixtureModel;
                }

                return null;
            });
        }
    }
}