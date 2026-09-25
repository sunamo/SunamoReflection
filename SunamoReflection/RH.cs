namespace SunamoReflection;

using PropertyDescriptor = YamlDotNet.Serialization.PropertyDescriptor;

/// <summary>
/// Reflection helper providing dump, property/field access, and type inspection utilities.
/// </summary>
public partial class RH
{
    /// <summary>
    /// Serializes an object to XML string. Returns the exception text if serialization fails.
    /// </summary>
    /// <param name="output">The object to serialize to XML.</param>
    /// <returns>XML string representation of the object.</returns>
    public static string DumpAsXml(object output)
    {
        string objectAsXmlString;
        XmlSerializer serializer = new(output.GetType());
        using (StringWriter writer = new())
        {
            try
            {
                serializer.Serialize(writer, output);
                objectAsXmlString = writer.ToString();
            }
            catch (Exception exception)
            {
                objectAsXmlString = exception.ToString();
            }
        }

        return objectAsXmlString;
    }

    /// <summary>
    /// Checks whether the child type is or derives from the parent type, including interface checks.
    /// </summary>
    /// <param name="children">The child type to check.</param>
    /// <param name="parent">The parent type to check against.</param>
    /// <param name="isAllowingString">Whether to allow string type as a valid child.</param>
    /// <returns>True if children is or derives from parent.</returns>
    public static bool IsOrIsDeriveFromBaseClass(Type? children, Type parent, bool isAllowingString = true)
    {
        if (children == typeof(string) && !isAllowingString)
            return false;
        if (children == null)
            ThrowEx.IsNull("children", children);
        while (true)
        {
            if (children == null)
                return false;
            if (children == parent)
                return true;
            foreach (var interfaceType in children.GetInterfaces())
                if (interfaceType == parent)
                    return true;
            children = children.BaseType;
        }
    }

    /// <summary>
    /// Gets all property names declared on the specified type (not inherited), regardless of access modifier.
    /// </summary>
    /// <param name="type">The type to inspect.</param>
    /// <returns>List of property names.</returns>
    public static List<string> GetPropertyNames(Type type)
    {
        PropertyInfo[] properties = type.GetProperties(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
        return properties.Select(property => property.Name).ToList();
    }

    /// <summary>
    /// Returns the full path of a code entity (Namespace.Name).
    /// </summary>
    /// <param name="type">The type to get the full path for.</param>
    /// <returns>Full namespace-qualified name.</returns>
    public static string FullPathCodeEntity(Type type)
    {
        return type.Namespace + "." + type.Name;
    }

    /// <summary>
    /// Finds a loaded assembly by name, trying exact name, full name, and partial match.
    /// </summary>
    /// <param name="name">The assembly name to search for.</param>
    /// <returns>The matching assembly or null.</returns>
    public static Assembly? AssemblyWithName(string name)
    {
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();
        var result = assemblies.Where(assembly => assembly.GetName().Name == name);
        if (!result.Any())
            result = assemblies.Where(assembly => assembly.FullName == name);
        if (!result.Any())
            result = assemblies.Where(assembly => assembly.FullName != null && assembly.FullName.Contains(name));
        return result.FirstOrDefault();
    }

    private static List<PropertyInfo> GetProps(object instance)
    {
        var instanceType = GetType(instance);
        var result = instanceType.GetProperties().ToList();
        return result;
    }

    private static Type GetType(object instance)
    {
        var objectType = instance.GetType();
        if (IsType(objectType))
            return (instance as Type) ?? objectType;
        return objectType;
    }

    /// <summary>
    /// Gets all public fields of an object. The parameter can be a Type or an instance.
    /// </summary>
    /// <param name="instance">The object or Type to inspect.</param>
    /// <returns>List of public fields.</returns>
    public static List<FieldInfo> GetFields(object instance)
    {
        var objectType = instance.GetType();
        Type instanceType;
        if (IsType(objectType))
            instanceType = (instance as Type) ?? objectType;
        else
            instanceType = objectType;
        var result = instanceType.GetFields().ToList();
        return result;
    }

    private static bool IsType(Type objectType)
    {
        var typeOfType = typeof(Type);
        return objectType.FullName == "System.RuntimeType" || objectType == typeOfType;
    }

    /// <summary>
    /// Gets string values of all constants defined on the specified type.
    /// </summary>
    /// <param name="type">The type to inspect for constants.</param>
    /// <returns>Trimmed string values of the constants.</returns>
    public static List<string> GetValuesOfConsts(Type type)
    {
        var constants = GetConsts(type);
        var result = new List<string>();
        foreach (var item in constants)
            result.Add(SH.NullToStringOrDefault(item.GetValue(null)!));
        for (var i = 0; i < result.Count; i++)
            result[i] = result[i].Trim();
        return result;
    }

    /// <summary>
    /// Gets constant values as a dictionary of name-value pairs, optionally filtered by name.
    /// </summary>
    /// <param name="type">The type to inspect for constants.</param>
    /// <param name="onlyNames">Optional filter for specific constant names.</param>
    /// <returns>Dictionary mapping constant names to their string values.</returns>
    public static Dictionary<string, string> GetValuesOfConsts(Type type, params string[] onlyNames)
    {
        var constants = GetConsts(type);
        var values = new Dictionary<string, string>(constants.Count);
        foreach (var item in constants)
        {
            if (onlyNames.Length > 0)
                if (!onlyNames.Contains(item.Name))
                    continue;
            var fieldValue = GetValueOfField(item.Name, type, null!, false);
            values.Add(item.Name, fieldValue?.ToString() ?? string.Empty);
        }

        return values;
    }

    /// <summary>
    /// Gets the value of a property or field by name from an object instance.
    /// </summary>
    /// <param name="instance">The object instance to inspect.</param>
    /// <param name="name">The name of the property or field.</param>
    /// <returns>The value of the property or field.</returns>
    public static object? GetValueOfPropertyOrField(object instance, string name)
    {
        var type = instance.GetType();
        var value = GetValueOfProperty(name, type, instance, false);
        if (value == null)
            value = GetValueOfField(name, type, instance, false);
        return value;
    }

    /// <summary>
    /// Gets the value of a field by name from an object instance.
    /// </summary>
    /// <param name="name">The field name.</param>
    /// <param name="type">The type containing the field.</param>
    /// <param name="instance">The object instance (null for static fields).</param>
    /// <param name="isIgnoringCase">Whether to ignore case when matching names.</param>
    /// <returns>The field value.</returns>
    public static object? GetValueOfField(string name, Type type, object instance, bool isIgnoringCase)
    {
        var fields = type.GetFields();
        return GetValue(name, type, instance, fields, isIgnoringCase, null);
    }

    /// <summary>
    /// Gets the value of a property by name from an object instance.
    /// </summary>
    /// <param name="name">The property name.</param>
    /// <param name="type">The type containing the property.</param>
    /// <param name="instance">The object instance.</param>
    /// <param name="isIgnoringCase">Whether to ignore case when matching names.</param>
    /// <returns>The property value.</returns>
    public static object? GetValueOfProperty(string name, Type type, object instance, bool isIgnoringCase)
    {
        var properties = type.GetProperties();
        return GetValue(name, type, instance, properties, isIgnoringCase, null);
    }

    /// <summary>
    /// Dumps a list of objects as a multi-line string with a header row of field names.
    /// </summary>
    /// <param name="args">Dump arguments containing the list object.</param>
    /// <param name="isRemovingNull">Whether to remove null entries from the list before dumping.</param>
    /// <returns>Formatted string representation of the list.</returns>
    public static string DumpListAsString(DumpAsStringArgs args, bool isRemovingNull = false)
    {
        var stringBuilder = new StringBuilder();
        var list = (List<object>)args.Object;
        if (isRemovingNull)
            list.RemoveAll(element => element == null);
        if (list.Count > 0)
        {
            stringBuilder.AppendLine(NameOfFieldsFromDump(list.First(), args));
            foreach (var item in list)
            {
                args.Object = item;
                stringBuilder.AppendLine(DumpAsString(args));
            }
        }

        return stringBuilder.ToString();
    }

    /// <summary>
    /// Dumps a list of objects as a compact one-line-per-item string with a header.
    /// </summary>
    /// <param name="operation">The operation name to display in the header.</param>
    /// <param name="list">The list of objects to dump.</param>
    /// <param name="args">Header arguments controlling which fields to include.</param>
    /// <returns>Formatted string representation.</returns>
    public static string DumpListAsStringOneLine(string operation, IList list, DumpAsStringHeaderArgsReflection args)
    {
        if (list.Count > 0)
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("***");
            stringBuilder.AppendLine(operation + " " + "(" + list.Count + ")" + ":");
            var headerObject = list.Count > 0 ? list[0] : null;
            if (headerObject != null)
                stringBuilder.AppendLine(NameOfFieldsFromDump(headerObject, args));
            foreach (var item in list)
            {
                stringBuilder.AppendLine(DumpAsString(new DumpAsStringArgs { Provider = DumpProvider.Reflection, Delimiter = "-", Object = item!, IsOnlyValues = true, OnlyNames = args.OnlyNames }));
            }

            stringBuilder.AppendLine("***");
            return stringBuilder.ToString();
        }

        return string.Empty;
    }
}
