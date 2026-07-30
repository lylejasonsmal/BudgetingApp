using System.Collections.Concurrent;
using SQLite;
using MyFirstApp.Domain.Database;
using MyFirstApp.Domain.Enums;
using MyFirstApp.Domain.Helpers;
using MyFirstApp.Domain.Models;

namespace MyFirstApp.Services.Repositories
{
    public class ExpenseRepository
    {
        private readonly SqliteDatabaseService _dbService;
        private SQLiteAsyncConnection? _db;

        private readonly ConcurrentDictionary<int, List<ExpenseModel>> _cachedExpenseModels = [];

        public ExpenseRepository(SqliteDatabaseService dbService)
        {
            _dbService = dbService;
            Initialize();
        }

        private async void Initialize()
        {
            try
            {
                _db = await _dbService.GetConnectionAsync();
                await _db.CreateTableAsync<ExpenseModel>();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public async Task<List<int>> LoadExpensesWithFiltersAppliedAsync(int budgetFixtureId, IList<ExpenseFilter> filters)
        {
            return await TaskRunner.ExecuteAsync("Load Filtered Expenses", async () =>
            {
                _db ??= await _dbService.GetConnectionAsync();
                var expenses = _cachedExpenseModels.TryGetValue(budgetFixtureId, out var list) ? list : await _db.Table<ExpenseModel>()
                    .Where(x => x.MonthlyBudgetFixtureId == budgetFixtureId)
                    .ToListAsync();

                _cachedExpenseModels.TryAdd(budgetFixtureId, expenses);

                if (filters.Count > 0)
                {
                    if (filters.Contains(ExpenseFilter.AlphabeticalOrder))
                    {
                        expenses = expenses.OrderBy(x => x.ExpenseName).ToList();
                    }

                    if (filters.Contains(ExpenseFilter.ReverseAlphabeticalOrder))
                    {
                        expenses = expenses.OrderByDescending(x => x.ExpenseName).ToList();
                    }

                    if (filters.Contains(ExpenseFilter.PaidExpensesFirst))
                    {
                        expenses = expenses.OrderByDescending(x => x.IsPaidFor).ToList();
                    }

                    if (filters.Contains(ExpenseFilter.PaidExpensesLast))
                    {
                        expenses = expenses.OrderBy(x => x.IsPaidFor).ToList();
                    }

                    if (filters.Contains(ExpenseFilter.PaidExpensesOnly))
                    {
                        expenses = expenses.Where(x => x.IsPaidFor).ToList();
                    }

                    if (filters.Contains(ExpenseFilter.UnpaidExpensesOnly))
                    {
                        expenses = expenses.Where(x => !x.IsPaidFor).ToList();
                    }
                }

                var ids = expenses.Select(x => x.Id).ToList();

                return ids;
            });
        }

        public async Task<List<ExpenseModel>> GetExpensesByFixtureIdAsync(int budgetFixtureId)
        {
            return await TaskRunner.ExecuteAsync("Load Expenses", async () =>
            {
                _db ??= await _dbService.GetConnectionAsync();

                var expenses = _cachedExpenseModels.TryGetValue(budgetFixtureId, out var list) ? list : await _db.Table<ExpenseModel>()
                    .Where(x => x.MonthlyBudgetFixtureId == budgetFixtureId)
                    .ToListAsync();

                _cachedExpenseModels.TryAdd(budgetFixtureId, expenses);

                return expenses;
            });
        }

        public async Task<ExpenseModel?> GetByIdAsync(int id)
        {
            return await TaskRunner.ExecuteAsync("Load Expense By Id", async () =>
            {
                _db ??= await _dbService.GetConnectionAsync();
                return _cachedExpenseModels.Values.SelectMany(x => x).FirstOrDefault(x => x.Id == id) ?? await _db.FindAsync<ExpenseModel>(id);
            });
        }

        public async Task<int> GetPaidExpenseCount(int budgetFixtureId)
        {
            _db ??= await _dbService.GetConnectionAsync();

            if(_cachedExpenseModels.TryGetValue(budgetFixtureId, out var list)) return list.Count(x => x.IsPaidFor);

            return await _db.Table<ExpenseModel>()
                .Where(x => x.MonthlyBudgetFixtureId == budgetFixtureId)
                .CountAsync(x => x.IsPaidFor);
        }

        public async Task<int> GetOverBudgetExpenseCount(int budgetFixtureId)
        {
            _db ??= await _dbService.GetConnectionAsync();

            if (_cachedExpenseModels.TryGetValue(budgetFixtureId, out var list)) return list.Count(x => x.ActualAmount > x.BudgetedForAmount);

            return await _db.Table<ExpenseModel>()
                .Where(x => x.MonthlyBudgetFixtureId == budgetFixtureId)
                .CountAsync(x => x.ActualAmount > x.BudgetedForAmount);
        }
        public async Task AddAsync(ExpenseModel expense)
        {
            _db ??= await _dbService.GetConnectionAsync();
            await _db.InsertAsync(expense);
            await InvalidateCacheAsync();
        }

        public async Task UpdateAsync(ExpenseModel expense)
        {
            _db ??= await _dbService.GetConnectionAsync();
            await _db.UpdateAsync(expense);
            await InvalidateCacheAsync();
        }

        public async Task DeleteAsync(ExpenseModel expense)
        {
            _db ??= await _dbService.GetConnectionAsync();
            await _db.DeleteAsync(expense);
            await InvalidateCacheAsync();
        }

        private async Task InvalidateCacheAsync()
        {
            await TaskRunner.ExecuteAsync("Invalidate Expense Cache", () =>
            {
                _cachedExpenseModels.Clear();
                return Task.CompletedTask;
            });
        }
    }
}