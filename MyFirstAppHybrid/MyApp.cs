using Microsoft.Extensions.Logging;
using Microsoft.Maui.Networking;
using MyFirstApp.Domain.Database;
using MyFirstApp.Domain.Helpers;
using MyFirstApp.Services.Implementations.ConnectivityService;
using MyFirstApp.Services.Implementations.ExpenseService;
using MyFirstApp.Services.Implementations.GoogleCalendarService;
using MyFirstApp.Services.Implementations.MonthlyBudgetFixtureService;
using MyFirstApp.Services.Implementations.SavingsService;
using MyFirstApp.Services.Implementations.ToastService;
using MyFirstApp.Services.Implementations.UserService;
using MyFirstApp.Services.Interfaces;
using MyFirstApp.Services.Repositories;

namespace MyFirstAppHybrid
{
    public static class MyApp
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });

            builder.Services.AddMauiBlazorWebView();
            builder.Services.AddBlazorBootstrap();
            builder.Services.AddSingleton<IMonthlyBudgetFixtureService, MonthlyBudgetFixtureService>();
            builder.Services.AddSingleton<ISavingsService, SavingsService>();
            builder.Services.AddSingleton<IExpenseService, ExpenseService>();
            builder.Services.AddSingleton<IToastService, ToastService>();
            builder.Services.AddSingleton<IUserService, UserService>();
            builder.Services.AddSingleton(Connectivity.Current);
            builder.Services.AddSingleton<IConnectivityService, ConnectivityService>();
            builder.Services.AddSingleton(Launcher.Default);
            builder.Services.AddSingleton<IGoogleCalendarService, GoogleCalendarService>();
            builder.Services.AddSingleton<SqliteDatabaseService>();
            builder.Services.AddSingleton<MonthlyBudgetFixtureRepository>();
            builder.Services.AddSingleton<ExpenseRepository>();
            builder.Services.AddSingleton<UserRepository>();
            builder.Services.AddSingleton<SavingsPocketRepository>();
#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
    		builder.Logging.AddDebug();
#endif
            var app = builder.Build();

            var toastService = app.Services.GetRequiredService<IToastService>();
            TaskRunner.OnErrorAsync = message => toastService.ShowFailureToastAsync(message);

            var dbService = app.Services.GetRequiredService<SqliteDatabaseService>();
            _ = dbService.GetConnectionAsync();

            return app;

        }
    }
}
