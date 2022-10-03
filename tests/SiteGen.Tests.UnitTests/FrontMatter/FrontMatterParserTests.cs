using SiteGen.Core.Models;

namespace SiteGen.Tests.UnitTests.FrontMatter
{
    public class FrontMatterParserTests
    {
        [Fact]
        public void ParseYaml()
        {
            var contents = @"---
title: YAML Title
description: ""YAML description""
---

This isn't front matter
";

            var result = FrontMatterParser.ReadBlock(contents);

            Assert.NotNull(result);
            Assert.Equal(FrontMatterFormat.Yaml, result?.Item1);
            Assert.Equal(@"title: YAML Title
description: ""YAML description""", result?.Item2);
        }

        [Fact]
        public void ParseJson()
        {
            var contents = @"{
title: ""YAML Title"",
description: ""YAML description"",
}

This isn't front matter
";

            var result = FrontMatterParser.ReadBlock(contents);

            Assert.NotNull(result);
            Assert.Equal(FrontMatterFormat.Json, result?.Item1);
            Assert.Equal(@"title: ""YAML Title"",
description: ""YAML description"",", result?.Item2);
        }

        [Fact]
        public void ParseToml()
        {
            var contents = @"+++
title=""TOML Title""
description=""TOML description""
+++

This isn't front matter
";

            var result = FrontMatterParser.ReadBlock(contents);

            Assert.NotNull(result);
            Assert.Equal(FrontMatterFormat.Toml, result?.Item1);
            Assert.Equal(@"title=""TOML Title""
description=""TOML description""", result?.Item2);
        }
    }
}
