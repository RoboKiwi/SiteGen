namespace SiteGen.Tests.Extensions;

[TestClass]
public class StringExtensionsTests
{
    [DataTestMethod]
    [DataRow("  This has four words. ", 4)]
    public void WordCount(string value, int expected)
    {
        Assert.AreEqual(value.AsSpan().WordCount(), expected);
    }
}