using Markdig.Renderers;
using Markdig.Renderers.Html;

namespace SiteGen.Extensions.Markdown.Monaco;

public class MonacoCodeBlockRenderer(MonacoHost host) : HtmlObjectRenderer<MonacoCodeBlock>
{
    readonly MonacoHost host = host;

    protected override void Write(HtmlRenderer renderer, MonacoCodeBlock obj)
    {
        renderer.EnsureLine();

        if (renderer.EnableHtmlForBlock)
        {
            // Get the code language, defaulting to text.
            var lang = obj.Info?.ToString() ?? "text";
            if (string.IsNullOrWhiteSpace(lang)) lang = "text";

            renderer.Write("<pre").WriteAttributes(obj).Write(">");

            try
            {
                var contents = obj.Lines.ToString();
                var output = host.Highlight(contents, lang).GetAwaiter().GetResult();

                renderer.Write("<code>");
                renderer.Write(output);
                renderer.Write("</code>");
            }
            catch (Exception ex)
            {
                renderer.Write("<code>");
                renderer.Write(ex.ToString());
                renderer.Write("</code>");
            }
            finally
            {
                renderer.WriteLine("</pre>");
            }
        }
        else
        {
            renderer.WriteLeafRawLines(obj, true, renderer.EnableHtmlEscape);
        }
    }
}
