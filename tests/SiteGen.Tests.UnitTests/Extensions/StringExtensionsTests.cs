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

        [Theory]
        [InlineData("StripstartThis is the rest", "This is the rest")]
        public void StripStart(string value, string expected)
        {
            Assert.Equal(value.AsSpan().StripStart("Stripstart").ToString(), expected);
        }
    }
}
