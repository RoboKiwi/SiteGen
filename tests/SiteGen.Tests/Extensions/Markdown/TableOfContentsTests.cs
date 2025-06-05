using Markdig;
using SiteGen.Core.Extensions.Markdown;

namespace SiteGen.Tests.UnitTests.Extensions.Markdown
{
    public class TableOfContentsTests
    {
        [TestMethod]
        public void TableOfContents()
        {
            var markup = @"# Heading 1

## Heading 2

## Heading 2

### Heading 3

##### Heading 5

## Heading 2

### Heading 3";

            var pipeline = new MarkdownPipelineBuilder()
            //.UseAdvancedExtensions()
            .Build();

            var doc = Markdig.Markdown.Parse(markup, pipeline);

            var toc = doc.ToTableOfContents();

            Assert.IsNotNull(toc);
        }
    }
}
