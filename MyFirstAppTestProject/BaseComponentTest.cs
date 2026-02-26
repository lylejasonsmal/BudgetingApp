using Bunit;
using Bunit.TestDoubles;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using MyFirstApp.Domain.Database;
using MyFirstApp.Services.Repositories;
using NUnit.Framework;
using TestContext = Bunit.TestContext;

namespace MyFirstAppTestProject
{
    [TestFixture]
    public abstract class BaseComponentTest<TComponent> : TestContext
        where TComponent : IComponent
    {
        protected IRenderedComponent<TComponent> Sut = null!;

        protected FakeNavigationManager FakeNavigation = null!;

        // Delegate to set parameters
        protected Action<ComponentParameterCollectionBuilder<TComponent>>? Parameters { get; set; }

        [SetUp]
        public virtual void Setup()
        {
            Parameters = null;

            Services.AddSingleton<SqliteDatabaseService>();
            Services.AddSingleton<MonthlyBudgetFixtureRepository>();

            // Get the fake navigation manager from bUnit DI
            FakeNavigation = Services.GetRequiredService<FakeNavigationManager>();

        }

        public void ConstructSut()
        {
            Sut ??= RenderComponent<TComponent>(Parameters ?? (p => { }));
        }
    }
}