using Bunit;
using MyFirstAppComponentLibrary.UI.Components;
using MyFirstAppTestProject.TestExtensions;
using NUnit.Framework;

namespace MyFirstAppTestProject.ComponentTests.NavigationBarComponentTests
{
    public class OnRenderTests : BaseComponentTest<NavigationBarComponent>
    {
        [Test]
        public void SHOULD_render_component()
        {
            //Act
            ConstructSut();

            //Assert
            Assert.That(Sut.Find("#navigation-bar"), Is.Not.Null);
            Sut.Find("#thread-navigation-link").AssertInnerHtmlValue("Thread");
            Sut.Find("#stats-navigation-link").AssertInnerHtmlValue("Statistics");
            Sut.Find("#profile-navigation-link").AssertInnerHtmlValue("Profile");
        }
    }
}
