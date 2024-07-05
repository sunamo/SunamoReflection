namespace SunamoReflection.Enums;

public enum DumpProvider
{
    /// <summary>
    /// When use JsonParser return empty.
    /// </summary>
    Json,
    /// <summary>
    /// Nejde použít na dynamic object
    /// </summary>
    Reflection,
    /// <summary>
    /// Throw NullReferenceException, DONT USE
    /// </summary>
    Yaml,
    /// <summary>
    /// Přidal jsem Net, protože ObjectDumper package je 12 let bez update
    ///
    /// Can be used on dynamic
    /// </summary>
    ObjectDumperNet,
    /// <summary>
    /// Taky nejde použít na dynamic object
    /// To be XML serializable, types which inherit from IEnumerable must have an implementation of Add(System.Object) at all levels of their inheritance hierarchy. System.Dynamic.ExpandoObject does not implement Add(System.Object).'
    /// </summary>
    Xml
}
