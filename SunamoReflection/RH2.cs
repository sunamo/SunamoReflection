namespace SunamoReflection;

using PropertyDescriptor = YamlDotNet.Serialization.PropertyDescriptor;

public partial class RH
{
    // When serializing ISymbol, execution may take unlimited time.
    public static string DumpAsString(DumpAsStringArgs args)
    {
        string? dump = null;
        if (args.Object.GetType() == typeof(string))
            dump = args.Object.ToString();
        else
            switch (args.Provider)
            {
                case DumpProvider.Xml:
                    dump = DumpAsXml(args.Object);
                    break;
                case DumpProvider.ObjectDumperNet:
                    return DumpAsObjectDumperNet(args.Object);
                case DumpProvider.Yaml:
                case DumpProvider.Json:
                case DumpProvider.Reflection:
                    dump = string.Join(args.IsOnlyValues ? args.Delimiter : Environment.NewLine, GetValuesOfProperty2(args.Object, args.OnlyNames, args.IsOnlyValues));
                    break;
                default:
                    ThrowEx.NotImplementedCase(args.Provider);
                    break;
            }

        if (string.IsNullOrWhiteSpace(args.Name))
            return dump ?? string.Empty;
        return args.Name + Environment.NewLine + dump;
    }

    public static string DumpAsReflection(object instance)
    {
        return DumpAsString(new DumpAsStringArgs { Provider = DumpProvider.Reflection, Object = instance });
    }

    private static string NameOfFieldsFromDump(object instance, DumpAsStringHeaderArgsReflection headerArgs)
    {
        var properties = TypeDescriptor.GetProperties(instance);
        var sourceList = new List<string>();
        foreach (PropertyDescriptor descriptor in properties)
        {
            var name = descriptor.Name;
            if (headerArgs.OnlyNames.Contains("!" + name))
                continue;
            sourceList.Add(name);
        }

        return string.Join("-", sourceList);
    }

    public static string DumpAsString3Dictionary3<TKey1, TKey2, TValue>(string operation, Dictionary<TKey1, Dictionary<TKey2, List<TValue>>> grouped) where TKey1 : notnull where TKey2 : notnull
    {
        var stringBuilder = new StringBuilder();
        stringBuilder.AppendLine(operation);
        foreach (var item in grouped)
        {
            stringBuilder.AppendLine("1) " + item.Key);
            foreach (var innerEntry in item.Value)
            {
                stringBuilder.AppendLine("2) " + innerEntry.Key);
                foreach (var value in innerEntry.Value)
                    stringBuilder.AppendLine(value?.ToString() ?? string.Empty);
                stringBuilder.AppendLine();
            }
        }

        var result = stringBuilder.ToString();
        return result;
    }

    public static string DumpAsString3Dictionary2<TKey, TValue>(string operation, Dictionary<TKey, List<TValue>> grouped) where TKey : notnull
    {
        var stringBuilder = new StringBuilder();
        stringBuilder.AppendLine(operation);
        foreach (var item in grouped)
        {
            stringBuilder.AppendLine("1) " + item.Key);
            foreach (var value in item.Value)
                stringBuilder.AppendLine(value?.ToString() ?? string.Empty);
            stringBuilder.AppendLine();
        }

        var result = stringBuilder.ToString();
        return result;
    }

    // Returns FieldInfo objects so the caller can extract Name, Value, etc.
    public static List<FieldInfo> GetConsts(Type type, GetMemberArgs? args = null)
    {
        if (args == null)
            args = new GetMemberArgs();
        IList<FieldInfo> fieldInfoList;
        if (args.IsOnlyPublic)
            fieldInfoList = type.GetFields(BindingFlags.Public | BindingFlags.Static |
            BindingFlags.FlattenHierarchy).ToList();
        else
            fieldInfoList = type.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy).ToList();
        var constants = fieldInfoList.Where(fieldInfo => fieldInfo.IsLiteral && !fieldInfo.IsInitOnly).ToList();
        return constants;
    }

    public static List<MethodInfo> GetMethods(Type type)
    {
        var methods = type.GetMethods(BindingFlags.Public | BindingFlags.Static |
        BindingFlags.FlattenHierarchy).ToList();
        return methods;
    }

    public static object? SetValueOfProperty(string name, Type type, object instance, bool isIgnoringCase, object? value)
    {
        var properties = type.GetProperties();
        return SetValue(name, type, instance, properties, isIgnoringCase, value);
    }

    public static object? SetValue(string name, Type type, object instance, IList members, bool isIgnoringCase, object? value)
    {
        return GetOrSetValue(name, type, instance, members, isIgnoringCase, SetValue, value);
    }

    private static object? SetValue(object instance, MemberInfo[] members, object? value)
    {
        var member = members[0];
        if (member is PropertyInfo propertyInfo)
        {
            propertyInfo.SetValue(instance, value);
        }
        else if (member is FieldInfo fieldInfo)
        {
            fieldInfo.SetValue(instance, value);
        }

        return null;
    }

    public static bool ExistsAssemblyNotFullName(string value)
    {
        var entryAssembly = Assembly.GetEntryAssembly();
        if (entryAssembly == null)
            return false;
        var referencedAssemblies = AllReferencedAssemblies(entryAssembly);
        referencedAssemblies.Sort();
        if (referencedAssemblies.Contains(value))
            return true;
        return false;
    }

    private static readonly List<string> allReferencedAssemblies = new();

    public static List<string> AllReferencedAssemblies(Assembly entryAssembly, bool isUsingCache = true)
    {
        if (!isUsingCache)
        {
            allReferencedAssemblies.Clear();
        }
        else
        {
            if (allReferencedAssemblies.Count != 0)
                return allReferencedAssemblies;
        }

        var referencedAssemblies = entryAssembly.GetReferencedAssemblies();
        foreach (var item in referencedAssemblies)
            AllReferencedAssemblies(allReferencedAssemblies, item);
        return allReferencedAssemblies;
    }

    private static void AllReferencedAssemblies(List<string> collectedNames, AssemblyName assemblyName)
    {
        if (collectedNames.Contains(assemblyName.Name ?? string.Empty))
            return;
        collectedNames.Add(assemblyName.Name ?? string.Empty);
        Assembly? loadedAssembly = null;
        try
        {
            loadedAssembly = Assembly.Load(assemblyName);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return;
        }

        var referencedAssemblies = loadedAssembly.GetReferencedAssemblies();
        foreach (var item in referencedAssemblies)
            AllReferencedAssemblies(collectedNames, item);
    }
}
