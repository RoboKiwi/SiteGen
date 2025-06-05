using Markdig.Helpers;
using System.Diagnostics.CodeAnalysis;

namespace SiteGen.Core.Extensions;

public static class SpanExtensions
{
    public static ReadOnlySpan<char> StripStart(this ReadOnlySpan<char> value, ReadOnlySpan<char> x)
    {
        if (value == null || x == null || x.Length > value.Length) return value;
        return value.IndexOf(x) == 0 ? value[x.Length..] : value;
    }

    public static bool IsEmpty([NotNullWhen(false)] this string? value)
    {
        return string.IsNullOrWhiteSpace(value);
    }

    /// <summary>
    /// Returns a count of the number of words in the string,
    /// separated by whitespace characters.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static int WordCount(this ReadOnlySpan<char> value)
    {
        var inWord = false;
        int count = 0;

        foreach (var ch in value)
        {
            var wasInWord = inWord;
            inWord = !ch.IsWhiteSpaceOrZero();
            if (inWord && !wasInWord) count++;
        }

        return count;
    }

    public static int CountLeadingWhitespace(this string? value)
    {
        for (var i = 0; i < value.Length; i++)
        {
            if (value[i] != ' ') return i;
        }
        return value.Length;
    }

    public static IList<Range> Split<T>(this ReadOnlySpan<T> span, ReadOnlySpan<T> delimiter) where T : IEquatable<T>
    {
        int startNext = 0;

        IList<Range> results = new List<Range>();

        while (startNext <= span.Length)
        {
            var slice = span.Slice(startNext);
            var startCurrent = startNext;

            int separatorIndex = slice.IndexOf(delimiter);
            int elementLength = (separatorIndex != -1 ? separatorIndex : slice.Length);

            var endCurrent = startCurrent + elementLength;
            startNext = endCurrent + delimiter.Length;

            results.Add(new Range(startCurrent, endCurrent));
        }

        return results;
    }

    public static IList<Range> Split<T>(this ReadOnlySpan<T> span, T delimiter) where T : IEquatable<T>
    {
        int startNext = 0;

        IList<Range> results = new List<Range>();

        while (startNext <= span.Length)
        {
            var slice = span.Slice(startNext);
            var startCurrent = startNext;

            int separatorIndex = slice.IndexOf(delimiter);
            int elementLength = (separatorIndex != -1 ? separatorIndex : slice.Length);

            var endCurrent = startCurrent + elementLength;
            startNext = endCurrent + 1;

            results.Add(new Range(startCurrent, endCurrent));
        }

        return results;
    }
}