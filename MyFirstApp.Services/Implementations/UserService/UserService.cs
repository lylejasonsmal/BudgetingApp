using MyFirstApp.Domain.Models;
using MyFirstApp.Domain.Values;
using MyFirstApp.Services.Implementations.PublisherService;
using MyFirstApp.Services.Interfaces;
using MyFirstApp.Services.Repositories;

namespace MyFirstApp.Services.Implementations.UserService
{
    public class UserService : PublisherService<UserModel>, IUserService
    {
        private readonly UserRepository _userRepository;

        public UserService(UserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task SubscribeAsync(Func<UserModel, Task> handler)
        {
            Subscribe(handler);

            await Task.CompletedTask;
        }

        public async Task<Result> UpdateUserAsync(UserModel userModel)
        {
            var resultBuilder = Result.Builder();
            //var isValid = true;
            //if (expenseModel.TryValidate(out var error) is false)
            //{
            //    resultBuilder.WithError(error);
            //    isValid = false;
            //}

            await _userRepository.UpdateAsync(userModel);
            await PublishAsync(userModel);

            return resultBuilder.Create();
        }

    }
}