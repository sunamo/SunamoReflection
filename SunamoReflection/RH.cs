// EN: Variable names have been checked and replaced with self-descriptive names
// CZ: Názvy proměnných byly zkontrolovány a nahrazeny samopopisnými názvy

namespace SunamoReflection;
using PropertyDescriptor = YamlDotNet.Serialization.PropertyDescriptor;

public class RH
{
    private static Type type = typeof(ThrowEx);

    /// <summary>
    ///     Usage: some methods just dump exceptions object
    /// </summary>
    /// <param name="empty"></param>
    /// <param name="output"></param>
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
        if (children == typeof(string) && !a1CanBeString) return false;
        if (children == null) ThrowEx.IsNull("children", children);
        while (true)
        {
            if (children == null) return false;
            if (children == parent) return true;
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
    /// <param name="obj"></param>
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
        if (result.Count() == 0) result = ass.Where(data => data.FullName == name);
        if (result.Count() == 0) result = ass.Where(data => data.FullName.Contains(name));
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
    /// <param name="carSAutoType"></param>
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
        foreach (var item in count) vr.Add(SH.NullToStringOrDefault(item.GetValue(null)));
        for (var i = 0; i < vr.Count; i++) vr[i] = vr[i].Trim();
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

        if (value == null) value = GetValueOfField(name, type, o, false);

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

        if (removeNull) f.RemoveAll(data => data == null);

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
    /// <param name="name"></param>
    /// <param name="o"></param>
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
                stringBuilder.AppendLine(DumpAsString(new DumpAsStringArgs
                {
                    data = DumpProvider.Reflection,
                    deli = "-",
                    o = item,
                    onlyValues = true,
                    onlyNames = a.onlyNames
                }));
                i++;
            }

            stringBuilder.AppendLine("***");
            return stringBuilder.ToString();
        }

        return string.Empty;
    }

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
    /// <param name="v"></param>
    /// <param name="tableRowPageNew"></param>
    /// <returns></returns>
    public static string DumpAsString3(object tableRowPageNew, DumpAsStringHeaderArgsReflection a = null)
    {
        if (a == null) a = DumpAsStringHeaderArgsReflection.Default;
        var dasa = new DumpAsStringArgs
        { o = tableRowPageNew, deli = "-", onlyValues = true, onlyNames = a.onlyNames };
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
    /// <param name="sb"></param>
    /// <param name="obj"></param>
    /// <param name="indent"></param>
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

    public static List<string> GetValuesOfProperty2(object obj, List<string> onlyNames, bool onlyValues/*,
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

    public static void GetValue(MemberInfo descriptor, bool isAllNeg, List<string> onlyNames, List<string> onlyNames2,
        object obj, List<string> values, bool onlyValues)
    {
        var add = true;
        var name = descriptor.Name;

        if (onlyNames.Contains("!" + name)) return;

        if (onlyNames2.Count > 0)
        {
            if (isAllNeg)
            {
                if (onlyNames2.Contains("!" + name)) add = false;
            }
            else
            {
                if (!onlyNames2.Contains(name)) add = false;
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


    public static object GetOrSetValue(string name, Type type, object instance, IList pis, bool ignoreCase,
        Func<object, MemberInfo[], object, object> getOrSet, object value)
    {
        if (ignoreCase)
        {
            name = name.ToLower();
            foreach (MemberInfo item in pis)
                if (item.Name.ToLower() == name)
                {
                    var property = type.GetMember(name);
                    if (property != null) return getOrSet(instance, property, value);
                    //return GetValue(instance, property);
                }
        }
        else
        {
            foreach (MemberInfo item in pis)
                if (item.Name == name)
                {
                    var property = type.GetMember(name);
                    if (property != null) return getOrSet(instance, property, value);
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
            values.Add($"{name}: {v}");
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

    /// <summary>
    ///     A1 have to be selected
    /// </summary>
    /// <param name="name"></param>
    /// <param name="o"></param>
    public static string DumpAsString(DumpAsStringArgs a)
    {
        // When I was serializing ISymbol, execution takes unlimited time here
        //return o.DumpToString(name);
        string dump = null;
        if (a.o.GetType() == typeof(string))
            dump = a.o.ToString();
        else
            switch (a.data)
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
                    dump = string.Join(a.onlyValues ? a.deli : Environment.NewLine,
                        GetValuesOfProperty2(a.o, a.onlyNames, a.onlyValues));
                    break;
                default:
                    ThrowEx.NotImplementedCase(a.data);
                    break;
            }

        if (string.IsNullOrWhiteSpace(a.name)) return dump;
        return a.name + Environment.NewLine + dump;
    }

    public static string DumpAsReflection(object o)
    {
        return DumpAsString(new DumpAsStringArgs { data = DumpProvider.Reflection, o = o });
    }

    private static string NameOfFieldsFromDump(object obj, DumpAsStringHeaderArgsReflection dumpAsStringHeaderArgs)
    {
        var properties = TypeDescriptor.GetProperties(obj);
        var sourceList = new List<string>();

        string name = null;

        foreach (PropertyDescriptor descriptor in properties)
        {
            name = descriptor.Name;
            if (dumpAsStringHeaderArgs.onlyNames.Contains("!" + name)) continue;
            sourceList.Add(name);
        }

        return string.Join("-", sourceList);
    }

    public static string DumpAsString3Dictionary3<temp, T2, U>(string operation,
        Dictionary<temp, Dictionary<T2, List<U>>> grouped)
    {
        var stringBuilder = new StringBuilder();
        stringBuilder.AppendLine(operation);

        foreach (var item in grouped)
        {
            stringBuilder.AppendLine("1) " + item.Key);

            foreach (var item3 in item.Value)
            {
                stringBuilder.AppendLine("2) " + item3.Key);

                foreach (var value in item3.Value) stringBuilder.AppendLine(value.ToString());
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

            foreach (var value in item.Value) stringBuilder.AppendLine(value.ToString());
            stringBuilder.AppendLine();
        }

        var vr = stringBuilder.ToString();
        return vr;
    }

    #region For easy copy from RHShared64.cs

    #region Get types of class

    /// <summary>
    ///     Return FieldInfo, so will be useful extract Name etc.
    /// </summary>
    /// <param name="type"></param>
    public static List<FieldInfo> GetConsts(Type type, GetMemberArgs a = null)
    {
        if (a == null) a = new GetMemberArgs();
        IList<FieldInfo> fieldInfos = null;
        if (a.onlyPublic)
            fieldInfos = type.GetFields(BindingFlags.Public | BindingFlags.Static |
                                        // return protected/public but not private
                                        BindingFlags.FlattenHierarchy).ToList();
        else
            ///fieldInfos = type.GetFields(BindingFlags.Static);//.Where(f => f.IsLiteral);
            fieldInfos = type.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.NonPublic |
                                        BindingFlags.FlattenHierarchy).ToList();


        var withType = fieldInfos.Where(fi => fi.IsLiteral && !fi.IsInitOnly).ToList();
        return withType;
    }

    #endregion

    #endregion

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

    #region Get types of class

    public static List<MethodInfo> GetMethods(Type temp)
    {
        var methods = temp.GetMethods(BindingFlags.Public | BindingFlags.Static |
                                   // return protected/public but not private
                                   BindingFlags.FlattenHierarchy).ToList();
        return methods;
    }

    #endregion

    #region from RHShared64.cs

    #region Get types of class

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

    #endregion

    #endregion

    #region For easy copy

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

        if (refAss.Contains(value)) return true;
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
            if (allReferencedAssemblies.Count != 0) return allReferencedAssemblies;
        }

        var refAss = execAssembly.GetReferencedAssemblies();
        foreach (var item in refAss) AllReferencedAssemblies(allReferencedAssemblies, item);
        //result.AddRange(refAss.Select(data => data.Name));
        return allReferencedAssemblies;
    }

    private static void AllReferencedAssemblies(List<string> n, AssemblyName execAssembly)
    {
        if (n.Contains(execAssembly.Name)) return;
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
        foreach (var item in refAss) AllReferencedAssemblies(n, item);
    }


    public static bool ExistsClass(string className)
    {
        var type2 = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                     from type in assembly.GetTypes()
                     where type.Name == className
                     select type).FirstOrDefault();

        return type2 != null;
    }

    #endregion

    #region Copy object

    public static object CopyObject(object input)
    {
        if (input != null)
        {
            var result = Activator.CreateInstance(input.GetType()); //, BindingFlags.Instance);
            foreach (var field in input.GetType().GetFields(
                         BindingFlags.GetField |
                         BindingFlags.GetProperty |
                         BindingFlags.NonPublic |
                         BindingFlags.Public |
                         BindingFlags.Static |
                         BindingFlags.Instance |
                         BindingFlags.Default |
                         BindingFlags.CreateInstance |
                         BindingFlags.DeclaredOnly
                     ))
                if (field.FieldType.GetInterface("IList", false) == null)
                {
                    field.SetValue(result, field.GetValue(input));
                }
                else
                {
                    var listObject = (IList)field.GetValue(result);
                    if (listObject != null)
                        foreach (var item in (IList)field.GetValue(input))
                            listObject.Add(CopyObject(item));
                }

            return result;
        }

        return null;
    }


    /// <summary>
    ///     Perform a deep Copy of the object.
    /// </summary>
    /// <typeparam name="T">The type of object being copied.</typeparam>
    /// <param name="source">The object instance to copy.</param>
    /// <returns>The copied object.</returns>
    public static temp Clone<T>(temp source)
    {
        if (!typeof(temp).IsSerializable) throw new Exception(XlfKeys.TheTypeMustBeSerializable + ". source");

        // Don't serialize a null object, simply return the default for that object
        if (ReferenceEquals(source, null)) return default;

        ThrowEx.NotImplementedMethod();
        return default;

        //IFormatter formatter = new BinaryFormatter();
        //Stream stream = new MemoryStream();


        //using (stream)
        //{
        //    formatter.Serialize(stream, source);
        //    stream.Seek(0, SeekOrigin.Begin);
        //    return (temp)formatter.Deserialize(stream);
        //}
    }

    public static List<string> GetValuesOfPropertyOrField(object o, params string[] onlyNames)
    {
        var values = new List<string>();
        values.AddRange(GetValuesOfProperty(o, onlyNames));
        values.AddRange(GetValuesOfField(o, onlyNames));

        return values;
    }


    /// <summary>
    ///     U složitějších ne mých .net objektů tu byla chyba, proto je zde GetValuesOfProperty2
    /// </summary>
    /// <param name="o"></param>
    /// <param name="onlyNames"></param>
    /// <returns></returns>
    public static List<string> GetValuesOfProperty(object o, params string[] onlyNames)
    {
        var props = o.GetType().GetProperties();
        var values = new List<string>(props.Length);

        foreach (var item in props)
        {
            if (onlyNames.Length > 0)
                if (!onlyNames.Contains(item.Name))
                    continue;

            var getMethod = item.GetGetMethod();
            if (getMethod != null)
            {
                var name = getMethod.Name;
                object value = null;

                if (getMethod.GetParameters().Length > 0)
                {
                    name += "[]";
                    value = item.GetValue(o
                    // nechápal jsem tak jsem to zakomentoval
                    //,new int[] { 1/* indexer value(s)*/}
                    );
                }
                else
                {
                    try
                    {
                        value = item.GetValue(o);
                    }
                    catch (Exception ex)
                    {
                        value = Exceptions.TextOfExceptions(ex);
                    }
                }

                name = name.Replace("get_", string.Empty);
                AddValue(values, name, value.ToString(), false);
            }
        }

        return values;
    }


    /// <summary>
    ///     Copy values of all readable properties
    /// </summary>
    /// <param name="source"></param>
    /// <param name="target"></param>
    public void CopyProperties(object source, object target)
    {
        var typeB = target.GetType();
        foreach (var property in source.GetType().GetProperties())
        {
            if (!property.CanRead || property.GetIndexParameters().Length > 0)
                continue;

            var other = typeB.GetProperty(property.Name);
            if (other != null && other.CanWrite)
                other.SetValue(target, property.GetValue(source, null), null);
        }
    }

    #endregion

    #region FullName

    public static string FullNameOfMethod(MethodInfo mi)
    {
        return mi.DeclaringType.FullName + mi.Name;
    }

    public static string FullNameOfClassEndsDot(Type value)
    {
        return value.FullName + ".";
    }

    public static string FullNameOfExecutedCode(MethodBase method)
    {
        var methodName = method.Name;
        var type = method.ReflectedType.Name;
        return SH.ConcatIfBeforeHasValue(type, ".", methodName, ":");
    }

    #endregion

    #region Whole assembly

    public static IList<Type> GetTypesInNamespace(Assembly assembly, string nameSpace)
    {
        var types = assembly.GetTypes();
        return types.Where(temp => string.Equals(temp.Namespace, nameSpace, StringComparison.Ordinal)).ToList();
    }

    /// <summary>
    ///     Pokud mám chybu Could not load file or assembly System.Reflection.Metadata, Version=1.4.5.0
    ///     program volám z AllProjectsSearchConsole tuto sunamo assembly,
    ///     musím přidat System.Reflection.Metadata do obou. Ověřeno.
    ///     Better than load assembly directly from running is use Assembly.LoadFrom
    /// </summary>
    /// <param name="assembly"></param>
    /// <param name="contains"></param>
    /// <returns></returns>
    public static IList<Type> GetTypesInAssembly(Assembly assembly, string contains)
    {
        var types = assembly.GetTypes();
        return types.Where(temp => temp.Name.Contains(contains)).ToList();
    }

    #endregion
}