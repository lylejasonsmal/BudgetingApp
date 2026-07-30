using SQLite;
using MyFirstApp.Domain.Database;
using MyFirstApp.Domain.Helpers;
using MyFirstApp.Domain.Models;

namespace MyFirstApp.Services.Repositories
{
    public class SavingsPocketRepository
    {
        private readonly SqliteDatabaseService _dbService;
        private SQLiteAsyncConnection? _db;

        private List<SavingsPocketModel>? _cachedSavingsModels;

        public SavingsPocketRepository(SqliteDatabaseService dbService)
        {
            _dbService = dbService;
            Initialize();
        }

        private async void Initialize()
        {
            try
            {
                _db = await _dbService.GetConnectionAsync();
                await _db.CreateTableAsync<SavingsPocketModel>();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public async Task<SavingsPocketModel?> GetByIdAsync(int id)
        {
            return await TaskRunner.ExecuteAsync("Get Savings Pocket By Id", async () =>
            {
                _db ??= await _dbService.GetConnectionAsync();
                return _cachedSavingsModels?.FirstOrDefault(x=>x.Id == id) ?? await _db.FindAsync<SavingsPocketModel>(id);
            });
        }
        public async Task<SavingsPocketModel> AddAsync(SavingsPocketModel expense)
        {
            return await TaskRunner.ExecuteAsync("Delete Savings Pocket", async () =>
            {
                _db ??= await _dbService.GetConnectionAsync();
                var id = await _db.InsertAsync(expense);
                await InvalidateCacheAsync();
                return await _db.FindAsync<SavingsPocketModel>(id);
            });
        }

        public async Task UpdateAsync(SavingsPocketModel expense)
        {
            await TaskRunner.ExecuteAsync("Update Savings Pocket", async () =>
            {
                _db ??= await _dbService.GetConnectionAsync();
                await _db.UpdateAsync(expense);
                await InvalidateCacheAsync();
            });
        }

        public async Task DeleteAsync(SavingsPocketModel expense)
        {
            await TaskRunner.ExecuteAsync("Delete Savings Pocket", async () =>
            {
                _db ??= await _dbService.GetConnectionAsync();
                await _db.DeleteAsync(expense);
                await InvalidateCacheAsync();
            });
        }

        public async Task<List<int>> GetAll()
        {
            return await TaskRunner.ExecuteAsync("Load Savings Pockets", async () =>
            {
                _db ??= await _dbService.GetConnectionAsync();
                var savingPockets = _cachedSavingsModels ?? await _db.Table<SavingsPocketModel>().ToListAsync();
                _cachedSavingsModels = savingPockets;
                return savingPockets.Select(x => x.Id).ToList();
            });
        }
        private async Task InvalidateCacheAsync()
        {
            await TaskRunner.ExecuteAsync("Invalidate Expense Cache", () =>
            {
                _cachedSavingsModels = null;
                return Task.CompletedTask;
            });
        }
    }
}