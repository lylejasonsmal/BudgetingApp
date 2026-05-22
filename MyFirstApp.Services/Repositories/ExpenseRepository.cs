using SQLite;
using MyFirstApp.Domain.Database;
using MyFirstApp.Domain.Enums;
using MyFirstApp.Domain.Models;

namespace MyFirstApp.Services.Repositories
{
    public class ExpenseRepository
    {
        private readonly SqliteDatabaseService _dbService;
        private SQLiteAsyncConnection? _db;

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

        public async Task<List<int>> GetExpenseIdsByFixtureIdAsync(int? budgetFixtureId)
        {
            _db ??= await _dbService.GetConnectionAsync();
            var expenses = await _db.Table<ExpenseModel>()
                .Where(x => x.MonthlyBudgetFixtureId == budgetFixtureId)
                .ToListAsync();

            var ids = expenses.Select(x => x.Id).ToList(); 

            return ids;
        }

        public async Task<List<int>> ApplyFiltersAsync(int? budgetFixtureId, IList<ExpenseFilters> filters)
        {
            _db ??= await _dbService.GetConnectionAsync();
            var expenses = await _db.Table<ExpenseModel>()
                .Where(x => x.MonthlyBudgetFixtureId == budgetFixtureId)
                .ToListAsync();



            if (filters.Contains(ExpenseFilters.AlphabeticalOrder))
            {
                expenses = expenses.OrderBy(x => x.ExpenseName).ToList();
            }
            if (filters.Contains(ExpenseFilters.ReverseAlphabeticalOrder))
            {
                expenses = expenses.OrderByDescending(x => x.ExpenseName).ToList();
            }
            if (filters.Contains(ExpenseFilters.PaidExpensesFirst))
            {
                expenses = expenses.OrderByDescending(x => x.IsPaidFor).ToList();
            }
            if (filters.Contains(ExpenseFilters.PaidExpensesLast))
            {
                expenses = expenses.OrderBy(x => x.IsPaidFor).ToList();
            }
            if (filters.Contains(ExpenseFilters.PaidExpensesOnly))
            {
                expenses = expenses.Where(x => x.IsPaidFor).ToList();
            }
            if (filters.Contains(ExpenseFilters.UnpaidExpensesOnly))
            {
                expenses = expenses.Where(x => !x.IsPaidFor).ToList();
            }

            var ids = expenses.Select(x => x.Id).ToList();

            return ids;
        }

        public async Task<List<ExpenseModel>> GetExpensesByFixtureIdAsync(int? budgetFixtureId)
        {
            _db ??= await _dbService.GetConnectionAsync();

            return await _db.Table<ExpenseModel>()
                .Where(x => x.MonthlyBudgetFixtureId == budgetFixtureId)
                .ToListAsync();
        }

        public async Task<ExpenseModel?> GetByIdAsync(int id)
        {
            _db ??= await _dbService.GetConnectionAsync();
            return await _db.FindAsync<ExpenseModel>(id);
        }

        public async Task<int> GetPaidExpenseCount(int? budgetFixtureId)
        {
            _db ??= await _dbService.GetConnectionAsync();
            var paidExpenses = await _db.Table<ExpenseModel>()
                .Where(x => x.MonthlyBudgetFixtureId == budgetFixtureId)
                .Where(x=>x.IsPaidFor)
                .ToListAsync();

            return paidExpenses.Count;
        }

        public async Task<int> GetOverBudgetExpenseCount(int? budgetFixtureId)
        {
            _db ??= await _dbService.GetConnectionAsync();
            var overBudgetExpenses = await _db.Table<ExpenseModel>()
                .Where(x => x.MonthlyBudgetFixtureId == budgetFixtureId)
                .Where(x => x.ActualAmount > x.BudgetedForAmount)
                .ToListAsync();

            return overBudgetExpenses.Count;
        }
        public async Task AddAsync(ExpenseModel expense)
        {
            _db ??= await _dbService.GetConnectionAsync();
            await _db.InsertAsync(expense);
        }

        public async Task UpdateAsync(ExpenseModel expense)
        {
            _db ??= await _dbService.GetConnectionAsync();
            await _db.UpdateAsync(expense);
        }

        public async Task DeleteAsync(ExpenseModel expense)
        {
            _db ??= await _dbService.GetConnectionAsync();
            await _db.DeleteAsync(expense);
        }
    }
}