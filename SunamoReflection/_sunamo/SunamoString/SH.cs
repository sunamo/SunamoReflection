namespace SunamoReflection._sunamo.SunamoString;

internal class SH
{
    internal static string NullToStringOrDefault(object value)
    {
        return value == null ? " " + "(null)" : " " + value;
    }

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
