using MyFirstApp.Domain.Database;
using MyFirstApp.Domain.Helpers;
using MyFirstApp.Domain.Models;
using SQLite;

namespace MyFirstApp.Services.Repositories
{
    public class UserRepository
    {
        private readonly SqliteDatabaseService _dbService;
        private SQLiteAsyncConnection? _db;
        private UserModel? _cachedUserModel;

        public UserRepository(SqliteDatabaseService dbService)
        {
            _dbService = dbService;
            Initialize();
        }

        private async void Initialize()
        {
            try
            {
                _db = await _dbService.GetConnectionAsync();
                await _db.CreateTableAsync<UserModel>();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public async Task<UserModel?> GetByIdAsync(int id)
        {
            return await TaskRunner.ExecuteAsync("Load User", async () =>
            {
                _db ??= await _dbService.GetConnectionAsync();
                var user = _cachedUserModel ?? await _db.FindAsync<UserModel>(id);
                _cachedUserModel = user;
                return user;
            });
        }

        //TODO: Move to the service
        //public async Task AddAsync(UserModel user)
        //{
        //    _db ??= await _dbService.GetConnectionAsync();
        //    await _db.InsertAsync(user);
        //}

        public async Task<UserModel?> UpdateAsync(UserModel user)
        {
            _db ??= await _dbService.GetConnectionAsync();
            await _db.UpdateAsync(user);
            return _cachedUserModel = user;
        }

        //public async Task DeleteAsync(UserModel user)
        //{
        //    _db ??= await _dbService.GetConnectionAsync();
        //    await _db.DeleteAsync(user);
        //}
    }
}
