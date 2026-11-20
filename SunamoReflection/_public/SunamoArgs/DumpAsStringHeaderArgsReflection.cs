namespace SunamoReflection._public.SunamoArgs;

/// <summary>
/// Represents the dump as string header args reflection type.
/// </summary>
public class DumpAsStringHeaderArgsReflection
{
    /// <summary>
    /// The default.
    /// </summary>
    public static DumpAsStringHeaderArgsReflection Default = new();

    /// <summary>
    /// Gets or sets the only names.
    /// </summary>
    public List<string> OnlyNames { get; set; } = new();
}
