using SQLite;
using MyFirstApp.Domain.Database;
using MyFirstApp.Domain.Models;

namespace MyFirstApp.Services.Repositories
{
    public class MonthlyBudgetFixtureRepository
    {
        private readonly SqliteDatabaseService _dbService;
        private SQLiteAsyncConnection? _db;

        public MonthlyBudgetFixtureRepository(SqliteDatabaseService dbService)
        {
            _dbService = dbService;
            Initialize();
        }

        private async void Initialize()
        {
            try
            {
                _db = await _dbService.GetConnectionAsync();
                await _db.CreateTableAsync<MonthlyBudgetFixtureModel>();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public async Task<List<MonthlyBudgetFixtureModel>> GetAllAsync()
        {
            _db ??= await _dbService.GetConnectionAsync();
            return await _db.Table<MonthlyBudgetFixtureModel>().ToListAsync();
        }

        public async Task<MonthlyBudgetFixtureModel?> GetByIdAsync(int id)
        {
            _db ??= await _dbService.GetConnectionAsync();
            return await _db.FindAsync<MonthlyBudgetFixtureModel>(id);
        }

        public async Task<int?> GetCurrentFixtureIdAsync()
        {
            _db ??= await _dbService.GetConnectionAsync();

            var fixture = await _db.Table<MonthlyBudgetFixtureModel>()
                .Where(f => f.CurrentlyInUse)
                .FirstOrDefaultAsync();

            return fixture?.Id;
        }
        public async Task AddAsync(MonthlyBudgetFixtureModel budget)
        {
            _db ??= await _dbService.GetConnectionAsync();
            await _db.InsertAsync(budget);
        }

        public async Task<MonthlyBudgetFixtureModel?> UpdateAsync(MonthlyBudgetFixtureModel budgetFixture)
        {
            _db ??= await _dbService.GetConnectionAsync();
            var updatedBudgetFixtureId = await _db.InsertOrReplaceAsync(budgetFixture);
            return await GetByIdAsync(updatedBudgetFixtureId);
        }

        //public async Task DeleteAsync(MonthlyBudgetFixtureModel budget)
        //{
        //    _db ??= await _dbService.GetConnectionAsync();
        //    await _db.DeleteAsync(budget);
        //}
    }
}