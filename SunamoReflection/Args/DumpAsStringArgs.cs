namespace SunamoReflection.Args;

/// <summary>
/// Arguments for dump-as-string operations used in SunamoReflection and SunamoLogger.
/// </summary>
public class DumpAsStringArgs : DumpAsStringHeaderArgsReflection
{
    /// <summary>
    /// The dump provider to use for serialization.
    /// </summary>
    public DumpProvider Provider { get; set; } = DumpProvider.Yaml;

    /// <summary>
    /// Delimiter used between values when dumping.
    /// </summary>
    public string Delimiter { get; set; } = " - ";

    /// <summary>
    /// Optional name/header for the dump output.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// The object to dump.
    /// </summary>
    public object Object { get; set; } = null!;

    /// <summary>
    /// When true, dumps only values without property names. Good for fast comparing objects.
    /// </summary>
    public bool IsOnlyValues { get; set; }
}
