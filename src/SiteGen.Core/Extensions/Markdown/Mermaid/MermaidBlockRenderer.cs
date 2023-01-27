using Markdig.Renderers;
using Markdig.Renderers.Html;
using SiteGen.Core.Services;
using System.Diagnostics;

namespace SiteGen.Core.Extensions.Markdown.Mermaid;

public class MermaidBlockRenderer : HtmlObjectRenderer<MermaidBlock>
{
    private FileCacheProvider cache;

    static readonly TimeSpan timeout = TimeSpan.FromSeconds(10);

    public MermaidBlockRenderer(FileCacheProvider cache)
    {
        this.cache = cache;
    }

    protected override void Write(HtmlRenderer renderer, MermaidBlock obj)
    {
        renderer.EnsureLine();

        if (renderer.EnableHtmlForBlock)
        {
            renderer.Write("<div").WriteAttributes(obj).WriteLine(">");

            //  npx -p @mermaid-js/mermaid-cli mmdc
            var contents = obj.Lines.ToSlice().Text;

            var hash = Hasher.Md5Hash(contents);

            var filename = $"{hash}.mmd";

            using var temp = cache.GetTempFile(filename);
            using var svgFile = cache.GetTempFile($"{filename}.svg");

            File.WriteAllText(temp.FullName, contents);

            var startInfo = new ProcessStartInfo
            {
                FileName = "C:\\Program Files\\nodejs\\npx.cmd",
                Arguments = $"-p @mermaid-js/mermaid-cli mmdc -i \"{temp.FullName}\" -o \"{svgFile.FullName}\"",
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };

            var process = Process.Start(startInfo)!;
            process.WaitForExit((int)timeout.TotalMilliseconds);

            var output = process.StandardOutput.ReadToEnd();
            var error = process.StandardError.ReadToEnd();

            var svg = File.ReadAllText(svgFile.FullName);

            renderer.Write(svg);

            renderer.WriteLine("</div>");
        }
        else
        {
            renderer.WriteLeafRawLines(obj, true, renderer.EnableHtmlEscape);
        }
    }
}
