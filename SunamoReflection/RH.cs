namespace SunamoReflection;

using PropertyDescriptor = YamlDotNet.Serialization.PropertyDescriptor;

/// <summary>
/// Represents the RH type.
/// </summary>
public partial class RH
{
    /// <summary>
    /// Dump as xml.
    /// </summary>
    /// <param name="output">The output.</param>
    /// <returns>The dump as xml result.</returns>
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
    /// Is or is derive from base class.
    /// </summary>
    /// <param name="children">The children.</param>
    /// <param name="parent">The parent.</param>
    /// <param name="isAllowingString">The is allowing string.</param>
    /// <returns>The is or is derive from base class result.</returns>
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
    /// Get property names.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <returns>The get property names result.</returns>
    public static List<string> GetPropertyNames(Type type)
    {
        PropertyInfo[] properties = type.GetProperties(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
        return properties.Select(property => property.Name).ToList();
    }

    /// <summary>
    /// Full path code entity.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <returns>The full path code entity result.</returns>
    public static string FullPathCodeEntity(Type type)
    {
        return type.Namespace + "." + type.Name;
    }

    /// <summary>
    /// Assembly with name.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <returns>The assembly with name result.</returns>
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
    /// Get fields.
    /// </summary>
    /// <param name="instance">The instance.</param>
    /// <returns>The get fields result.</returns>
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
    /// Get values of consts.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <returns>The get values of consts result.</returns>
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
    /// Get values of consts.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <param name="onlyNames">The only names.</param>
    /// <returns>The get values of consts result.</returns>
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
    /// Get value of property or field.
    /// </summary>
    /// <param name="instance">The instance.</param>
    /// <param name="name">The name.</param>
    /// <returns>The get value of property or field result.</returns>
    public static object? GetValueOfPropertyOrField(object instance, string name)
    {
        var type = instance.GetType();
        var value = GetValueOfProperty(name, type, instance, false);
        if (value == null)
            value = GetValueOfField(name, type, instance, false);
        return value;
    }

    /// <summary>
    /// Get value of field.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="type">The type.</param>
    /// <param name="instance">The instance.</param>
    /// <param name="isIgnoringCase">The is ignoring case.</param>
    /// <returns>The get value of field result.</returns>
    public static object? GetValueOfField(string name, Type type, object instance, bool isIgnoringCase)
    {
        var fields = type.GetFields();
        return GetValue(name, type, instance, fields, isIgnoringCase, null);
    }

    /// <summary>
    /// Get value of property.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="type">The type.</param>
    /// <param name="instance">The instance.</param>
    /// <param name="isIgnoringCase">The is ignoring case.</param>
    /// <returns>The get value of property result.</returns>
    public static object? GetValueOfProperty(string name, Type type, object instance, bool isIgnoringCase)
    {
        var properties = type.GetProperties();
        return GetValue(name, type, instance, properties, isIgnoringCase, null);
    }

    /// <summary>
    /// Dump list as string.
    /// </summary>
    /// <param name="args">The args.</param>
    /// <param name="isRemovingNull">The is removing null.</param>
    /// <returns>The dump list as string result.</returns>
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
    /// Dump list as string one line.
    /// </summary>
    /// <param name="operation">The operation.</param>
    /// <param name="list">The list.</param>
    /// <param name="args">The args.</param>
    /// <returns>The dump list as string one line result.</returns>
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
