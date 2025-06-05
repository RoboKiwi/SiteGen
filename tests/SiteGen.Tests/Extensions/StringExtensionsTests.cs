using SiteGen.Core.Extensions;

namespace SiteGen.Tests.UnitTests.Extensions
{
    public class StringExtensionsTests
    {
        [DataTestMethod]
        [DataRow("  This has four words. ", 4)]
        public void WordCount(string value, int expected)
        {
            Assert.AreEqual(value.AsSpan().WordCount(), expected);
        }
    }
}
