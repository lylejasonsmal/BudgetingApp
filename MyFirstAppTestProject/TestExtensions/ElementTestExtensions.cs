using AngleSharp.Dom;
using NUnit.Framework;

namespace MyFirstAppTestProject.TestExtensions
{
    public static class ElementTestExtensions
    {
        public static void AssertInnerHtmlValue(this IElement element, string expectedValue)
        {
            Assert.That(element.InnerHtml, Is.EqualTo(expectedValue));
        }
    }
}
