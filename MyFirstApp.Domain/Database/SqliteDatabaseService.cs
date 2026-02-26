using SQLite;
using MyFirstApp.Domain.Models;
using Microsoft.Maui.Storage;

namespace MyFirstApp.Domain.Database;

public class SqliteDatabaseService
{
    private SQLiteAsyncConnection? _db;

    public SqliteDatabaseService() { }

    public async Task<SQLiteAsyncConnection> GetConnectionAsync()
    {
        if (_db != null)
            return _db;

        string path = Path.Combine(FileSystem.AppDataDirectory, "budgetApp.sqlite");
        _db = new SQLiteAsyncConnection(path);

        await _db.CreateTableAsync<MonthlyBudgetFixtureModel>();
        await _db.CreateTableAsync<ExpenseModel>();

        await SeedDatabaseAsync(_db);

        return _db;
    }

    private async Task SeedDatabaseAsync(SQLiteAsyncConnection db)
    {
        var existingFixture = await db.Table<MonthlyBudgetFixtureModel>()
                                      .Where(f => f.Month == "January" && f.Year == "2026")
                                      .FirstOrDefaultAsync();

        int fixtureId;

        if (existingFixture == null)
        {
            var fixture = new MonthlyBudgetFixtureModel
            {
                Month = "January",
                Year = "2026",
                NumberOfExpenses = 5,
                StoredNetSalary = 25000,
                BudgetedForAmount = 18000,
                LeftOverAmount = 7000,
                CurrentlyInUse = true
            };

            fixtureId = await db.InsertAsync(fixture);
        }
        else
        {
            fixtureId = existingFixture.Id;
        }

        var existingExpenses = await db.Table<ExpenseModel>()
                                       .Where(e => e.MonthlyBudgetFixtureId == fixtureId)
                                       .ToListAsync();

        if (existingExpenses.Count == 0)
        {
            var expenses = new[]
            {
                new ExpenseModel { ExpenseName = "Rent", BudgetedForAmount = 8000, ActualAmount = 8000, IsPaidFor = true, IsARecurringExpense = true, MonthlyBudgetFixtureId = fixtureId, Notes = "Put aside money for other expenses."},
                new ExpenseModel { ExpenseName = "Groceries", BudgetedForAmount = 3000, ActualAmount = 2750, IsPaidFor = true, IsARecurringExpense = true, MonthlyBudgetFixtureId = fixtureId,  Notes = "Less luxuries, more protein."},
                new ExpenseModel { ExpenseName = "Electricity", BudgetedForAmount = 1500, ActualAmount = 1400, IsPaidFor = true, IsARecurringExpense = true, MonthlyBudgetFixtureId = fixtureId },
                new ExpenseModel { ExpenseName = "Transport", BudgetedForAmount = 1200, ActualAmount = 1100, IsPaidFor = true, IsARecurringExpense = true, MonthlyBudgetFixtureId = fixtureId },
                new ExpenseModel { ExpenseName = "Entertainment", BudgetedForAmount = 800, ActualAmount = 600, IsPaidFor = false, IsARecurringExpense = false, MonthlyBudgetFixtureId = fixtureId }
            };

            await db.InsertAllAsync(expenses);
        }
    }
}
