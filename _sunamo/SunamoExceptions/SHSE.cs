namespace SunamoReflection._sunamo.SunamoExceptions;
internal class SHSE
{
    internal static string NullToStringOrDefault(object n)
    {
        //return NullToStringOrDefault(n, null);
        return n == null ? " " + Consts.nulled : AllStrings.space + n;
    }
}
