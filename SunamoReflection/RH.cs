// EN: Variable names have been checked and replaced with self-descriptive names
// CZ: Názvy proměnných byly zkontrolovány a nahrazeny samopopisnými názvy
namespace SunamoReflection;
using PropertyDescriptor = YamlDotNet.Serialization.PropertyDescriptor;

public partial class RH
{
    private static Type type = typeof(ThrowEx);
    /// <summary>
    ///     Usage: some methods just dump exceptions object
    /// </summary>
    /// <param name = "empty"></param>
    /// <param name = "output"></param>
    /// <returns></returns>
    public static string DumpAsXml(object output)
    {
        string objectAsXmlString;
        XmlSerializer xs = new(output.GetType());
        using (StringWriter sw = new())
        {
            try
            {
                xs.Serialize(sw, output);
                objectAsXmlString = sw.ToString();
            }
            catch (Exception ex)
            {
                objectAsXmlString = ex.ToString();
            }
        }

        return objectAsXmlString;
    }

    public static bool IsOrIsDeriveFromBaseClass(Type children, Type parent, bool a1CanBeString = true)
    {
        if (children == typeof(string) && !a1CanBeString)
            return false;
        if (children == null)
            ThrowEx.IsNull("children", children);
        while (true)
        {
            if (children == null)
                return false;
            if (children == parent)
                return true;
            foreach (var inter in children.GetInterfaces())
                if (inter == parent)
                    return true;
            children = children.BaseType;
        }
    }

    /// <summary>
    /// Získá názvy všech properties ve třídě bez ohledu na access modifier.
    /// Pouze deklarované value třídě, bez jakýchkoliv zděděných. 
    /// </summary>
    /// <param name = "obj"></param>
    /// <returns></returns>
    public static List<string> GetPropertyNames(Type type)
    {
        PropertyInfo[] properties = type.GetProperties(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
        return properties.Select(data => data.Name).ToList();
    }

    public static string FullPathCodeEntity(Type temp)
    {
        return temp.Namespace + "." + temp.Name;
    }

    public static Assembly AssemblyWithName(string name)
    {
        var ass = AppDomain.CurrentDomain.GetAssemblies();
        var result = ass.Where(data => data.GetName().Name == name);
        if (result.Count() == 0)
            result = ass.Where(data => data.FullName == name);
        if (result.Count() == 0)
            result = ass.Where(data => data.FullName.Contains(name));
        return result.FirstOrDefault();
    }

    private static List<PropertyInfo> GetProps(object carSAuto)
    {
        var carSAutoType = GetType(carSAuto);
        var result = carSAutoType.GetProperties().ToList();
        return result;
    }

    private static Type GetType(object carSAuto)
    {
        Type carSAutoType = null;
        var t1 = carSAuto.GetType();
        if (IsType(t1))
            carSAutoType = carSAuto as Type;
        else
            carSAutoType = carSAuto.GetType();
        return carSAutoType;
    }

    /// <summary>
    ///     A1 can be Type of instance
    ///     All fields must be public
    /// </summary>
    /// <param name = "carSAutoType"></param>
    public static List<FieldInfo> GetFields(object carSAuto)
    {
        Type carSAutoType = null;
        var t1 = carSAuto.GetType();
        if (IsType(t1))
            carSAutoType = carSAuto as Type;
        else
            carSAutoType = carSAuto.GetType();
        var result = carSAutoType.GetFields().ToList();
        return result;
    }

    private static bool IsType(Type t1)
    {
        var t2 = typeof(Type);
        return t1.FullName == "System.RuntimeType" || t1 == t2;
    }

    public static List<string> GetValuesOfConsts(Type type)
    {
        var count = GetConsts(type);
        var vr = new List<string>();
        foreach (var item in count)
            vr.Add(SH.NullToStringOrDefault(item.GetValue(null)));
        for (var i = 0; i < vr.Count; i++)
            vr[i] = vr[i].Trim();
        return vr;
    }

    public static Dictionary<string, string> GetValuesOfConsts(Type temp, params string[] onlyNames)
    {
        var props = GetConsts(temp);
        var values = new Dictionary<string, string>(props.Count);
        foreach (var item in props)
        {
            if (onlyNames.Length > 0)
                if (!onlyNames.Contains(item.Name))
                    continue;
            var o = GetValueOfField(item.Name, temp, null, false);
            values.Add(item.Name, o.ToString());
        }

        return values;
    }

    public static object GetValueOfPropertyOrField(object o, string name)
    {
        var type = o.GetType();
        var value = GetValueOfProperty(name, type, o, false);
        if (value == null)
            value = GetValueOfField(name, type, o, false);
        return value;
    }

    public static object GetValueOfField(string name, Type type, object instance, bool ignoreCase)
    {
        var pis = type.GetFields();
        return GetValue(name, type, instance, pis, ignoreCase, null);
    }

    public static object GetValueOfProperty(string name, Type type, object instance, bool ignoreCase)
    {
        var pis = type.GetProperties();
        return GetValue(name, type, instance, pis, ignoreCase, null);
    }

    public static string DumpListAsString(DumpAsStringArgs a, bool removeNull = false)
    {
        var stringBuilder = new StringBuilder();
        var f = (List<object>)a.o;
        if (removeNull)
            f.RemoveAll(data => data == null);
        if (f.Count > 0)
        {
            stringBuilder.AppendLine(NameOfFieldsFromDump(f.First(), a));
            foreach (var item in f)
            {
                a.o = item;
                stringBuilder.AppendLine(DumpAsString(a));
            }
        }

        return stringBuilder.ToString();
    }

    /// <summary>
    ///     DumpAsString2 se mi ztratilo
    ///     nemůžu ho najít value žádném repu
    /// </summary>
    /// <param name = "name"></param>
    /// <param name = "o"></param>
    /// <returns></returns>
     //public static string DumpListAsString(string name, IList o)
    //{
    //    var stringBuilder = new StringBuilder();
    //    var i = 0;
    //    foreach (var item in o) ThrowEx.NotImplementedMethod();
    //    //stringBuilder.AppendLine(DumpAsString2(name + "#" + i, item));
    //    //i++;
    //    return stringBuilder.ToString();
    //}
    public static string DumpListAsStringOneLine(string operation, IList o, DumpAsStringHeaderArgsReflection a)
    {
        if (o.Count > 0)
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("***");
            stringBuilder.AppendLine(operation + " " + "(" + o.Count + ")" + ":");
            stringBuilder.AppendLine(NameOfFieldsFromDump(o.Count != 0 ? null : o[0], a));
            var i = 0;
            foreach (var item in o)
            {
                stringBuilder.AppendLine(DumpAsString(new DumpAsStringArgs { d = DumpProvider.Reflection, deli = "-", o = item, onlyValues = true, onlyNames = a.onlyNames }));
                i++;
            }

            stringBuilder.AppendLine("***");
            return stringBuilder.ToString();
        }

        return string.Empty;
    }
}