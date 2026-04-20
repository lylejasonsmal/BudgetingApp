using MyFirstApp.Domain.Models;
using MyFirstApp.Domain.Values;
using MyFirstApp.Services.Interfaces.PublisherService;

namespace MyFirstApp.Services.Interfaces
{
    public interface IUserService : IPublisher<UserModel>
    {
        Task SubscribeAsync(Func<UserModel, Task> handler);
        Task<Result> UpdateUserAsync(UserModel userModel);
    }
}
