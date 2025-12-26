# Architecture

When SiteGen runs, it gets the sitemap.xml from the host,
and then crawls the site, generating the static HTML files from the results.

The hosting website can itself reference SiteGen, and use SiteGen to generate the sitemap dynamically
from any manner of dynamic or static content, such as Markdown files.

## SiteGen as a dynamic generator

SiteGen can be injected into an ASP.NET website to allow it to handle generating the SiteMap,
and rendering pages dynamically.

SiteGen is included as a Razor Component Library, so that it can include logic, default views and static assets.

You can then configure or override any of the default behaviour.

## Site Map

The site map is just a flat collection of `SiteNode` objects.

This site map can be appended to easily by any built-in generators, or extension points.

The list of nodes are built by running all of the registered generators (`INodeGenerator`):

- The `MarkdownGenerator` scans the configured content directories for `.md` files, and generates nodes for each file found.
- The `TaxonomyGenerator` generates nodes for any taxonomies (tags, categories) found in the Markdown front matter.

Generators normally set the raw `Content` on the `SiteNode`, and other properties.

A `SiteNode` can represent logical content (e.g. a markdown page), or virtual content (e.g. a taxonomy listing page).

## Processing (rendering)

When the pages in a site map are visited, and it falls back to the SiteGen controller, it renders the
node by passing it through the `ISiteNodeProcessor` pipeline.

Typically this involves transforming the raw `Content` into HTML.

- `FrontMatterProcessor` reads any front matter block from the Content file, updates the SiteNode's properties and metadata
- `GitInfoProcessor` updates the SiteNode's properties with git information (last modified date, author, etc)
- `WordCountProcessor` counts the words in the markdown file and updates the SiteNode's properties
- `WordCountFuzzyProcessor` generate a fuzzy workcount
- `ReadingTimeProcessor` estimates the reading time based on the word count
- `MarkdownProcessor` converts the markdown content to HTML
- `TableOfContentsProcessor` generates a table of contents from the markdown headings

## Front Matter

Front matter can be in YAML, TOML or JSON.

Front matter blocks and formats are auto-detected by `FrontMatterParser`.

The raw front matter properties are loaded into the FrontMatter dictionary on the SiteNode.

Additionally, these values are then bound to the SiteNode using the Configuration pipeline, allowing the front matter to
set / overwrite SiteNode properties.

There is a list of properties that excluded from binding:

- guid
- type

## Markdown

The MarkdownProcessor uses the configured Markdig pipeline to convert the markdown to HTML and plain text.
