namespace SunamoReflection;

using PropertyDescriptor = YamlDotNet.Serialization.PropertyDescriptor;

public partial class RH
{
    public static bool ExistsClass(string className)
    {
        var type2 = (
            from assembly in AppDomain.CurrentDomain.GetAssemblies()from type in assembly.GetTypes()
            where type.Name == className
            select type).FirstOrDefault();
        return type2 != null;
    }

    public static object CopyObject(object input)
    {
        if (input != null)
        {
            var result = Activator.CreateInstance(input.GetType()); //, BindingFlags.Instance);
            foreach (var field in input.GetType().GetFields(BindingFlags.GetField | BindingFlags.GetProperty | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance | BindingFlags.Default | BindingFlags.CreateInstance | BindingFlags.DeclaredOnly))
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
    /// <typeparam name = "T">The type of object being copied.</typeparam>
    /// <param name = "source">The object instance to copy.</param>
    /// <returns>The copied object.</returns>
    public static T Clone<T>(T source)
    {
        if (!typeof(T).IsSerializable)
            throw new Exception(XlfKeys.TheTypeMustBeSerializable + ". source");
        // Don't serialize a null object, simply return the default for that object
        if (ReferenceEquals(source, null))
            return default;
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
    /// <param name = "o"></param>
    /// <param name = "onlyNames"></param>
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
                    value = item.GetValue(o// nechápal jsem tak jsem to zakomentoval
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
    /// <param name = "source"></param>
    /// <param name = "target"></param>
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
    /// <param name = "assembly"></param>
    /// <param name = "contains"></param>
    /// <returns></returns>
    public static IList<Type> GetTypesInAssembly(Assembly assembly, string contains)
    {
        var types = assembly.GetTypes();
        return types.Where(temp => temp.Name.Contains(contains)).ToList();
    }
}