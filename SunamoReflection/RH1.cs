// EN: Variable names have been checked and replaced with self-descriptive names
// CZ: Názvy proměnných byly zkontrolovány a nahrazeny samopopisnými názvy
namespace SunamoReflection;
using PropertyDescriptor = YamlDotNet.Serialization.PropertyDescriptor;

public partial class RH
{
    /////// <summary>
    /////// Delimited by NL
    /////// </summary>
    /////// <param name="v"></param>
    /////// <param name="device"></param>
    /////// <returns></returns>
    //public static string DumpAsString2(string value, object device)
    //{
    //    return SunamoExceptions.RH.DumpAsString(value, device);
    //    //DumpAsString(new DumpAsStringArgs { name = value, o = device, data = DumpProvider.Yaml });
    //}
    /// <summary>
    ///     swda Delimiter
    ///     Mainly for fast comparing objects
    ///     Zde můžu zadat jen onlyNames kvůli DumpAsStringHeaderArgs
    ///     Pokud chci více customizovat výstup, musím užít DumpAsString - DumpAsStringArgs
    /// </summary>
    /// <param name = "v"></param>
    /// <param name = "tableRowPageNew"></param>
    /// <returns></returns>
    public static string DumpAsString3(object tableRowPageNew, DumpAsStringHeaderArgsReflection a = null)
    {
        if (a == null)
            a = DumpAsStringHeaderArgsReflection.Default;
        var dasa = new DumpAsStringArgs
        {
            o = tableRowPageNew,
            deli = "-",
            onlyValues = true,
            onlyNames = a.onlyNames
        };
        return DumpAsString(dasa);
    }

    public static List<string> GetValuesOfField(object o, params string[] onlyNames)
    {
        return GetValuesOfField(o, onlyNames);
    }

    public static List<string> GetValuesOfField(object o, IList<string> onlyNames, bool onlyValues)
    {
        var temp = o.GetType();
        var props = temp.GetFields();
        var values = new List<string>(props.Length);
        foreach (var item in props)
        {
            if (onlyNames.Count > 0)
                if (!onlyNames.Contains(item.Name))
                    continue;
            //values.Add(item.Name + ":" + SHGetString.ListToString(GetValueOfField(item.Name, temp, o, false)));
            AddValue(values, item.Name, GetValueOfField(item.Name, temp, o, false).ToString(), onlyValues);
        }

        return values;
    }

    /// <summary>
    /// Tato metoda se mi hodila value usysu
    /// Tam mi dělala StackOverflowException
    /// </summary>
    /// <param name = "sb"></param>
    /// <param name = "obj"></param>
    /// <param name = "indent"></param>
    public static void PrintPublicPropertiesRecursively(StringBuilder stringBuilder, Type obj, string indent = "  ")
    {
        if (obj == null)
        {
            return;
        }

        stringBuilder.AppendLine($"{indent}Object Type: {obj.Name}");
        PropertyInfo[] properties = obj.GetProperties(BindingFlags.Public | BindingFlags.Instance);
        foreach (PropertyInfo property in properties)
        {
            string typeName = property.PropertyType.Name;
            try
            {
                //object value = property.GetValue(obj);
                //if (value == null)
                //{
                //    stringBuilder.AppendLine($"{indent}- {property.Name} ({typeName})");
                //}
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
            catch (Exception ex)
            {
                stringBuilder.AppendLine($"{indent}- {property.Name} ({typeName}): Error getting value - {ex.Message}");
            }
        }
    }

    public static List<string> GetValuesOfProperty2(object obj, List<string> onlyNames, bool onlyValues /*,
        bool takeVariablesIfThereIsNoProps = true*/)
    {
        var onlyNames2 = onlyNames.ToList();
        var values = new List<string>();
        string name = null;
        var props = GetProps(obj); //TypeDescriptor.GetProperties(obj);
        var isAllNeg = true;
        foreach (var item in onlyNames)
            if (!item.StartsWith("!"))
                isAllNeg = false;
        if (props.Count == 0)
        {
            var data = GetFields(obj);
            foreach (var descriptor in data)
                GetValue(descriptor, isAllNeg, onlyNames, onlyNames2, obj, values, onlyValues);
        }
        else
        {
            foreach (var descriptor in props)
                GetValue(descriptor, isAllNeg, onlyNames, onlyNames2, obj, values, onlyValues);
        }

        return values;
    }

    public static void GetValue(MemberInfo descriptor, bool isAllNeg, List<string> onlyNames, List<string> onlyNames2, object obj, List<string> values, bool onlyValues)
    {
        var add = true;
        var name = descriptor.Name;
        if (onlyNames.Contains("!" + name))
            return;
        if (onlyNames2.Count > 0)
        {
            if (isAllNeg)
            {
                if (onlyNames2.Contains("!" + name))
                    add = false;
            }
            else
            {
                if (!onlyNames2.Contains(name))
                    add = false;
            }
        }

        if (add)
        {
            var value = GetValue(obj, descriptor);
            AddValue(values, name, value.ToString(), onlyValues);
        }
    }

    private static object GetValue(object instance, MemberInfo[] property, object value)
    {
        return GetValue(instance, property);
    }

    private static object GetValue(object instance, params MemberInfo[] property)
    {
        var val = property[0];
        if (val is PropertyInfo)
        {
            var pi = (PropertyInfo)val;
            return pi.GetValue(instance);
        }

        if (val is FieldInfo)
        {
            var pi = (FieldInfo)val;
            return pi.GetValue(instance);
        }

        return null;
    }

    public static object GetValue(string name, Type type, object instance, IList pis, bool ignoreCase, object value)
    {
        return GetOrSetValue(name, type, instance, pis, ignoreCase, GetValue, value);
    }

    public static object GetOrSetValue(string name, Type type, object instance, IList pis, bool ignoreCase, Func<object, MemberInfo[], object, object> getOrSet, object value)
    {
        if (ignoreCase)
        {
            name = name.ToLower();
            foreach (MemberInfo item in pis)
                if (item.Name.ToLower() == name)
                {
                    var property = type.GetMember(name);
                    if (property != null)
                        return getOrSet(instance, property, value);
                //return GetValue(instance, property);
                }
        }
        else
        {
            foreach (MemberInfo item in pis)
                if (item.Name == name)
                {
                    var property = type.GetMember(name);
                    if (property != null)
                        return getOrSet(instance, property, value);
                //return GetValue(instance, property);
                }
        }

        return null;
    }

    private static void AddValue(List<string> values, string name, string value, bool onlyValue)
    {
        //var value = SHGetString.ListToString(value, null);
        if (onlyValue)
            values.Add(value);
        else
            values.Add($"{name}: {value}");
    }

    ///// <summary>
    ///// Check whether A1 is or is derived from A2
    ///// </summary>
    ///// <param name="type1"></param>
    ///// <param name="type2"></param>
    //public static bool IsOrIsDeriveFromBaseClass(Type children, Type parent, bool a1CanBeString = true)
    //{
    //    return se.RH.IsOrIsDeriveFromBaseClass(children, parent, a1CanBeString);
    //}
    public static string DumpAsObjectDumperNet(object o)
    {
        //return ObjectDumperNetHelper.Dump(o);
        return ObjectDumper.Dump(o);
    }
}