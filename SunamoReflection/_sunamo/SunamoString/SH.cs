namespace SunamoReflection._sunamo.SunamoString;

/// <summary>
/// Internal string helper methods for the SunamoReflection library.
/// </summary>
internal class SH
{
    /// <summary>
    /// Converts an object to string, returning "(null)" if the object is null.
    /// </summary>
    /// <param name="value">The object to convert.</param>
    /// <returns>String representation prefixed with a space, or " (null)" if null.</returns>
    internal static string NullToStringOrDefault(object value)
    {
        return value == null ? " " + "(null)" : " " + value;
    }

    /// <summary>
    /// Concatenates pairs of strings where the first element of each pair is non-empty.
    /// Elements are processed in pairs: even-indexed elements are checked, odd-indexed are appended.
    /// </summary>
    /// <param name="parts">Alternating pairs of (value, separator) strings.</param>
    /// <returns>Concatenated result of non-empty pairs.</returns>
    internal static string ConcatIfBeforeHasValue(params string[] parts)
    {
        var result = new StringBuilder();
        for (var i = 0; i < parts.Length; i++)
        {
            var currentPart = parts[i];
            if (!string.IsNullOrWhiteSpace(currentPart))
                result.Append(currentPart + parts[++i]);
        }

        return result.ToString();
    }
}
