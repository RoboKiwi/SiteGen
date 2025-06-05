using SiteGen.Core.Extensions;
using SiteGen.Core.Services.Generators;

namespace SiteGen.Core.Models;

public class DirectorySiteNode : SiteNode
{
    readonly DirectoryInfo directory;

    public DirectorySiteNode(DirectoryInfo directory, DirectoryInfo baseDirectory)
    {
        this.directory = directory;

        Path = directory.FullName;
        Ext = directory.Extension;
        FileName = directory.Name;
        Title = Humanizer.FromFilename(directory);
        Type = NodeType.Section;
        Url = UrlBuilder.RelativeToDirectory(directory, baseDirectory);

        if(directory.Exists)
        {
            Date = directory.LastWriteTime < directory.CreationTime ? directory.LastWriteTime : directory.CreationTime;
            DateCreated = directory.CreationTime;
            DateModified = directory.LastWriteTime;
        }
    }
}
