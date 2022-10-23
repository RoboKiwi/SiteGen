namespace SiteGen.Core.Services.Generators;

public static class UrlBuilder
{
    public static Uri Build(params string[] segments)
    {
        var value = '/' + string.Join('/', segments.Select(s => s.ToLowerInvariant().Trim('/', '\\'))).Replace('\\', '/') + '/';
        while (value.IndexOf("//") > -1) value = value.Replace("//", "/");
        return new Uri(value, UriKind.Relative);
    }

    public static string Friendly(string value)
    {
        return value.ToLowerInvariant().Replace(' ', '-');
    }

    public static string UriToFilename(Uri uri)
    {
        var clean = new Uri("/" + uri.ToString().TrimStart('/'), UriKind.RelativeOrAbsolute);
        return clean.ToString().Replace('/','\\');
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

        while (relativeFilename.IndexOf("//") > -1)
        {
            relativeFilename = relativeFilename.Replace("//", "/");
        }

        return Build(relativeFilename);
    }
}
