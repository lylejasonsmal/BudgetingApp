using Bunit;
using MyFirstAppComponentLibrary.UI.Components.Commonly_Used;
using NUnit.Framework;

namespace MyFirstAppTestProject.ComponentTests.CardWrapperComponentTests
{
    public class OnRenderTests : BaseComponentTest<CardWrapperComponent>
    {
        private string _expectedTitle = "Title";

        [Test]
        public void SHOULD_render_component()
        {
            //Arrange
            Parameters = p => p.Add(x => x.HeaderText, _expectedTitle)
                .Add(x => x.ChildContent, "<div>BodyContent</div>")
                .Add(x=>x.Link, "/")
                .Add(x => x.IconName, "gesture");

            //Act
            ConstructSut();

            //Assert
            Assert.That(Sut.Find("#card"), Is.Not.Null);
            Assert.That(Sut.Find("#card-header"), Is.Not.Null);
            Assert.That(Sut.Find("#title").InnerHtml, Is.EqualTo(_expectedTitle));
            Assert.That(Sut.Find("#card-body"), Is.Not.Null);
            Assert.That(Sut.Find("#card-body").InnerHtml, Does.Contain("BodyContent"));
            Assert.That(Sut.Find("#card-footer"), Is.Not.Null);
            Assert.That(Sut.FindComponents<MaterialDesignIconComponent>(), Has.Count.EqualTo(2));
        }

        [Test]
        public void WHEN_no_child_content_is_provided_SHOULD_not_render_card_body()
        {
            //Arrange
            Parameters = p => p.Add(x => x.HeaderText, _expectedTitle);

            //Act
            ConstructSut();

            //Assert
            Assert.That(Sut.Find("#card"), Is.Not.Null);
            Assert.That(Sut.Find("#card-header"), Is.Not.Null);
            Assert.That(Sut.Find("#title"), Is.Not.Null);
            Assert.That(Sut.Find("#title").InnerHtml, Is.EqualTo(_expectedTitle));
            Assert.That(Sut.FindAll("#card-body"), Is.Empty);
            Assert.That(Sut.FindAll("#card-footer"), Is.Empty);
        }

        [Test]
        public void WHEN_isLoading_SHOULD_show_loading_card()
        {
            //Arrange
            Parameters = p => p.Add(x => x.IsLoading, true);

            //Act
            ConstructSut();

            //Assert
            Assert.That(Sut.Find("#card"), Is.Not.Null);
            Assert.That(Sut.Markup, Does.Contain("shimmer"));
        }
    }
}
