using Microsoft.AspNetCore.Mvc;
using SiteGen.Core.Formatters.SiteMapXml;
using SiteGen.Core.Services;
using SiteGen.Core.Services.Generators;
using SiteGen.Core.Services.Processors;
using System.Text;
using System.Xml;

namespace SiteGen.Core.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ISiteMapBuilder graph;
    private readonly MarkdownProcessor processor;
    private readonly IServiceProvider services;
    private readonly IList<ISiteNodeProcessor> processors;

    public HomeController(ILogger<HomeController> logger, ISiteMapBuilder graph, MarkdownProcessor processor, IServiceProvider services, IEnumerable<ISiteNodeProcessor> processors)
    {
        _logger = logger;
        this.graph = graph;
        this.processor = processor;
        this.services = services;
        this.processors = processors.ToList();
    }

    /// <summary>
    /// Outputs the site map
    /// </summary>
    /// <returns></returns>
    [Route("/sitemap.xml")]
    public async Task<IActionResult> Xml()
    {
        var site = await graph.BuildAsync();

        var urls = new List<SiteMapXmlUrl>();

        foreach(var node in site)
        {
            var uri = new SiteMapXmlUrl(node.Url.ToString(), node.DateModified ?? node.DateCreated);
            urls.Add(uri);
        }

        var sb = new StringBuilder();
        var settings = new XmlWriterSettings
        {
            NamespaceHandling = NamespaceHandling.OmitDuplicates,
            OmitXmlDeclaration = false
        };

        using var writer = XmlWriter.Create(sb, settings);
        SiteMapSerializer.Serialize(writer, urls);
        writer.Flush();
        
        return new ContentResult()
        {
            StatusCode = 200,
            ContentType = "application/xml",
            Content = sb.ToString()
        };
    }

    public async Task<IActionResult> Page()
    {
        var site = await graph.BuildAsync();

        var uri = UrlBuilder.Build((string?)Request.Path.ToUriComponent() ?? "/");

        var node = site.FindByUri(uri);

        if (node == null)
        {
            return NotFound();
        }

        node.Tree.Refresh(site);// nodes);

        ViewBag.Root = site.First();

        // Process Markdown
        await processor.ProcessAsync(node);

        // Run processors
        foreach (var processor in processors)
        {
            if (processor is IInitializable intializable)
            {
                await intializable.InitializeAsync();
            }
            await processor.ProcessAsync(node);
        }
        
        return View("Page", node);
    }
}