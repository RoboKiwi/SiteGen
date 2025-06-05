using Markdig.Renderers;
using Markdig.Renderers.Html;
using SiteGen.Core.Services;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace SiteGen.Core.Extensions.Markdown.Mermaid;

public class MermaidBlockRenderer : HtmlObjectRenderer<MermaidBlock>
{
    FileCacheProvider cache;

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
            try
            {
                renderer.Write("<div").WriteAttributes(obj).WriteLine(">");

                //  npx -p @mermaid-js/mermaid-cli mmdc
                var contents = obj.Lines.ToSlice().Text;

                var hash = Hasher.Md5Hash(contents);

                var filename = $"{hash}.mmd";

                using var temp = cache.GetTempFile(filename);
                using var svgFile = cache.GetTempFile($"{filename}.svg");

                File.WriteAllText(temp.FullName, contents);

                var command = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "npm.cmd" : "npm";

                var startInfo = new ProcessStartInfo
                {
                    FileName = command,
                    Arguments = $"run -- @mermaid-js/mermaid-cli mmdc -i \"{temp.FullName}\" -o \"{svgFile.FullName}\"",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                };

                var process = Process.Start(startInfo)!;
                process.WaitForExit((int)timeout.TotalMilliseconds);

                var output = process.StandardOutput.ReadToEnd();
                var error = process.StandardError.ReadToEnd();

                if(!string.IsNullOrWhiteSpace(error))
                {
                    renderer.Write("<pre><code>");
                    renderer.Write(error);
                    renderer.Write("</code></pre>");
                }
                else
                {
                    var svg = File.ReadAllText(svgFile.FullName);
                    renderer.Write(svg);
                }
            }
            catch(Exception ex)
            {
                renderer.Write("<pre><code>");
                renderer.Write(ex.ToString());
                renderer.Write("</code></pre>");
            }
            finally
            {
                renderer.WriteLine("</div>");
            }
        }
        else
        {
            renderer.WriteLeafRawLines(obj, true, renderer.EnableHtmlEscape);
        }
    }
}
