using Microsoft.AspNetCore.Mvc;
using SiteGen.Core.Models;
using SiteGen.Core.Services;
using SiteGen.Core.Services.Generators;
using SiteGen.Core.Services.Processors;

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
            await processor.ProcessAsync(node);
        }

        switch (node.Type)
        {
            case NodeType.Section:
//                return View("Section", node);

            case NodeType.Page:
            default:
                return View("Page", node);
        }
    }
}