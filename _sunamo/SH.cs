namespace SunamoReflection;

//namespace SunamoReflection;

public class SH
{
    //    public static Func<object, string> NullToStringOrDefault;
    //    public static Func<string[], string> ConcatIfBeforeHasValue;
    //    public static Func<object, object, string> ListToString;

    public static string ConcatIfBeforeHasValue(params string[] className)
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
