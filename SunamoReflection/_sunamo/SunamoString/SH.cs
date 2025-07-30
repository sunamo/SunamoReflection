namespace SunamoReflection._sunamo.SunamoString;

internal class SH
{
    //    internal static Func<object, string> NullToStringOrDefault;
    //    internal static Func<string[], string> ConcatIfBeforeHasValue;
    //    internal static Func<object, object, string> ListToString;

    internal static string NullToStringOrDefault(object n)
    {
        //return NullToStringOrDefault(n, null);
        return n == null ? " " + "(null)" : " " + n;
    }

    internal static string ConcatIfBeforeHasValue(params string[] className)
    {
        var result = new StringBuilder();
        for (var i = 0; i < className.Length; i++)
        {
            var even = className[i];
            if (!string.IsNullOrWhiteSpace(even))
                //string odd =
                result.Append(even + className[++i]);
        }

        return result.ToString();
    }





}