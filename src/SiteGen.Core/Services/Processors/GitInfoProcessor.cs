using LibGit2Sharp;
using SiteGen.Core.Models;

namespace SiteGen.Core.Services.Processors;

/// <summary>
/// Updates the node front matter and metadata with
/// information from its last Git commit.
/// </summary>
public class GitInfoProcessor : ISiteNodeProcessor
{
    class GitInfo
    {
        public DateTimeOffset AddedTimestamp { get; set; }
        public Signature AddedAuthor { get; set; } = null!;
        public DateTimeOffset ModifiedTimestamp { get; set; }
        public Signature ModifiedAuthor { get; set; } = null!;
    }

    public async Task ProcessAsync(SiteNode node, CancellationToken cancellationToken)
    {
        var history = QueryHistory(node, cancellationToken);
        if(history == null) return;
        node.Date ??= history.AddedTimestamp;
        node.DateModified = history.ModifiedTimestamp;
    }

    static readonly Dictionary<string, Dictionary<string, GitInfo>> histories = new();

    GitInfo? QueryHistory(SiteNode node, CancellationToken cancellationToken)
    {
        var repoPath = Repository.Discover(node.Path);
        if(string.IsNullOrWhiteSpace(repoPath)) return null;

        if(!histories.TryGetValue(repoPath, out var history))
        {
            using var repo = new Repository(repoPath);

            history = new Dictionary<string, GitInfo>();
            var deletedFiles = new HashSet<string>();

            var filter = new CommitFilter
            {
                SortBy = CommitSortStrategies.Topological | CommitSortStrategies.Reverse
            };

            foreach(var commit in repo.Commits.QueryBy(filter))
            {
                var timestamp = commit.Author.When;
                var author = commit.Author;

                var changes = !commit.Parents.Any()
                    ? repo.Diff.Compare<TreeChanges>(null, commit.Tree)
                    : repo.Diff.Compare<TreeChanges>(commit.Parents.First().Tree, commit.Tree);

                foreach(var change in changes)
                {
                    string path = Path.GetFullPath(Path.Combine(repo.Info.WorkingDirectory, change.Path));

                    if(change.Status == ChangeKind.Deleted)
                    {
                        deletedFiles.Add(path);
                        continue;
                    }

                    if(!history.ContainsKey(path))
                    {
                        // First time we see the file: it's being added
                        history[path] = new GitInfo
                        {
                            AddedTimestamp = timestamp,
                            AddedAuthor = author,
                            ModifiedTimestamp = timestamp,
                            ModifiedAuthor = author
                        };
                    }
                    else
                    {
                        // Update last modified info
                        history[path].ModifiedTimestamp = timestamp;
                        history[path].ModifiedAuthor = author;
                    }
                }
            }

            // Remove deleted files from history
            foreach(var deleted in deletedFiles)
            {
                history.Remove(deleted);
            }

            histories[repoPath] = history;
        }
        
        return history.TryGetValue(node.Path, out var gitInfo) ? gitInfo : null;
    }
}