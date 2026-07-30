using MyFirstApp.Domain.Models;
using MyFirstApp.Services.Implementations.PublisherService;
using MyFirstApp.Services.Interfaces;
using MyFirstApp.Services.Repositories;

namespace MyFirstApp.Services.Implementations.SavingsService
{
    public class SavingsService(SavingsPocketRepository savingsPocketRepository)
        : PublisherService<IReadOnlyList<SavingsPocketModel>>, ISavingsService
    {
        protected override async Task OnSubscribedAsync(Func<IReadOnlyList<SavingsPocketModel>, Task> handler)
        {
            await handler(await GetAllAsync());
        }

        public async Task<IReadOnlyList<SavingsPocketModel>> GetAllAsync()
        {
            var ids = await savingsPocketRepository.GetAll();

            var pockets = new List<SavingsPocketModel>();
            foreach (var id in ids)
            {
                var pocket = await savingsPocketRepository.GetByIdAsync(id);
                if (pocket is not null) pockets.Add(pocket);
            }

            return pockets;
        }

        public Task<SavingsPocketModel?> GetByIdAsync(int id)
        {
            return savingsPocketRepository.GetByIdAsync(id);
        }

        public async Task AddAsync(SavingsPocketModel savingsPocketModel)
        {
            await savingsPocketRepository.AddAsync(savingsPocketModel);
            await PublishCurrentAsync();
        }

        public async Task UpdateAsync(SavingsPocketModel savingsPocketModel)
        {
            await savingsPocketRepository.UpdateAsync(savingsPocketModel);
            await PublishCurrentAsync();
        }

        public async Task DeleteAsync(SavingsPocketModel savingsPocketModel)
        {
            await savingsPocketRepository.DeleteAsync(savingsPocketModel);
            await PublishCurrentAsync();
        }

        private async Task PublishCurrentAsync()
        {
            await PublishAsync(await GetAllAsync());
        }
    }
}
