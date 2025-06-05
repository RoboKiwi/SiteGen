using SiteGen.Core.Models;

namespace SiteGen.Tests.FrontMatter;

[TestClass]
public class FrontMatterParserTests
{
    [TestMethod]
    public void ParseYaml()
    {
        var contents = @"---
title: YAML Title
description: ""YAML description""
---

This isn't front matter
";

        var result = FrontMatterParser.ReadBlock(contents);

        Assert.IsNotNull(result);
        Assert.AreEqual(FrontMatterFormat.Yaml, result?.Item1);
        Assert.AreEqual(@"title: YAML Title
description: ""YAML description""", result?.Item2);
    }

    [TestMethod]
    public void ParseJson()
    {
        var contents = @"{
title: ""YAML Title"",
description: ""YAML description"",
}

This isn't front matter
";

        var result = FrontMatterParser.ReadBlock(contents);

        Assert.IsNotNull(result);
        Assert.AreEqual(FrontMatterFormat.Json, result?.Item1);
        Assert.AreEqual(@"title: ""YAML Title"",
description: ""YAML description"",", result?.Item2);
    }

    [TestMethod]
    public void ParseToml()
    {
        var contents = @"+++
title=""TOML Title""
description=""TOML description""
+++

This isn't front matter
";

        var result = FrontMatterParser.ReadBlock(contents);

        Assert.IsNotNull(result);
        Assert.AreEqual(FrontMatterFormat.Toml, result?.Item1);
        Assert.AreEqual(@"title=""TOML Title""
description=""TOML description""", result?.Item2);
    }
}