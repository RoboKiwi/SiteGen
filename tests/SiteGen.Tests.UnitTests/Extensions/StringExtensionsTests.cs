using SiteGen.Core.Extensions;

namespace SiteGen.Tests.UnitTests.Extensions
{
    public class StringExtensionsTests
    {
        [Theory]
        [InlineData("  This has four words. ", 4)]
        public void WordCount(string value, int expected)
        {
            Assert.Equal(value.AsSpan().WordCount(), expected);
        }
    }
}
