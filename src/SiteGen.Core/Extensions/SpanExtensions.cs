namespace SiteGen.Core.Extensions;

public static class SpanExtensions
{
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