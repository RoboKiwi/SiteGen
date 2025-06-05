namespace SiteGen.Core.Services;

public class FileCacheProvider : IDisposable
{
    public FileCacheProvider()
    {
        var path = Path.Combine(Path.GetTempPath(), "SiteGen");
        Directory = new DirectoryInfo(path);
        if (!Directory.Exists) Directory.Create();
    }

    public DirectoryInfo Directory { get; }

    public TempFile GetTempFile(string filename)
    {
        var path = Path.Combine(Directory.FullName, filename);
        return new TempFile(path);
    }

    public void Dispose()
    {
    }
}