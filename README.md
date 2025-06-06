[![Release to NuGet.org](https://github.com/RoboKiwi/SiteGen/actions/workflows/release-to-nuget.yml/badge.svg)](https://github.com/RoboKiwi/SiteGen/actions/workflows/release-to-nuget.yml)

# SiteGen

SiteGen is a .NET-based static site generator.

## Features

- Markdown support
- Pygments syntax highlighting
- Mermaid diagrams
- Razor templates
- Rich site hierarchy model, for building navigation in your templates
- Table of Contents
- Extensible

## Usage

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

Add your `.md` files in a `content` folder, and your static assets in `static`.

If you generate client-side css and javascript, you can distribute your built artifacts to either the `static` or `wwwroot` folders.

In your Razor views, you can reference the `ViewBag.Root` for the base node of the website, and use the `.Tree` property to crawl the site hierarchy.

## Building and publishing

- Install the SiteGen tool

```dotnetcli
dotnet new tool-manifest
dotnet tool install --local SiteGen.Cli
```

- Launch your website and bind to port :5000
- Run `dotnet sitegen` to crawl your site and publish the static resources to `/public`

# Services

# Interactive, hosted browser scraping with Playwright

Playwright can be used to dynamically scrape pages and sites and even do things like invoke javascript.

Singleton instances of `IPlaywright` (main Playwright service), and a keyed singleton instance of `IBrowser` (key of `"Chromium"`) are injected.

These hosts are used to dynamically host and invoke syntax highlighters like Monaco Editor and Prism.js.
