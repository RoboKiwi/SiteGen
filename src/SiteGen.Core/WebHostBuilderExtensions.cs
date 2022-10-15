namespace SiteGen.Core;

public static class WebHostBuilderExtensions
{
    public static IEndpointRouteBuilder UseSiteGen(this IEndpointRouteBuilder app)
    {
        app.MapFallbackToController("Page", "Home");
        return app;
    }
}