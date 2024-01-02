using Markdig.Renderers;
using Markdig.Renderers.Html;

namespace SiteGen.Extensions.Markdown.Prism;

public class PrismCodeBlockRenderer : HtmlObjectRenderer<PrismCodeBlock>
{
    readonly PrismHost host;

    public PrismCodeBlockRenderer(PrismHost host)
    {
        this.host = host;
    }

    protected override void Write(HtmlRenderer renderer, PrismCodeBlock obj)
    {
        renderer.EnsureLine();

        if (renderer.EnableHtmlForBlock)
        {
            // Get the code language, defaulting to text.
            var lang = obj.Info?.ToString() ?? "text";
            if (string.IsNullOrWhiteSpace(lang)) lang = "text";

            renderer.Write("<pre").WriteAttributes(obj).WriteLine(">");
            
            var contents = obj.Lines.ToString();

            try
            {
                renderer.Write("<code ").WriteAttributes(obj).WriteLine(">");
                var output = host.Highlight(contents, lang).GetAwaiter().GetResult();
                renderer.Write(output);
                renderer.WriteLine("</code>");
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
