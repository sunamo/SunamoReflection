namespace SunamoReflection;

using PropertyDescriptor = YamlDotNet.Serialization.PropertyDescriptor;

public partial class RH
{
    /// <summary>
    /// Dumps an object as a string using default header arguments with delimiter-separated values.
    /// Mainly for fast comparing objects. Only OnlyNames can be specified via DumpAsStringHeaderArgsReflection.
    /// For more customization, use DumpAsString with DumpAsStringArgs directly.
    /// </summary>
    /// <param name="instance">The object to dump.</param>
    /// <param name="args">Optional header arguments for name filtering.</param>
    /// <returns>Delimiter-separated string of object values.</returns>
    public static string DumpAsString3(object instance, DumpAsStringHeaderArgsReflection? args = null)
    {
        if (args == null)
            args = DumpAsStringHeaderArgsReflection.Default;
        var dumpArgs = new DumpAsStringArgs
        {
            Object = instance,
            Delimiter = "-",
            IsOnlyValues = true,
            OnlyNames = args.OnlyNames
        };
        return DumpAsString(dumpArgs);
    }

    /// <summary>
    /// Gets string values of all fields from an object, optionally filtered by name.
    /// </summary>
    /// <param name="instance">The object to inspect.</param>
    /// <param name="onlyNames">Optional filter for specific field names.</param>
    /// <returns>List of field values as strings.</returns>
    public static List<string> GetValuesOfField(object instance, params string[] onlyNames)
    {
        return GetValuesOfField(instance, onlyNames);
    }

    /// <summary>
    /// Gets string values of all fields from an object with control over value-only output.
    /// </summary>
    /// <param name="instance">The object to inspect.</param>
    /// <param name="onlyNames">Filter for specific field names.</param>
    /// <param name="isOnlyValues">When true, returns only values without field names.</param>
    /// <returns>List of field values as strings.</returns>
    public static List<string> GetValuesOfField(object instance, IList<string> onlyNames, bool isOnlyValues)
    {
        var type = instance.GetType();
        var fields = type.GetFields();
        var values = new List<string>(fields.Length);
        foreach (var item in fields)
        {
            if (onlyNames.Count > 0)
                if (!onlyNames.Contains(item.Name))
                    continue;
            AddValue(values, item.Name, GetValueOfField(item.Name, type, instance, false)?.ToString() ?? string.Empty, isOnlyValues);
        }

        return values;
    }

    /// <summary>
    /// Recursively prints the public properties of a type and its nested complex types.
    /// This method was useful in usysu where StackOverflowException occurred.
    /// </summary>
    /// <param name="stringBuilder">The StringBuilder to append output to.</param>
    /// <param name="type">The type to inspect.</param>
    /// <param name="indent">The indentation prefix for nested levels.</param>
    public static void PrintPublicPropertiesRecursively(StringBuilder stringBuilder, Type? type, string indent = "  ")
    {
        if (type == null)
        {
            return;
        }

        stringBuilder.AppendLine($"{indent}Object Type: {type.Name}");
        PropertyInfo[] properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
        foreach (PropertyInfo property in properties)
        {
            string typeName = property.PropertyType.Name;
            try
            {
                if (property.PropertyType.IsPrimitive || property.PropertyType == typeof(string) || property.PropertyType.IsValueType)
                {
                    stringBuilder.AppendLine($"{indent}- {property.Name} ({typeName})");
                }
                else
                {
                    stringBuilder.AppendLine($"{indent}- {property.Name} ({typeName}):");
                    PrintPublicPropertiesRecursively(stringBuilder, property.PropertyType, indent + "  ");
                }
            }
            catch (Exception exception)
            {
                stringBuilder.AppendLine($"{indent}- {property.Name} ({typeName}): Error getting value - {exception.Message}");
            }
        }
    }

    /// <summary>
    /// Gets property values of an object, falling back to fields if no properties exist.
    /// Supports negation filters with "!" prefix in onlyNames.
    /// </summary>
    /// <param name="instance">The object to inspect.</param>
    /// <param name="onlyNames">Filter for specific property names. Prefix with "!" to exclude.</param>
    /// <param name="isOnlyValues">When true, returns only values without property names.</param>
    /// <returns>List of property/field values as strings.</returns>
    public static List<string> GetValuesOfProperty2(object instance, List<string> onlyNames, bool isOnlyValues)
    {
        var filterNames = onlyNames.ToList();
        var values = new List<string>();
        var properties = GetProps(instance);
        var isAllNegated = true;
        foreach (var item in onlyNames)
            if (!item.StartsWith("!"))
                isAllNegated = false;
        if (properties.Count == 0)
        {
            var fieldList = GetFields(instance);
            foreach (var descriptor in fieldList)
                GetValue(descriptor, isAllNegated, onlyNames, filterNames, instance, values, isOnlyValues);
        }
        else
        {
            foreach (var descriptor in properties)
                GetValue(descriptor, isAllNegated, onlyNames, filterNames, instance, values, isOnlyValues);
        }

        return values;
    }

    /// <summary>
    /// Extracts the value of a member (property or field) and adds it to the values list, applying name filters.
    /// </summary>
    /// <param name="descriptor">The member to extract the value from.</param>
    /// <param name="isAllNegated">Whether all filter names are negation filters.</param>
    /// <param name="onlyNames">The original name filter list.</param>
    /// <param name="filterNames">Working copy of the filter list.</param>
    /// <param name="instance">The object instance.</param>
    /// <param name="values">The list to add extracted values to.</param>
    /// <param name="isOnlyValues">When true, adds only the value without the name prefix.</param>
    public static void GetValue(MemberInfo descriptor, bool isAllNegated, List<string> onlyNames, List<string> filterNames, object instance, List<string> values, bool isOnlyValues)
    {
        var isAdding = true;
        var name = descriptor.Name;
        if (onlyNames.Contains("!" + name))
            return;
        if (filterNames.Count > 0)
        {
            if (isAllNegated)
            {
                if (filterNames.Contains("!" + name))
                    isAdding = false;
            }
            else
            {
                if (!filterNames.Contains(name))
                    isAdding = false;
            }
        }

        if (isAdding)
        {
            var value = GetValue(instance, descriptor);
            AddValue(values, name, value?.ToString() ?? string.Empty, isOnlyValues);
        }
    }

    private static object? GetValue(object instance, MemberInfo[] members, object? value)
    {
        return GetValue(instance, members);
    }

    private static object? GetValue(object instance, params MemberInfo[] members)
    {
        var member = members[0];
        if (member is PropertyInfo propertyInfo)
        {
            return propertyInfo.GetValue(instance);
        }

        if (member is FieldInfo fieldInfo)
        {
            return fieldInfo.GetValue(instance);
        }

        return null;
    }

    /// <summary>
    /// Gets a member value by name from an object instance, searching in the provided member list.
    /// </summary>
    /// <param name="name">The member name to find.</param>
    /// <param name="type">The type containing the member.</param>
    /// <param name="instance">The object instance.</param>
    /// <param name="members">The list of members to search in.</param>
    /// <param name="isIgnoringCase">Whether to ignore case when matching names.</param>
    /// <param name="value">Unused parameter (reserved for set operations).</param>
    /// <returns>The member value or null if not found.</returns>
    public static object? GetValue(string name, Type type, object instance, IList members, bool isIgnoringCase, object? value)
    {
        return GetOrSetValue(name, type, instance, members, isIgnoringCase, GetValue, value);
    }

    /// <summary>
    /// Gets or sets a member value by name, using the provided delegate for the actual get/set operation.
    /// </summary>
    /// <param name="name">The member name to find.</param>
    /// <param name="type">The type containing the member.</param>
    /// <param name="instance">The object instance.</param>
    /// <param name="members">The list of members to search in.</param>
    /// <param name="isIgnoringCase">Whether to ignore case when matching names.</param>
    /// <param name="getOrSet">The delegate to perform the actual get or set operation.</param>
    /// <param name="value">The value to set (used only for set operations).</param>
    /// <returns>The member value for get operations, or null.</returns>
    public static object? GetOrSetValue(string name, Type type, object instance, IList members, bool isIgnoringCase, Func<object, MemberInfo[], object?, object?> getOrSet, object? value)
    {
        if (isIgnoringCase)
        {
            name = name.ToLower();
            foreach (MemberInfo item in members)
                if (item.Name.ToLower() == name)
                {
                    var memberArray = type.GetMember(name);
                    if (memberArray != null)
                        return getOrSet(instance, memberArray, value);
                }
        }
        else
        {
            foreach (MemberInfo item in members)
                if (item.Name == name)
                {
                    var memberArray = type.GetMember(name);
                    if (memberArray != null)
                        return getOrSet(instance, memberArray, value);
                }
        }

        return null;
    }

    private static void AddValue(List<string> values, string name, string value, bool isOnlyValue)
    {
        if (isOnlyValue)
            values.Add(value);
        else
            values.Add($"{name}: {value}");
    }

    /// <summary>
    /// Dumps an object using the ObjectDumper.NET library.
    /// </summary>
    /// <param name="instance">The object to dump.</param>
    /// <returns>String representation from ObjectDumper.NET.</returns>
    public static string DumpAsObjectDumperNet(object instance)
    {
        return ObjectDumper.Dump(instance);
    }
}
