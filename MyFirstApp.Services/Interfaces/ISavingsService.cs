using MyFirstApp.Domain.Models;
using MyFirstApp.Services.Interfaces.PublisherService;

namespace MyFirstApp.Services.Interfaces
{
    public interface ISavingsService : IPublisher<IReadOnlyList<SavingsPocketModel>>
    {
        Task<IReadOnlyList<SavingsPocketModel>> GetAllAsync();
        Task<SavingsPocketModel?> GetByIdAsync(int id);
        Task AddAsync(SavingsPocketModel savingsPocketModel);
        Task UpdateAsync(SavingsPocketModel savingsPocketModel);
        Task DeleteAsync(SavingsPocketModel savingsPocketModel);
    }
}
