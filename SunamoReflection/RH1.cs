namespace SunamoReflection;

using PropertyDescriptor = YamlDotNet.Serialization.PropertyDescriptor;

public partial class RH
{
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

    public static List<string> GetValuesOfField(object instance, params string[] onlyNames)
    {
        return GetValuesOfField(instance, onlyNames);
    }

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

    // This method was useful in usysu where StackOverflowException occurred.
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

    // Supports negation filters with "!" prefix in onlyNames.
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

    public static object? GetValue(string name, Type type, object instance, IList members, bool isIgnoringCase, object? value)
    {
        return GetOrSetValue(name, type, instance, members, isIgnoringCase, GetValue, value);
    }

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

    public static string DumpAsObjectDumperNet(object instance)
    {
        return ObjectDumper.Dump(instance);
    }
}
