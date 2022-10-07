using Markdig;
using Markdig.Renderers;
using Markdig.Renderers.Html;
using Markdig.Syntax;
using SiteGen.Core.Models;
using SiteGen.Core.Models.Hierarchy;
using System.Text;

namespace SiteGen.Core.Extensions.Markdown
{
    public static class MarkdownDocumentExtensions
    {
        public static void ToPlainText(this MarkdownDocument document, TextWriter writer, MarkdownPipeline pipeline)
        {
            // We override the renderer with our own writer
            var renderer = new HtmlRenderer(writer)
            {
                EnableHtmlForBlock = false,
                EnableHtmlForInline = false,
                EnableHtmlEscape = false,
            };
            pipeline.Setup(renderer);

            renderer.Render(document);
            writer.Flush();
        }
        public static string ToPlainText(this MarkdownDocument document, MarkdownPipeline pipeline)
        {
            var sb = new StringBuilder();
            using var writer = new StringWriter(sb);
            ToPlainText(document, writer, pipeline );
            return sb.ToString();
        }

        public static IList<TocNode> ToTableOfContents(this MarkdownDocument document, int maxLevel = -1)
        {
            var results = new List<TocNode>();

            var stack = new Stack<TocNode>();

            var headings = document.Where(x => x is HeadingBlock).Cast<HeadingBlock>().ToList();

            foreach (var heading in headings)
            {
                var attributes = heading.TryGetAttributes();

                var content = heading.Inline?.Single().ToString();
                var entry = new TocNode(heading.Level, content, attributes?.Id);

                var node = stack.Count > 0 ? stack.Peek() : null;

                if( node == null)
                {
                    results.Add(entry);
                    stack.Push(entry);
                }
                else
                {
                    while( node != null && entry.Level < node.Level && stack.Count > 0)
                    {
                        node = stack.Pop();
                    }

                    if( node == null)
                    {
                        results.Add(entry);
                    }
                    else if( node.Level < entry.Level)
                    {
                        node.Tree.Children.Add(entry);
                    }
                    else if( node.Level == entry.Level)
                    {
                        if( node.Tree.Parent != null)
                        {
                            node.Tree.Parent.Tree.Children.Add(entry);
                        }
                        else
                        {
                            results.Add(entry);
                        }
                    }
                    else
                    {
                        results.Add(entry);
                    }

                    stack.Push(entry);
                }
            }

            return results;
        }
    }

    public class TocNode : INamedEntity, ITreeEntity<TocNode>
    {
        public TocNode(int level, string name, string? id)
        {
            Guid = Guid.NewGuid();
            Level = level;
            Name = name;
            Href = id;
            Tree = new TreeInfo<TocNode>(this);
        }

        public Guid Guid { get; set; }

        public int Level { get; set; }

        public string Name { get; set; }

        public string? Href { get; init; }

        public TreeInfo<TocNode> Tree { get; init; }
    }
}
