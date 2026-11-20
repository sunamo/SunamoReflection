namespace SunamoReflection.Enums;

/// <summary>
/// Represents the dump provider type.
/// </summary>
public enum DumpProvider
{
    // When using JsonParser returns empty.
    /// <summary>
    /// The json value.
    /// </summary>
    Json,

    // Cannot be used on dynamic objects.
    /// <summary>
    /// The reflection value.
    /// </summary>
    Reflection,

    // Throws NullReferenceException, DO NOT USE.
    /// <summary>
    /// The yaml value.
    /// </summary>
    Yaml,

    // Added "Net" suffix because the original ObjectDumper package has not been updated for 12 years.
    // Can be used on dynamic objects.
    /// <summary>
    /// The object dumper net value.
    /// </summary>
    ObjectDumperNet,

    // Cannot be used on dynamic objects.
    // To be XML serializable, types which inherit from IEnumerable must have an implementation of Add(System.Object) at
    // all levels of their inheritance hierarchy. System.Dynamic.ExpandoObject does not implement Add(System.Object).
    /// <summary>
    /// The xml value.
    /// </summary>
    Xml
}
