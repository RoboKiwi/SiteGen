namespace SiteGen.Core.Extensions;

public static class StringCaseExtensions
{
    public static string ToCamelCase(this string value)
    {
        var normalized = DelimitWords(value);
        if (normalized == null) return null;

        Span<char> buffer = stackalloc char[normalized.Length];

        var length = 0;

        for (int i = 0; i < normalized.Length; i++)
        {
            if (i == 0)
            {
                buffer[length++] = char.ToLower(normalized[i]);
                continue;
            }
            else if (i > 0 && normalized[i - 1] == '\0')
            {
                buffer[length++] = char.ToUpper(normalized[i]);
                continue;
            }

            if (normalized[i] == '\0') continue;

            buffer[length++] = normalized[i];
        }

        return buffer.Slice(0, length).ToString();
    }

    public static string ToSnakeCase(this string value)
    {
        return ToDelimitedWords(value, '_');
    }
    public static string ToKebabCase(this string value)
    {
        return ToDelimitedWords(value, '-');
    }

    public static string ToDelimitedWords(this string value, char delimiter, TitleCaseOptions titleCaseOptions = TitleCaseOptions.None)
    {
        var normalized = DelimitWords(value);
        if (string.IsNullOrEmpty(normalized)) return normalized;

        Span<char> buffer = stackalloc char[normalized.Length];

        var length = 0;

        for (int i = 0; i < normalized.Length; i++)
        {
            if (titleCaseOptions != TitleCaseOptions.None && (i == 0 || (titleCaseOptions == TitleCaseOptions.AllWords && i > 0 && normalized[i - 1] == '\0')))
            {
                buffer[length++] = char.ToUpper(normalized[i]);
                continue;
            }

            if (normalized[i] == '\0')
            {
                buffer[length++] = delimiter;
            }
            else
            {
                buffer[length++] = normalized[i];
            }
        }

        return buffer.Slice(0, length).ToString();
    }

    public static string ToPascalCase(this string value)
    {
        var normalized = DelimitWords(value);
        if (normalized == null) return null;
        Span<char> buffer = stackalloc char[normalized.Length];

        var length = 0;

        for (int i = 0; i < normalized.Length; i++)
        {
            if (i == 0 || (i > 0 && normalized[i - 1] == '\0'))
            {
                buffer[length++] = char.ToUpper(normalized[i]);
                continue;
            }

            if (normalized[i] == '\0') continue;

            buffer[length++] = normalized[i];
        }

        return buffer.Slice(0, length).ToString();
    }

    /// <summary>
    /// Finds all words in <paramref name="value"/> and delimits them with <paramref name="delimiter"/>. All
    /// characters are changed to lower case, and any non-alphanumeric characters are stripped and replaced with
    /// <paramref name="delimiter"/>.
    /// </summary>
    /// <remarks>
    /// Any leading, trailing or consecutive delimiters are stripped.
    /// </remarks>
    /// <param name="value"></param>
    /// <param name="delimiter"></param>
    /// <returns></returns>
    public static string DelimitWords(this string value, char delimiter = '\0')
    {
        if (string.IsNullOrEmpty(value)) return value;
        Span<char> buffer = stackalloc char[value.Length * 2];

        var length = 0;

        for (int i = 0; i < value.Length; i++)
        {
            // Strip non-alphanumeric
            if (!char.IsLetterOrDigit(value[i]))
            {
                // A non-alphanumeric character will always signify a word boundary,
                // but make sure we prevent consecutive delimiters.
                if (length > 0 && buffer[length - 1] != delimiter)
                {
                    buffer[length++] = delimiter;
                }
                continue;
            }

            // Lowercase characters we can just push onto the result
            if (!char.IsUpper(value[i]))
            {
                buffer[length++] = value[i];
                continue;
            }

            // WORDBoundary
            //     ^ are we here? We should split into "word" and "boundary"
            if (i > 0 && i < (value.Length - 1) && (char.IsUpper(value[i - 1]) || char.IsNumber(value[i - 1])) && char.IsLower(value[i + 1]))
            {
                buffer[length++] = delimiter;
            }
            // wordBoundary
            //     ^ Are we here?
            else if (i > 0 && (char.IsLower(value[i - 1]) || char.IsNumber(value[i - 1])))
            {
                buffer[length++] = delimiter;
            }

            buffer[length++] = char.ToLower(value[i]);
        }

        var start = 0;

        // Trim delimiter from start and end of the result
        while (buffer[start] == delimiter) start++;
        while (buffer[length - 1] == delimiter) length--;

        return buffer.Slice(start, length).ToString();
    }
}