using LibGit2Sharp;
using SiteGen.Core.Configuration;
using SiteGen.Core.Configuration.Yaml;
using SiteGen.Core.Models;
using System.Text;
using Tommy.Extensions.Configuration;

namespace SiteGen.Core.Services.Processors;

/// <summary>
/// Updates the node front matter and metadata with
/// information from its last Git commit.
/// </summary>
public class GitInfoProcessor : ISiteNodeProcessor, IInitializable
{
    private readonly SiteGenSettings settings;

    public GitInfoProcessor(SiteGenSettings settings)
    {
        this.settings = settings;
    }

    public async Task InitializeAsync()
    {
        //var path = Path.GetFullPath(settings.ContentPaths.First());
        //var repoPath = Repository.Discover(path);
        //if (repoPath == null) return;

        //using (var repo = new Repository(repoPath))
        //{
        //    var relativePath = Path.GetRelativePath(repoPath, path);
        //    var normalizedPath = relativePath.TrimStart('.').TrimStart('\\').Replace('\\', '/');

        //    var lookupList = new[] {path};
        //    var explicitPathOptions = new ExplicitPathsOptions { };
        //    var compareOptions = new CompareOptions() { IncludeUnmodified = true, Similarity = SimilarityOptions.None };
        //    var files = repo.Diff.Compare<TreeChanges>((Tree)null, DiffTargets.Index, lookupList, explicitPathOptions, compareOptions);
        //}
    }

    public async Task ProcessAsync(SiteNode node)
    {
        var repoPath = Repository.Discover(node.Path);
        if (repoPath == null) return;
        
        var filter = new CommitFilter()
        {
            SortBy = CommitSortStrategies.Time | CommitSortStrategies.Reverse
        };

        IDictionary<string,Commit> commits = new Dictionary<string,Commit>();

        using (var repo = new Repository(repoPath))
        {
            var relativePath = Path.GetRelativePath(repoPath, node.Path);
            var normalizedPath = relativePath.TrimStart('.').TrimStart('\\').Replace('\\','/');

            var log = repo.Commits.QueryBy(normalizedPath).FirstOrDefault();

            if( log == null) return;

            var commit = log.Commit;

            node.Date = commit.Author.When;
            node.DateModified = commit.Author.When;
        }
    }
}