namespace SunamoReflection._sunamo.SunamoString;

internal class SH
{
    //    internal static Func<object, string> NullToStringOrDefault;
    //    internal static Func<string[], string> ConcatIfBeforeHasValue;
    //    internal static Func<object, object, string> ListToString;

    internal static string NullToStringOrDefault(object n)
    {
        //return NullToStringOrDefault(n, null);
        return n == null ? " " + Consts.nulled : AllStrings.space + n;
    }

    internal static string ConcatIfBeforeHasValue(params string[] className)
    {
        StringBuilder result = new StringBuilder();
        for (int i = 0; i < className.Length; i++)
        {
            string even = className[i];
            if (!string.IsNullOrWhiteSpace(even))
            {
                //string odd =
                result.Append(even + className[++i]);
            }
        }
        return result.ToString();
    }
}
