using Microsoft.Extensions.Logging;
using MyFirstApp.Domain.Database;
using MyFirstApp.Services.Implementations.ExpenseService;
using MyFirstApp.Services.Implementations.MonthlyBudgetFixtureService;
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
            builder.Services.AddSingleton<IMonthlyBudgetFixtureService, MonthlyBudgetFixtureService>();
            builder.Services.AddSingleton<IExpenseService, ExpenseService>();
            builder.Services.AddSingleton<SqliteDatabaseService>();
            builder.Services.AddSingleton<MonthlyBudgetFixtureRepository>();
            builder.Services.AddSingleton<ExpenseRepository>();
            builder.Services.AddSingleton<UserRepository>();

#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
    		builder.Logging.AddDebug();
#endif
            var app = builder.Build();

            var dbService = app.Services.GetRequiredService<SqliteDatabaseService>();
            _ = dbService.GetConnectionAsync();

            return app;

        }
    }
}
