namespace SunamoReflection;

using PropertyDescriptor = YamlDotNet.Serialization.PropertyDescriptor;

public partial class RH
{
    public static bool ExistsClass(string className)
    {
        var foundType = (
            from assembly in AppDomain.CurrentDomain.GetAssemblies()from type in assembly.GetTypes()
            where type.Name == className
            select type).FirstOrDefault();
        return foundType != null;
    }

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

    public static List<string> GetValuesOfPropertyOrField(object instance, params string[] onlyNames)
    {
        var values = new List<string>();
        values.AddRange(GetValuesOfProperty(instance, onlyNames));
        values.AddRange(GetValuesOfField(instance, onlyNames));
        return values;
    }

    // For more complex .NET objects, use GetValuesOfProperty2 instead.
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

    public static string FullNameOfMethod(MethodInfo methodInfo)
    {
        return (methodInfo.DeclaringType?.FullName ?? string.Empty) + methodInfo.Name;
    }

    public static string FullNameOfClassEndsDot(Type type)
    {
        return type.FullName + ".";
    }

    public static string FullNameOfExecutedCode(MethodBase method)
    {
        var methodName = method.Name;
        var typeName = method.ReflectedType?.Name ?? string.Empty;
        return SH.ConcatIfBeforeHasValue(typeName, ".", methodName, ":");
    }

    public static IList<Type> GetTypesInNamespace(Assembly assembly, string nameSpace)
    {
        var types = assembly.GetTypes();
        return types.Where(type => string.Equals(type.Namespace, nameSpace, StringComparison.Ordinal)).ToList();
    }

    // Better than loading assemblies directly from the running process is using Assembly.LoadFrom.
    // If you encounter "Could not load file or assembly System.Reflection.Metadata",
    // add System.Reflection.Metadata to both the calling and target projects.
    public static IList<Type> GetTypesInAssembly(Assembly assembly, string contains)
    {
        var types = assembly.GetTypes();
        return types.Where(type => type.Name.Contains(contains)).ToList();
    }
}
