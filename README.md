[![Release to NuGet.org](https://github.com/RoboKiwi/SiteGen/actions/workflows/release-to-nuget.yml/badge.svg)](https://github.com/RoboKiwi/SiteGen/actions/workflows/release-to-nuget.yml)

# SiteGen

SiteGen is a .NET-based static site generator.

## Features

- Markdown support
- Syntax Highlighting
  - Pygments
  - Prism.js
  - Monaco
- Mermaid diagrams
- Razor templates
- Rich site hierarchy model, for building navigation in your templates
- Table of Contents
- Extensible

## Get Started

In your ASP.NET website, reference `SiteGen.Core`.

Example `Programs.cs`:

```csharp
using SiteGen.Core;
using SiteGen.Core.Configuration;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.ConfigureSiteGen();

var settings = new SiteGenSettings();
builder.Configuration.Bind(settings);
builder.Services.AddSingleton(settings);

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.UseSiteGen();

app.Run();
```

Configure paths in `appsettings.json`:

```json
{
  "ContentPaths": [ "content" ],
  "StaticPaths": [ "static", "wwwroot" ]
}
```

Add your `.md` files into the `content` folder, and your static assets in `static` or `wwwroot`.

Add a `Page.cshtml` to 'Pages/Shared' (for Razor Pages) or 'Views/Home' or 'Views/Shared' (for MVC) folder.

> You should remove the @page directive from the top of the `Page.cshtml` file, as SiteGen will handle routing for you.

If you generate client-side css and javascript, you can distribute your built artifacts to either the `static` or `wwwroot` folders.

In your Razor views, you can reference the `ViewBag.Root` for the base node of the website, and use the `.Tree` property to crawl the site hierarchy.

## Building and publishing

- Install the SiteGen tool

```dotnetcli
dotnet new tool-manifest
dotnet tool install --local SiteGen.Cli --prerelease
```

- Launch your website and bind to port :5000
- Run `dotnet sitegen` to crawl your site and publish the static resources to `/public`

# How it works

SiteGen registers itself as a default fallback route. This allows your website to implement its own routes which will take precedence.

By default, SiteGen will generate a site map from the markdown files it finds in the configured content directories.

The SiteGen fallback route it for its in-built HomeController and Page action. This is why you must implement a default Page.cshtml view.

# Services

# Interactive, hosted browser scraping with Playwright

Playwright can be used to dynamically scrape pages and sites and even do things like invoke javascript.

Singleton instances of `IPlaywright` (main Playwright service), and a keyed singleton instance of `IBrowser` (key of `"Chromium"`) are injected.

These hosts are used to dynamically host and invoke syntax highlighters like Monaco Editor and Prism.js.
