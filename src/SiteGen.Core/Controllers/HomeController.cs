using Microsoft.AspNetCore.Mvc;
using SiteGen.Core.Models;
using SiteGen.Core.Services;

namespace SiteGen.Core.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ISiteMapService siteMap;
    private readonly SitePipelineBuilder pipeline;

    public HomeController(ILogger<HomeController> logger, ISiteMapService siteMap, SitePipelineBuilder pipeline)
    {
        _logger = logger;
        this.siteMap = siteMap;
        this.pipeline = pipeline;
    }

    public async Task<IActionResult> Page()
    {
        var path = Request.Path.ToUriComponent();

        var normalizedPath = ("/" + (path ?? "/").TrimStart('/')).TrimEnd('/') + '/';

        var uri = new Uri(normalizedPath, UriKind.Relative);

        var nodes = await siteMap.GetNodesAsync("content");

        var node = nodes.SingleOrDefault(x => x.Url == uri);

        if (node == null)
        {
            return NotFound();
        }

        node.Tree.Refresh(nodes);

        ViewBag.Root = nodes.First();

        await pipeline.ProcessNode(node);

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