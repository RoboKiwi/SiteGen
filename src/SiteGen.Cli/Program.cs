using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SiteGen.Core.Formatters.SiteMapXml;
using SiteGen.Cli;
using SiteGen.Cli.Commands;
using System.Diagnostics;

var configBuilder = new ConfigurationBuilder();
configBuilder.AddEnvironmentVariables();
configBuilder.AddCommandLine(args);

var configuration = configBuilder.Build();

var settings = new CliArgs
{
    Command = Enum.Parse<Command>(args.Length > 0 ? args[0] : "Build", true),
    ContentPath = "public",
    ServerUri = "http://localhost:5000",
    StaticPaths = new List<string>
    {
        "wwwroot", "static"
    },
};

configuration.Bind(settings);

var services = new ServiceCollection();
services.AddSingleton(configuration);
services.AddSingleton(settings);
services.AddTransient<ServeCommand>();

var serviceProviderOptions = new ServiceProviderOptions { ValidateScopes = true, ValidateOnBuild = true };
var factory = new DefaultServiceProviderFactory(serviceProviderOptions);
var serviceProvider = factory.CreateServiceProvider(services);

ICommandlet command = default;

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
client.BaseAddress = new Uri(settings.ServerUri);

var xml = await client.GetStringAsync("/sitemap.xml");

var map = SiteMapSerializer.Deserialize(xml);

var directory = new DirectoryInfo("public");
if (!directory.Exists) directory.Create();

// Drop the sitemap into the destination
File.WriteAllText( Path.Combine( directory.FullName, "sitemap.xml"), xml);

var stopwatch = new Stopwatch();
stopwatch.Start();

Console.WriteLine("Building...");

//var config = Configuration.Default;
//var context = BrowsingContext.New(config);
//using (var document = context.GetService<IHtmlParser>().ParseDocument(contents))

foreach (var item in map)
{
    var directoryName = Path.Join(directory.FullName, item.Location.ToString().Replace('/', Path.DirectorySeparatorChar));
    var filename = Path.GetFullPath( Path.Join(directoryName, "index.html") );
        
    var file = new FileInfo(filename);
    if (file.Directory?.Exists == false) file.Directory.Create();
    
    var uri = new Uri(client.BaseAddress, item.Location);

    Console.WriteLine("{0} => {1}", uri, filename);

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
Console.WriteLine("Copying static files...");
foreach(var staticPath in settings.StaticPaths)
{
    var dir = new DirectoryInfo(staticPath);
    if (!dir.Exists) continue;

    Console.Write("From: {0}", dir.FullName);
    
    foreach(var file in dir.EnumerateFiles("*", SearchOption.AllDirectories))
    {
        if (!file.Exists) continue;
        var relativePath = Path.GetRelativePath(staticPath, file.FullName);
        var destination = new FileInfo(Path.Combine(directory.FullName, relativePath));
        Console.WriteLine("{0} => {1}", file.FullName, destination.FullName);
        if(destination.Directory?.Exists == false) destination.Directory.Create();
        file.CopyTo(destination.FullName, true);
    }
}

stopwatch.Stop();

Console.WriteLine($"Finished in {stopwatch.Elapsed}");