namespace SunamoReflection.Args;

/// <summary>
/// Represents the dump as string args type.
/// </summary>
public class DumpAsStringArgs : DumpAsStringHeaderArgsReflection
{
    /// <summary>
    /// Gets or sets the provider.
    /// </summary>
    public DumpProvider Provider { get; set; } = DumpProvider.Yaml;

    /// <summary>
    /// Gets or sets the delimiter.
    /// </summary>
    public string Delimiter { get; set; } = " - ";

    /// <summary>
    /// Gets or sets the name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the object.
    /// </summary>
    public object Object { get; set; } = null!;

    // When true, dumps only values without property names. Good for fast comparing objects.
    /// <summary>
    /// Gets or sets the is only values.
    /// </summary>
    public bool IsOnlyValues { get; set; }
}
