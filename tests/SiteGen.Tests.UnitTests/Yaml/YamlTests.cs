using System;
using System.Buffers;
using System.Collections.Generic;
using System.Dynamic;
using System.IO.Pipelines;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using SiteGen.Core.Extensions;
using YamlDotNet.Serialization.NamingConventions;

namespace SiteGen.Tests.UnitTests.Yaml
{
    public class YamlTests
    {
        [Fact]
        public void Test()
        {
            //var yaml = @"--- !<tag:clarkevans.com,2002:invoice>
            var yaml = @"invoice: 34843
date   : 2001-01-23
bill-to: &id001
  given  : Chris
  family : Dumars
  address:
    lines: |
      458 Walkman Dr.
      Suite #292
    city    : Royal Oak
    state   : MI
    postal  : 48046
ship-to: *id001
product:
- sku         : BL394D
  quantity    : 4
  description : Basketball
  price       : 450.00
- sku         : BL4438H
  quantity    : 1
  description : Super Hoop
  price       : 2392.00
tax  : 251.42
total: 4443.52
comments:
  Late afternoon is best.
  Backup contact is Nancy
  Billsmer @ 338-4338.";

            var bytes = Encoding.UTF8.GetBytes(yaml);

            var deserializer = new YamlDotNet.Serialization.DeserializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .Build();

            var values = deserializer.Deserialize<Dictionary<string,object>>(yaml);

            //var values = YamlReader.Read(bytes.AsSpan()).ToDictionary(pair => pair.Key, pair => pair.Value);

            Assert.Equal("34843", values["invoice"]);
        }
    }

    public enum YamlToken
    {
        None = 0,
        Scalar,
        Collection,
        BlockSequenceEntry,
        MappingKey,
        MappingValue,
        FlowSequence,
        FlowMapping,

        Property,

        FlowSequenceStart,
        FlowSequenceEnd,
        FlowCollectionEntry,
        MappingSequenceStart,
        MappingSequenceEnd,
        Comment,
        Anchor,
        Tag,
        Alias,
        LiteralBlock,
        FoldedBlock,
        SingleQuote,
        DoubleQuote,
        Directive,
        DocumentMarker
    }

    public static class YamlConstants
    {
        
            public const byte OpenBrace = (byte)'{';
            public const byte CloseBrace = (byte)'}';
            public const byte OpenBracket = (byte)'[';
            public const byte CloseBracket = (byte)']';
            public const byte QuestionMark = (byte)'?';
            public const byte Space = (byte)' ';
            public const byte CarriageReturn = (byte)'\r';
            public const byte LineFeed = (byte)'\n';
            public const byte Tab = (byte)'\t';
            public const byte ListSeparator = (byte)',';
            public const byte KeyValueSeparator = (byte)':';
            public const byte Quote = (byte)'"';
            public const byte SingleQuote = (byte)'\'';
            public const byte Percent = (byte)'%';
            public const byte BackSlash = (byte)'\\';
            public const byte Slash = (byte)'/';
            public const byte BackSpace = (byte)'\b';
            public const byte FormFeed = (byte)'\f';
            public const byte Asterisk = (byte)'*';
            public const byte Colon = (byte)':';
            public const byte Period = (byte)'.';
            public const byte Plus = (byte)'+';
            public const byte Hyphen = (byte)'-';
            public const byte UtcOffsetToken = (byte)'Z';
            public const byte TimePrefix = (byte)'T';
            public const byte Comma = (byte)',';
            public const byte Hash = (byte)'#';
            public const byte Ampersand = (byte)'&';
            public const byte Exclamation = (byte)'!';
            public const byte Pipe = (byte)'|';
            public const byte GreaterThan = (byte)'>';

            // \u2028 and \u2029 are considered respectively line and paragraph separators
            // UTF-8 representation for them is E2, 80, A8/A9
            public const byte StartingByteOfNonStandardSeparator = 0xE2;

            public static ReadOnlySpan<byte> Utf8Bom => new byte[] { 0xEF, 0xBB, 0xBF };
            public static ReadOnlySpan<byte> TrueValue => new byte[] { (byte)'t', (byte)'r', (byte)'u', (byte)'e' };
            public static ReadOnlySpan<byte> FalseValue => new byte[] { (byte)'f', (byte)'a', (byte)'l', (byte)'s', (byte)'e' };
            public static ReadOnlySpan<byte> NullValue => new byte[] { (byte)'n', (byte)'u', (byte)'l', (byte)'l' };

            public static ReadOnlySpan<byte> NaNValue => new byte[] { (byte)'N', (byte)'a', (byte)'N' };
            public static ReadOnlySpan<byte> PositiveInfinityValue => new byte[] { (byte)'I', (byte)'n', (byte)'f', (byte)'i', (byte)'n', (byte)'i', (byte)'t', (byte)'y' };
            public static ReadOnlySpan<byte> NegativeInfinityValue => new byte[] { (byte)'-', (byte)'I', (byte)'n', (byte)'f', (byte)'i', (byte)'n', (byte)'i', (byte)'t', (byte)'y' };

            // Used to search for the end of a number
            public static ReadOnlySpan<byte> Delimiters => new byte[] { ListSeparator, CloseBrace, CloseBracket, Space, LineFeed, CarriageReturn, Tab, Slash };

            // Explicitly skipping ReverseSolidus since that is handled separately
            public static ReadOnlySpan<byte> EscapableChars => new byte[] { Quote, (byte)'n', (byte)'r', (byte)'t', Slash, (byte)'u', (byte)'b', (byte)'f' };
            
    }

    class YamlReader
    {
        // public static IDictionary<string, string?> Read(Stream stream)
        // {
        //     var reader = PipeReader.Create(stream);
            
            

        //     SequenceReader<char> reader = new SequenceReader<char>(null);

            
        //     var values = new Dictionary<string, string?>();

        //     using var reader = new StreamReader(stream, Encoding.UTF8, true);

        //     reader.Read
        // }

        public static IEnumerable<KeyValuePair<string, string?>> Read(ReadOnlySpan<byte> yaml)
        {
            var values = new Dictionary<string, string?>();

            var reader = new YamlReader();

            var span = yaml;
            
            if (span.Length == 0) return values;

            var token = YamlToken.None;

            var sequence = new ReadOnlySequence<byte>(yaml.ToArray());

            //SequenceReader<byte> reader = new SequenceReader<byte>()
            
            token = reader.ReadNextToken(span);

            return values;
        }

        public static IDictionary<string,string?> Parse(Stream stream)
        {
            var results = new Dictionary<string, string?>();
            using var reader = new StreamReader(stream);

            var indentation = 0;
            var stack = new Stack<string>();

            var line = reader.ReadLine()?.TrimEnd();
            while (line != null)
            {
                var count = line.CountLeadingWhitespace();
                var parts = line.Split(":", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

                if (!parts[0].StartsWith("#"))
                {
                    results[parts[0]] = parts.Length > 1 ? parts[1] : null;
                }
            }

            return results;
        }

        YamlToken ReadNextToken(ReadOnlySpan<byte> span)
        {
            switch (span[0])
            {
                case YamlConstants.Hyphen:

                    // Peek to see if this is really a structure block
                    if (span.Length > 3 && span[1] == YamlConstants.Hyphen && span[2] == YamlConstants.Hyphen)
                        return YamlToken.DocumentMarker;

                    return YamlToken.BlockSequenceEntry;

                case YamlConstants.OpenBracket:
                    return YamlToken.FlowSequenceStart;
                case YamlConstants.CloseBracket:
                    return YamlToken.FlowSequenceEnd;
                case YamlConstants.OpenBrace:
                    return YamlToken.MappingSequenceStart;
                case YamlConstants.CloseBrace:
                    return YamlToken.MappingSequenceEnd;
                case YamlConstants.QuestionMark:
                    return YamlToken.MappingKey;
                case YamlConstants.Colon:
                    return YamlToken.MappingValue;
                case YamlConstants.Comma:
                    return YamlToken.FlowCollectionEntry;
                case YamlConstants.Hash:
                    return YamlToken.Comment;
                case YamlConstants.Ampersand:
                    return YamlToken.Anchor;
                case YamlConstants.Exclamation:
                    return YamlToken.Tag;
                case YamlConstants.Asterisk:
                    return YamlToken.Alias;
                case YamlConstants.Pipe:
                    return YamlToken.LiteralBlock;
                case YamlConstants.GreaterThan:
                    return YamlToken.FoldedBlock;
                case YamlConstants.Quote:
                    return YamlToken.DoubleQuote;
                case YamlConstants.SingleQuote:
                    return YamlToken.SingleQuote;
                case YamlConstants.Percent:
                    return YamlToken.Directive;
                default:
                    return YamlToken.None;
            }
        }
    }

    
}
