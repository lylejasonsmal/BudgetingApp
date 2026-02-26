using Bunit;
using MyFirstAppTestProject.TestExtensions;
using NUnit.Framework;
using UI.Pages;

namespace MyFirstAppTestProject.PageTests.StatisticsDashboardPageTests
{
    public class OnRenderTests : BaseComponentTest<StatisticsDashboardPage>
    {
        [Test]
        public void SHOULD_render()
        {
            //Act
            ConstructSut();

            //Assert
            Assert.That(Sut.Markup, Is.Not.Empty);
            Sut.Find("#statistics-dashboard-heading").AssertInnerHtmlValue("Statistics");
        }

    }
}
