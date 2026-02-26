using Bunit;
using MyFirstAppComponentLibrary.UI.Components;
using NUnit.Framework;
using UI.Pages;

namespace MyFirstAppTestProject.PageTests.ThreadPageTests
{
    public class OnRenderTests : BaseComponentTest<ThreadPage>
    {
        [Test]
        public void SHOULD_render()
        {
            //Act
            ConstructSut();

            //Assert
            Assert.That(Sut.Markup, Is.Not.Empty);
            var budgetCard = Sut.FindComponent<BudgetFixtureComponent>();
            Assert.That(budgetCard, Is.Not.Null);
            var expenseCard = Sut.FindComponents<BudgetFixtureComponent>();
            Assert.That(budgetCard, Is.Not.Null);
        }

    }
}
