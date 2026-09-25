namespace SunamoReflection;

using PropertyDescriptor = YamlDotNet.Serialization.PropertyDescriptor;

public partial class RH
{
    /// <summary>
    /// Checks whether a class with the specified name exists in any loaded assembly.
    /// </summary>
    /// <param name="className">The class name to search for.</param>
    /// <returns>True if a class with the specified name exists.</returns>
    public static bool ExistsClass(string className)
    {
        var foundType = (
            from assembly in AppDomain.CurrentDomain.GetAssemblies()from type in assembly.GetTypes()
            where type.Name == className
            select type).FirstOrDefault();
        return foundType != null;
    }

    /// <summary>
    /// Creates a shallow copy of an object including list fields via reflection.
    /// </summary>
    /// <param name="input">The object to copy.</param>
    /// <returns>A copy of the object, or null if input is null.</returns>
    public static object? CopyObject(object? input)
    {
        if (input != null)
        {
            var result = Activator.CreateInstance(input.GetType());
            foreach (var field in input.GetType().GetFields(BindingFlags.GetField | BindingFlags.GetProperty | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance | BindingFlags.Default | BindingFlags.CreateInstance | BindingFlags.DeclaredOnly))
                if (field.FieldType.GetInterface("IList", false) == null)
                {
                    field.SetValue(result, field.GetValue(input));
                }
                else
                {
                    var listObject = (IList?)field.GetValue(result);
                    if (listObject != null)
                        foreach (var item in (IList)field.GetValue(input)!)
                            listObject.Add(CopyObject(item));
                }

            return result;
        }

        return null;
    }

    /// <summary>
    /// Performs a deep copy of an object. Currently not implemented for non-serializable types.
    /// </summary>
    /// <typeparam name="T">The type of object being copied.</typeparam>
    /// <param name="source">The object instance to copy.</param>
    /// <returns>The copied object.</returns>
#pragma warning disable SYSLIB0050
    public static T? Clone<T>(T source)
    {
        if (!typeof(T).IsSerializable)
            throw new Exception(XlfKeys.TheTypeMustBeSerializable + ". source");
        if (ReferenceEquals(source, null))
            return default;
        ThrowEx.NotImplementedMethod();
        return default;
    }
#pragma warning restore SYSLIB0050

    /// <summary>
    /// Gets string values of all properties and fields from an object, optionally filtered by name.
    /// </summary>
    /// <param name="instance">The object to inspect.</param>
    /// <param name="onlyNames">Optional filter for specific property/field names.</param>
    /// <returns>Combined list of property and field values as strings.</returns>
    public static List<string> GetValuesOfPropertyOrField(object instance, params string[] onlyNames)
    {
        var values = new List<string>();
        values.AddRange(GetValuesOfProperty(instance, onlyNames));
        values.AddRange(GetValuesOfField(instance, onlyNames));
        return values;
    }

    /// <summary>
    /// Gets string values of all properties from an object using reflection.
    /// For more complex .NET objects, use GetValuesOfProperty2 instead.
    /// </summary>
    /// <param name="instance">The object to inspect.</param>
    /// <param name="onlyNames">Optional filter for specific property names.</param>
    /// <returns>List of property values as strings.</returns>
    public static List<string> GetValuesOfProperty(object instance, params string[] onlyNames)
    {
        var properties = instance.GetType().GetProperties();
        var values = new List<string>(properties.Length);
        foreach (var item in properties)
        {
            if (onlyNames.Length > 0)
                if (!onlyNames.Contains(item.Name))
                    continue;
            var getMethod = item.GetGetMethod();
            if (getMethod != null)
            {
                var name = getMethod.Name;
                object? value = null;
                if (getMethod.GetParameters().Length > 0)
                {
                    name += "[]";
                    value = item.GetValue(instance);
                }
                else
                {
                    try
                    {
                        value = item.GetValue(instance);
                    }
                    catch (Exception exception)
                    {
                        value = Exceptions.TextOfExceptions(exception);
                    }
                }

                name = name.Replace("get_", string.Empty);
                AddValue(values, name, value?.ToString() ?? string.Empty, false);
            }
        }

        return values;
    }

    /// <summary>
    /// Copies values of all readable properties from source to target object.
    /// </summary>
    /// <param name="source">The source object to copy from.</param>
    /// <param name="target">The target object to copy to.</param>
    public void CopyProperties(object source, object target)
    {
        var targetType = target.GetType();
        foreach (var property in source.GetType().GetProperties())
        {
            if (!property.CanRead || property.GetIndexParameters().Length > 0)
                continue;
            var targetProperty = targetType.GetProperty(property.Name);
            if (targetProperty != null && targetProperty.CanWrite)
                targetProperty.SetValue(target, property.GetValue(source, null), null);
        }
    }

    /// <summary>
    /// Gets the full name of a method (DeclaringType.FullName + MethodName).
    /// </summary>
    /// <param name="methodInfo">The method info to get the name from.</param>
    /// <returns>Full name string.</returns>
    public static string FullNameOfMethod(MethodInfo methodInfo)
    {
        return (methodInfo.DeclaringType?.FullName ?? string.Empty) + methodInfo.Name;
    }

    /// <summary>
    /// Gets the full name of a class ending with a dot.
    /// </summary>
    /// <param name="type">The type to get the full name for.</param>
    /// <returns>Full name followed by a dot.</returns>
    public static string FullNameOfClassEndsDot(Type type)
    {
        return type.FullName + ".";
    }

    /// <summary>
    /// Gets the full name of the currently executing code location from a MethodBase.
    /// </summary>
    /// <param name="method">The method base to extract info from.</param>
    /// <returns>Formatted string "TypeName.MethodName:".</returns>
    public static string FullNameOfExecutedCode(MethodBase method)
    {
        var methodName = method.Name;
        var typeName = method.ReflectedType?.Name ?? string.Empty;
        return SH.ConcatIfBeforeHasValue(typeName, ".", methodName, ":");
    }

    /// <summary>
    /// Gets all types defined in the specified namespace within the given assembly.
    /// </summary>
    /// <param name="assembly">The assembly to search.</param>
    /// <param name="nameSpace">The namespace to filter by.</param>
    /// <returns>List of types in the specified namespace.</returns>
    public static IList<Type> GetTypesInNamespace(Assembly assembly, string nameSpace)
    {
        var types = assembly.GetTypes();
        return types.Where(type => string.Equals(type.Namespace, nameSpace, StringComparison.Ordinal)).ToList();
    }

    /// <summary>
    /// Gets all types in the assembly whose names contain the specified string.
    /// Better than loading assemblies directly from the running process is using Assembly.LoadFrom.
    /// If you encounter "Could not load file or assembly System.Reflection.Metadata",
    /// add System.Reflection.Metadata to both the calling and target projects.
    /// </summary>
    /// <param name="assembly">The assembly to search.</param>
    /// <param name="contains">The substring to search for in type names.</param>
    /// <returns>List of matching types.</returns>
    public static IList<Type> GetTypesInAssembly(Assembly assembly, string contains)
    {
        var types = assembly.GetTypes();
        return types.Where(type => type.Name.Contains(contains)).ToList();
    }
}
