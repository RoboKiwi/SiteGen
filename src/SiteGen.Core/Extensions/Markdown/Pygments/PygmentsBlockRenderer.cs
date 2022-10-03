using Markdig.Renderers;
using Markdig.Renderers.Html;
using System.Diagnostics;

namespace SiteGen.Core.Extensions.Markdown.Pygments;

public class PygmentsBlockRenderer : HtmlObjectRenderer<PygmentsBlock>
{
    protected override void Write(HtmlRenderer renderer, PygmentsBlock obj)
    {
        renderer.EnsureLine();

        if (renderer.EnableHtmlForBlock)
        {
            renderer.Write("<div").WriteAttributes(obj).WriteLine(">");

            var contents = obj.Lines.ToString();

            var lang = obj.Info?.ToString() ?? "text";

            if (string.IsNullOrWhiteSpace(lang)) lang = "text";

            var startInfo = new ProcessStartInfo
            {
                FileName = "pygmentize",
                Arguments = $"-l {lang} -f html",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                RedirectStandardInput = true
            };

            var process = Process.Start(startInfo)!;

            process.StandardInput.WriteLine(contents);
            process.StandardInput.Flush();
            process.StandardInput.Close();

            process.WaitForExit(2000);

            var output = process.StandardOutput.ReadToEnd();
            var error = process.StandardError.ReadToEnd();

            if (!string.IsNullOrWhiteSpace(error))
            {
                renderer.Write("<pre>");
                renderer.Write(error);
                renderer.Write("</pre>");
            }

            renderer.Write(output);

            renderer.WriteLine("</div>");
        }
        else
        {
            renderer.WriteLeafRawLines(obj, true, renderer.EnableHtmlEscape);
        }
    }
}
