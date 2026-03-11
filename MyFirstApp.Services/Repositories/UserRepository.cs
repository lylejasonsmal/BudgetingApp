using MyFirstApp.Domain.Database;
using MyFirstApp.Domain.Models;
using SQLite;

namespace MyFirstApp.Services.Repositories
{
    public class UserRepository
    {
        private readonly SqliteDatabaseService _dbService;
        private SQLiteAsyncConnection? _db;

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

        public async Task<List<UserModel>> GetAllAsync()
        {
            _db ??= await _dbService.GetConnectionAsync();
            return await _db.Table<UserModel>().ToListAsync();
        }

        public async Task<UserModel?> GetByIdAsync(int id)
        {
            _db ??= await _dbService.GetConnectionAsync();
            return await _db.FindAsync<UserModel>(id);
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
            var updatedUserId = await _db.UpdateAsync(user);
            return await GetByIdAsync(updatedUserId);
        }

        //public async Task DeleteAsync(UserModel user)
        //{
        //    _db ??= await _dbService.GetConnectionAsync();
        //    await _db.DeleteAsync(user);
        //}
    }
}
