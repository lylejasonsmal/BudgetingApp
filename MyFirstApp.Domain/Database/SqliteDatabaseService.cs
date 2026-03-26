using SQLite;
using MyFirstApp.Domain.Models;
using Microsoft.Maui.Storage;

namespace MyFirstApp.Domain.Database;

public class SqliteDatabaseService
{
    private SQLiteAsyncConnection? _dbConnection;
    private bool _hasBeenSeeded;
    public SqliteDatabaseService() { }

    public async Task<SQLiteAsyncConnection> GetConnectionAsync()
    {
        if (_dbConnection != null)
        {
            await _dbConnection.CloseAsync();
        }

        string path = Path.Combine(FileSystem.AppDataDirectory, "budgetApp.sqlite");
        _dbConnection = new SQLiteAsyncConnection(path);

        await _dbConnection.CreateTableAsync<MonthlyBudgetFixtureModel>();
        await _dbConnection.CreateTableAsync<ExpenseModel>();
        await _dbConnection.CreateTableAsync<UserModel>();

        if (_hasBeenSeeded is false)
        {
            await SeedDatabaseAsync(_dbConnection);
        }

        return _dbConnection;
    }

    private async Task SeedDatabaseAsync(SQLiteAsyncConnection dbConnection)
    {
        var existingFixture = await dbConnection.Table<MonthlyBudgetFixtureModel>()
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

            fixtureId = await dbConnection.InsertAsync(fixture);
        }
        else
        {
            fixtureId = existingFixture.Id;
        }

        var existingUsers = await dbConnection.Table<UserModel>()
            .ToListAsync();

        if (existingUsers.Count == 0)
        {
            var user = new UserModel { Birthday = new DateTime(2004, 01, 26), Email = "lylesmal@gmail.com", FirstName = "Lyle", LastName = "Smal", Username = "lylejasonsmal", ProfileImage = null, Description = "I'm a software developer!"};

            await dbConnection.InsertAsync(user);
        }

        _hasBeenSeeded = true;
    }
}
