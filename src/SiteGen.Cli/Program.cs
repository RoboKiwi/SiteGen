using AngleSharp;
using AngleSharp.Css.Dom;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SiteGen.Core.Configuration;
using SiteGen.Core;
using SiteGen.Core.Formatters.SiteMapXml;
using SiteGen.Core.Services.Generators;
using SiteGen.Cli;
using SiteGen.Cli.Commands;
using System.Diagnostics;


var configBuilder = new ConfigurationBuilder();
configBuilder.AddEnvironmentVariables();
configBuilder.AddCommandLine(args);

var configuration = configBuilder.Build();

var settings = new CliArgs
{
    ContentPath = "public"
};

var serverUri = "http://localhost:5048";

settings.StaticPaths = new List<string> { "D:\\Work\\RoboKiwi\\RoboKiwi.com\\static", "D:\\Work\\RoboKiwi\\SiteGen\\examples\\RoboKiwi.com\\wwwroot" };

settings.Command = Enum.Parse<Command>(args.Length > 0 ? args[0] : "Build", true);

configuration.Bind(settings);

var services = new ServiceCollection();
services.AddSingleton(configuration);
services.AddSingleton(settings);
services.AddTransient<ServeCommand>();

var factory = new DefaultServiceProviderFactory();
var serviceProvider = factory.CreateServiceProvider(services);

ICommandlet command = null;

switch (settings.Command)
{
    case Command.None:
        break;
    case Command.Build:
        break;
    case Command.Serve:
        command = serviceProvider.GetRequiredService<ServeCommand>();
        break;
    default:
        break;
}

if( command != null)
{
    await command.ExecuteAsync();
    return;
}

// Get the sitemap
var client = new HttpClient();
client.BaseAddress = new Uri(serverUri);

var xml = await client.GetStringAsync("/sitemap.xml");

var map = SiteMapSerializer.Deserialize(xml);

var directory = new DirectoryInfo("public");
if (!directory.Exists) directory.Create();

// Drop the sitemap into the destination
File.WriteAllText(xml, directory.FullName);

var stopwatch = new Stopwatch();
stopwatch.Start();

Console.WriteLine("Building...");

var config = Configuration.Default;
var context = BrowsingContext.New(config);

//using (var document = context.GetService<IHtmlParser>().ParseDocument(contents))

foreach (var item in map)
{
    var filename = Path.GetFullPath( directory.FullName + item.Location.ToString().Replace('/','\\') + "\\index.html");
    
    var file = new FileInfo(filename);
    if (file.Directory?.Exists == false) file.Directory.Create();
    
    var uri = new Uri(client.BaseAddress, item.Location);

    Console.WriteLine(uri);

    var response = await client.GetAsync(uri);
    response.EnsureSuccessStatusCode();    
    
    // Get the content
    var contents = await response.Content.ReadAsStringAsync();

    // Write to the output
    File.WriteAllText(filename, contents);

    //using var document = await context.OpenAsync(response =>
    //{
    //    response
    //        .Content(contents)
    //        .Address(uri);
    //});

    //var query = document.QuerySelectorAll<IHtmlLinkElement>("link[rel=stylesheet]");
    //foreach(var link in query)
    //{
    //    var linkUri = new Uri(link.Href);
    //    var relativeUri = client.BaseAddress.MakeRelativeUri( new Uri(linkUri.GetLeftPart(UriPartial.Path)) );

    //    var stylesheetFilename = UrlBuilder.UriToFilename(relativeUri);

    //    using var stream = await client.GetStreamAsync(link.Href);
    //    var stylesheetDestination = Path.GetFullPath(directory.FullName + stylesheetFilename);

    //    using var stylesheet = File.OpenWrite(stylesheetDestination);
    //    using var writer = new StreamWriter(stylesheet);
    //    stream.CopyTo(stylesheet);
    //    stylesheet.Flush();
    //}
}

// Copy static files
foreach(var staticPath in settings.StaticPaths)
{
    var dir = new DirectoryInfo(staticPath);
    if (!dir.Exists) continue;
    
    foreach(var file in dir.EnumerateFiles("*", SearchOption.AllDirectories))
    {
        if (!file.Exists) continue;
        var relativePath = Path.GetRelativePath(staticPath, file.FullName);
        var destination = new FileInfo(Path.Combine(directory.FullName, relativePath));
        Console.WriteLine(destination.FullName);
        if(destination.Directory != null && !destination.Directory.Exists) destination.Directory.Create();
        file.CopyTo(destination.FullName, true);
    }
}

stopwatch.Stop();

Console.WriteLine($"Finished in {stopwatch.Elapsed}");

Console.WriteLine("Press ENTER");
Console.ReadLine();