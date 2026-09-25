namespace SunamoReflection._public.SunamoArgs;

/// <summary>
/// Header arguments for dump-as-string operations with optional name filtering.
/// </summary>
public class DumpAsStringHeaderArgsReflection
{
    /// <summary>
    /// Default instance with empty name filter.
    /// </summary>
    public static DumpAsStringHeaderArgsReflection Default = new();

    /// <summary>
    /// List of property/field names to include in the dump. Prefix with "!" to exclude.
    /// </summary>
    public List<string> OnlyNames { get; set; } = new();
}
