namespace SunamoReflection;


/// <summary>
///     Mus� b�t v SunamoArgs proto�e je sd�lena ve SunamoReflection i SunamoCollectionWithoutDuplicates
/// </summary>
public class DumpAsStringHeaderArgsReflection
{
    internal static DumpAsStringHeaderArgsReflection Default = new();
    /// <summary>
    ///     Only names of properties to get
    ///     If starting with ! => surely delete
    /// </summary>
    internal List<string> onlyNames = new();
}