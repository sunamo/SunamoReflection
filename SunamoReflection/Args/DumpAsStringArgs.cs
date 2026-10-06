namespace SunamoReflection.Args;

public class DumpAsStringArgs : DumpAsStringHeaderArgsReflection
{
    public DumpProvider Provider { get; set; } = DumpProvider.Yaml;

    public string Delimiter { get; set; } = " - ";

    public string Name { get; set; } = string.Empty;

    public object Object { get; set; } = null!;

    // When true, dumps only values without property names. Good for fast comparing objects.
    public bool IsOnlyValues { get; set; }
}
