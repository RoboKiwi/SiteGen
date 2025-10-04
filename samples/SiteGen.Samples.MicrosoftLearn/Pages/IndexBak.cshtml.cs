using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SiteGen.Samples.MicrosoftLearn.Pages;
public class IndexBakModel : PageModel
{
    private readonly ILogger<IndexBakModel> _logger;

    public IndexBakModel(ILogger<IndexBakModel> logger)
    {
        _logger = logger;
    }

    public void OnGet()
    {

    }
}
