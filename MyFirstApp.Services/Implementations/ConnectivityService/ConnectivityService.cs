using Microsoft.Maui.Networking;
using MyFirstApp.Services.Implementations.PublisherService;
using MyFirstApp.Services.Interfaces;

namespace MyFirstApp.Services.Implementations.ConnectivityService
{
    public class ConnectivityService : PublisherService<bool>, IConnectivityService
    {
        // The same lightweight endpoint Android uses for its own captive-portal check; returns 204 No Content.
        private static readonly Uri ProbeUri = new("https://www.gstatic.com/generate_204");
        private static readonly TimeSpan PollInterval = TimeSpan.FromSeconds(10);

        private readonly IConnectivity _connectivity;
        private readonly HttpClient _httpClient;

        public ConnectivityService(IConnectivity connectivity)
        {
            _connectivity = connectivity;
            _httpClient = new HttpClient { Timeout = TimeSpan.FromSeconds(5) };
            _connectivity.ConnectivityChanged += async (_, _) => await CheckConnectivityAsync();
            _ = MonitorAsync();
        }

        public bool IsConnected { get; private set; }

        public async Task SubscribeAsync(Func<bool, Task> handler)
        {
            Subscribe(handler);
            await Task.CompletedTask;
        }

        public async Task<bool> CheckConnectivityAsync()
        {
            var isConnected = await HasInternetAsync();
            if (isConnected != IsConnected)
            {
                IsConnected = isConnected;
                await PublishAsync(isConnected);
            }
            return isConnected;
        }

        // NetworkAccess only tells us the device is attached to a network that should have internet,
        // not that the internet is actually reachable, so we confirm with an active probe.
        private async Task<bool> HasInternetAsync()
        {
            if (_connectivity.NetworkAccess != NetworkAccess.Internet)
                return false;

            try
            {
                using var response = await _httpClient.GetAsync(ProbeUri);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        // Poll so we also catch the internet dropping while the device stays attached to Wi-Fi,
        // which does not raise ConnectivityChanged.
        private async Task MonitorAsync()
        {
            using var timer = new PeriodicTimer(PollInterval);
            do
            {
                await CheckConnectivityAsync();
            }
            while (await timer.WaitForNextTickAsync());
        }
    }
}
