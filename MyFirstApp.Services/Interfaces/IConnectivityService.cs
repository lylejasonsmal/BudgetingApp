using MyFirstApp.Services.Interfaces.PublisherService;

namespace MyFirstApp.Services.Interfaces
{
    public interface IConnectivityService : IPublisher<bool>
    {
        bool IsConnected { get; }
        Task<bool> CheckConnectivityAsync();
    }
}
