namespace SunamoReflection.Enums;

/// <summary>
/// Specifies the provider used for dumping objects as strings.
/// </summary>
public enum DumpProvider
{
    /// <summary>
    /// When using JsonParser returns empty.
    /// </summary>
    Json,

    /// <summary>
    /// Cannot be used on dynamic objects.
    /// </summary>
    Reflection,

    /// <summary>
    /// Throws NullReferenceException, DO NOT USE.
    /// </summary>
    Yaml,

    /// <summary>
    /// Added "Net" suffix because the original ObjectDumper package has not been updated for 12 years.
    /// Can be used on dynamic objects.
    /// </summary>
    ObjectDumperNet,

    /// <summary>
    /// Cannot be used on dynamic objects.
    /// To be XML serializable, types which inherit from IEnumerable must have an implementation of Add(System.Object) at
    /// all levels of their inheritance hierarchy. System.Dynamic.ExpandoObject does not implement Add(System.Object).
    /// </summary>
    Xml
}