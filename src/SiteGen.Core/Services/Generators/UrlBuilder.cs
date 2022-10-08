namespace SiteGen.Core.Services.Generators;

public static class UrlBuilder
{
    public static Uri Build(params string[] segments)
    {
        return new Uri( '/' + string.Join('/', segments.Select(s => s.ToLowerInvariant().Trim('/', '\\'))).Replace('\\', '/') + '/', UriKind.Relative);
    }

    public static string Friendly(string value)
    {
        return value.ToLowerInvariant().Replace(' ', '-');
    }

    public static Uri RelativeToDirectory(FileSystemInfo file, FileSystemInfo baseDirectory)
    {
        var relativeFilename = Path.GetRelativePath(baseDirectory.FullName, file.FullName).ToLowerInvariant();

        if (relativeFilename.EndsWith(".md"))
        {
            relativeFilename = relativeFilename.Substring(0, relativeFilename.Length - ".md".Length);
        }

        if (relativeFilename.EndsWith("_index"))
        {
            relativeFilename = relativeFilename.Substring(0, relativeFilename.Length - "_index".Length);
        }

        relativeFilename = "/" + relativeFilename.TrimStart('/', '\\').TrimEnd('/', '\\').Replace('\\', '/') + "/";

        if (relativeFilename.Equals("/./")) relativeFilename = "/";

        return Build(relativeFilename);
    }
}
