using Bunit.TestDoubles;
using NUnit.Framework;

namespace MyFirstAppTestProject.TestExtensions
{
    public static class FakeNavigationTestExtensions
    {
        public static void VerifyNavigatedTo(this FakeNavigationManager fakeNavigation, string relativeUrl)
        {
            Assert.That(fakeNavigation.Uri, Does.EndWith(relativeUrl));
        }
    }
}
