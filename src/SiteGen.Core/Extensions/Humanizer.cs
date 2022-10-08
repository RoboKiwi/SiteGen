namespace SiteGen.Core.Extensions;

public static class Humanizer
{
    public static string FromFilename(FileSystemInfo file)
    {
       return Path.GetFileNameWithoutExtension(file.FullName).ToDelimitedWords(' ', TitleCaseOptions.FirstWordOnly);
    }

    internal static string FromString(string value)
    {
        return value.ToDelimitedWords(' ', TitleCaseOptions.FirstWordOnly);
    }
}