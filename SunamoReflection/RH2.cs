// EN: Variable names have been checked and replaced with self-descriptive names
// CZ: Názvy proměnných byly zkontrolovány a nahrazeny samopopisnými názvy
namespace SunamoReflection;
using PropertyDescriptor = YamlDotNet.Serialization.PropertyDescriptor;

public partial class RH
{
    /// <summary>
    ///     A1 have to be selected
    /// </summary>
    /// <param name = "name"></param>
    /// <param name = "o"></param>
    public static string DumpAsString(DumpAsStringArgs a)
    {
        // When I was serializing ISymbol, execution takes unlimited time here
        //return o.DumpToString(name);
        string dump = null;
        if (a.o.GetType() == typeof(string))
            dump = a.o.ToString();
        else
            switch (a.d)
            {
                case DumpProvider.Xml:
                    dump = DumpAsXml(a.o);
                    break;
                case DumpProvider.ObjectDumperNet:
                    return DumpAsObjectDumperNet(a.o);
                    break;
                case DumpProvider.Yaml:
                case DumpProvider.Json:
                case DumpProvider.Reflection:
                    dump = string.Join(a.onlyValues ? a.deli : Environment.NewLine, GetValuesOfProperty2(a.o, a.onlyNames, a.onlyValues));
                    break;
                default:
                    ThrowEx.NotImplementedCase(a.d);
                    break;
            }

        if (string.IsNullOrWhiteSpace(a.name))
            return dump;
        return a.name + Environment.NewLine + dump;
    }

    public static string DumpAsReflection(object o)
    {
        return DumpAsString(new DumpAsStringArgs { d = DumpProvider.Reflection, o = o });
    }

    private static string NameOfFieldsFromDump(object obj, DumpAsStringHeaderArgsReflection dumpAsStringHeaderArgs)
    {
        var properties = TypeDescriptor.GetProperties(obj);
        var sourceList = new List<string>();
        string name = null;
        foreach (PropertyDescriptor descriptor in properties)
        {
            name = descriptor.Name;
            if (dumpAsStringHeaderArgs.onlyNames.Contains("!" + name))
                continue;
            sourceList.Add(name);
        }

        return string.Join("-", sourceList);
    }

    public static string DumpAsString3Dictionary3<temp, T2, U>(string operation, Dictionary<temp, Dictionary<T2, List<U>>> grouped)
    {
        var stringBuilder = new StringBuilder();
        stringBuilder.AppendLine(operation);
        foreach (var item in grouped)
        {
            stringBuilder.AppendLine("1) " + item.Key);
            foreach (var item3 in item.Value)
            {
                stringBuilder.AppendLine("2) " + item3.Key);
                foreach (var value in item3.Value)
                    stringBuilder.AppendLine(value.ToString());
                stringBuilder.AppendLine();
            }
        }

        var vr = stringBuilder.ToString();
        return vr;
    }

    public static string DumpAsString3Dictionary2<temp, T1>(string operation, Dictionary<temp, List<T1>> grouped)
    {
        var stringBuilder = new StringBuilder();
        stringBuilder.AppendLine(operation);
        foreach (var item in grouped)
        {
            stringBuilder.AppendLine("1) " + item.Key);
            foreach (var value in item.Value)
                stringBuilder.AppendLine(value.ToString());
            stringBuilder.AppendLine();
        }

        var vr = stringBuilder.ToString();
        return vr;
    }

    /// <summary>
    ///     Return FieldInfo, so will be useful extract Name etc.
    /// </summary>
    /// <param name = "type"></param>
    public static List<FieldInfo> GetConsts(Type type, GetMemberArgs a = null)
    {
        if (a == null)
            a = new GetMemberArgs();
        IList<FieldInfo> fieldInfos = null;
        if (a.onlyPublic)
            fieldInfos = type.GetFields(BindingFlags.Public | BindingFlags.Static | // return protected/public but not private
            BindingFlags.FlattenHierarchy).ToList();
        else
            ///fieldInfos = type.GetFields(BindingFlags.Static);//.Where(f => f.IsLiteral);
            fieldInfos = type.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy).ToList();
        var withType = fieldInfos.Where(fi => fi.IsLiteral && !fi.IsInitOnly).ToList();
        return withType;
    }

    //public static void SetPropertyToInnerClass<T>(temp propertyGroup, XName name, string value)
    //{
    //    /*
    //    RH.GetValuesOfProperty
    //    RH.GetValuesOfProperty2
    //    RH.GetValuesOfPropertyOrField
    //    */
    //    var count = propertyGroup;
    //    //foreach
    //}
    public static List<MethodInfo> GetMethods(Type temp)
    {
        var methods = temp.GetMethods(BindingFlags.Public | BindingFlags.Static | // return protected/public but not private
        BindingFlags.FlattenHierarchy).ToList();
        return methods;
    }

    ///// <summary>
    ///// Return FieldInfo, so will be useful extract Name etc.
    ///// </summary>
    ///// <param name="type"></param>
    //public static List<FieldInfo> GetConsts(Type type, GetMemberArgs a = null)
    //{
    //    if (a == null)
    //    {
    //        a = new GetMemberArgs();
    //    }
    //    IEnumerable<FieldInfo> fieldInfos = null;
    //    if (a.onlyPublic)
    //    {
    //        fieldInfos = type.GetFields(BindingFlags.Public | BindingFlags.Static |
    //        // return protected/public but not private
    //        BindingFlags.FlattenHierarchy).ToList();
    //    }
    //    else
    //    {
    //        ///fieldInfos = type.GetFields(BindingFlags.Static);//.Where(f => f.IsLiteral);
    //        fieldInfos = type.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.NonPublic |
    //          BindingFlags.FlattenHierarchy).ToList();
    //    }
    //    var withType = fieldInfos.Where(fi => fi.IsLiteral && !fi.IsInitOnly).ToList();
    //    return withType;
    //}
    public static object SetValueOfProperty(string name, Type type, object instance, bool ignoreCase, object value)
    {
        var pis = type.GetProperties();
        return SetValue(name, type, instance, pis, ignoreCase, value);
    }

    public static object SetValue(string name, Type type, object instance, IList pis, bool ignoreCase, object value)
    {
        return GetOrSetValue(name, type, instance, pis, ignoreCase, SetValue, value);
    }

    private static object SetValue(object instance, MemberInfo[] property, object value)
    {
        var val = property[0];
        if (val is PropertyInfo)
        {
            var pi = (PropertyInfo)val;
            pi.SetValue(instance, value);
        }
        else if (val is FieldInfo)
        {
            var pi = (FieldInfo)val;
            pi.SetValue(instance, value);
        }

        return null;
    }

    public static bool ExistsAssemblyNotFullName(string value)
    {
        var execAssembly = Assembly.GetEntryAssembly();
        var refAss = AllReferencedAssemblies(execAssembly);
#if DEBUG
        //var names = refAss.Select(data => data.Name).ToList();
        refAss.Sort();
        if (value == "Aps.Xlf")
        {
        }
#endif
        if (refAss.Contains(value))
            return true;
        return false;
    }

    private static readonly List<string> allReferencedAssemblies = new();
    public static List<string> AllReferencedAssemblies(Assembly execAssembly, bool useCache = true)
    {
        if (!useCache)
        {
            allReferencedAssemblies.Clear();
        }
        else
        {
            if (allReferencedAssemblies.Count != 0)
                return allReferencedAssemblies;
        }

        var refAss = execAssembly.GetReferencedAssemblies();
        foreach (var item in refAss)
            AllReferencedAssemblies(allReferencedAssemblies, item);
        //result.AddRange(refAss.Select(data => data.Name));
        return allReferencedAssemblies;
    }

    private static void AllReferencedAssemblies(List<string> n, AssemblyName execAssembly)
    {
        if (n.Contains(execAssembly.Name))
            return;
        n.Add(execAssembly.Name);
        Assembly ass = null;
        try
        {
            ass = Assembly.Load(execAssembly);
        }
        catch (Exception ex)
        {
            return;
        }

        var refAss = ass.GetReferencedAssemblies();
        foreach (var item in refAss)
            AllReferencedAssemblies(n, item);
    }
}