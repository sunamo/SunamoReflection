namespace SunamoReflection.Args;

/// <summary>
///     U��v� se v SunamoReflection+SUnamoLogger
/// </summary>
public class DumpAsStringArgs : DumpAsStringHeaderArgsReflection
{
    public DumpProvider d = DumpProvider.Yaml;
    public string deli = " - ";
    public string name = string.Empty;
    public object o;

    /// <summary>
    ///     Good for fast comparing objects
    /// </summary>
    public bool onlyValues;
}