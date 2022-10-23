namespace SiteGen.Core.Configuration
{
    public class SiteGenSettings
    {
        public SiteGenSettings() {

            Taxonomies = new Dictionary<string, string?>()
            {
                {"category", "categories" },
                {"tag", "tags" }
            };

            ContentPaths = new List<string>();// { "content" };
        }

        public IDictionary<string, string?> Taxonomies { get; set; }

        public IList<string> ContentPaths { get; set; }
    }
}