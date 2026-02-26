using MyFirstAppComponentLibrary.UI.Components;
using NUnit.Framework;
using Bunit;
using Microsoft.AspNetCore.Components.Web;
using MyFirstAppTestProject.TestExtensions;

namespace MyFirstAppTestProject.ComponentTests.NavigationBarComponentTests
{
    public class NavigationTests : BaseComponentTest<NavigationBarComponent>
    {
        [Test]
        public async Task SHOULD_navigate_to_thread_when_clicked()
        {
            //Arrange
            ConstructSut();

            //Act
            await Sut.Find("#thread-navigation-link")
                .ClickAsync(new MouseEventArgs());

            //Assert
            FakeNavigation.VerifyNavigatedTo("/");
        }

        [Test]
        public async Task SHOULD_navigate_to_stats_when_clicked()
        {
            //Arrange
            ConstructSut();

            //Act
            await Sut.Find("#stats-navigation-link")
                .ClickAsync(new MouseEventArgs());

            //Assert
            FakeNavigation.VerifyNavigatedTo("/statistics-dashboard");
        }

        [Test]
        public async Task SHOULD_navigate_to_profile_when_clicked()
        {
            //Arrange
            ConstructSut();

            //Act
            await Sut.Find("#profile-navigation-link")
                .ClickAsync(new MouseEventArgs());

            //Assert
            FakeNavigation.VerifyNavigatedTo("/profile");
        }
    }
}
