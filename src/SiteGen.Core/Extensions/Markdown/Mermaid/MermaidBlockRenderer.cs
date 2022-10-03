using Markdig.Renderers;
using Markdig.Renderers.Html;
using System.Diagnostics;

namespace SiteGen.Core.Extensions.Markdown.Mermaid;

public class MermaidBlockRenderer : HtmlObjectRenderer<MermaidBlock>
{
    protected override void Write(HtmlRenderer renderer, MermaidBlock obj)
    {
        renderer.EnsureLine();

        if (renderer.EnableHtmlForBlock)
        {
            renderer.Write("<div").WriteAttributes(obj).WriteLine(">");

            //  npx -p @mermaid-js/mermaid-cli mmdc
            var contents = obj.Lines.ToSlice().Text;
            File.WriteAllText("mermaid.mmd", contents);

            var startInfo = new ProcessStartInfo
            {
                FileName = "C:\\Program Files\\nodejs\\npx.cmd",
                Arguments = "-p @mermaid-js/mermaid-cli mmdc -i mermaid.mmd -o mermaid.svg",
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };

            var process = Process.Start(startInfo)!;
            process.WaitForExit();

            var output = process.StandardOutput.ReadToEnd();
            var error = process.StandardError.ReadToEnd();

            var svg = File.ReadAllText("mermaid.svg");

            renderer.Write(svg);

            renderer.WriteLine("</div>");
        }
        else
        {
            renderer.WriteLeafRawLines(obj, true, renderer.EnableHtmlEscape);
        }
    }
}
