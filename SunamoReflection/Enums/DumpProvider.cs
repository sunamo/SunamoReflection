namespace SunamoReflection.Enums;

public enum DumpProvider
{
    // When using JsonParser returns empty.
    Json,

    // Cannot be used on dynamic objects.
    Reflection,

    // Throws NullReferenceException, DO NOT USE.
    Yaml,

    // Added "Net" suffix because the original ObjectDumper package has not been updated for 12 years.
    // Can be used on dynamic objects.
    ObjectDumperNet,

    // Cannot be used on dynamic objects.
    // To be XML serializable, types which inherit from IEnumerable must have an implementation of Add(System.Object) at
    // all levels of their inheritance hierarchy. System.Dynamic.ExpandoObject does not implement Add(System.Object).
    Xml
}
