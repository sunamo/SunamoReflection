namespace SunamoReflection;

using PropertyDescriptor = YamlDotNet.Serialization.PropertyDescriptor;

public partial class RH
{
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

    public static List<string> GetPropertyNames(Type type)
    {
        PropertyInfo[] properties = type.GetProperties(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
        return properties.Select(property => property.Name).ToList();
    }

    public static string FullPathCodeEntity(Type type)
    {
        return type.Namespace + "." + type.Name;
    }

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

    public static object? GetValueOfPropertyOrField(object instance, string name)
    {
        var type = instance.GetType();
        var value = GetValueOfProperty(name, type, instance, false);
        if (value == null)
            value = GetValueOfField(name, type, instance, false);
        return value;
    }

    public static object? GetValueOfField(string name, Type type, object instance, bool isIgnoringCase)
    {
        var fields = type.GetFields();
        return GetValue(name, type, instance, fields, isIgnoringCase, null);
    }

    public static object? GetValueOfProperty(string name, Type type, object instance, bool isIgnoringCase)
    {
        var properties = type.GetProperties();
        return GetValue(name, type, instance, properties, isIgnoringCase, null);
    }

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
