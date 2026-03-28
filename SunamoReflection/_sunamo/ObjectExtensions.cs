namespace SunamoReflection._sunamo;

/// <summary>
/// Provides deep copy functionality for objects using reflection.
/// </summary>
internal static class ObjectExtensions
{
    private static readonly MethodInfo? CloneMethod =
        typeof(object).GetMethod("MemberwiseClone", BindingFlags.NonPublic | BindingFlags.Instance);

    /// <summary>
    /// Determines whether the specified type is a primitive type (including string).
    /// </summary>
    /// <param name="type">The type to check.</param>
    /// <returns>True if the type is primitive or string.</returns>
    internal static bool IsPrimitive(this Type type)
    {
        if (type == typeof(string)) return true;
        return type.IsValueType & type.IsPrimitive;
    }

    /// <summary>
    /// Creates a deep copy of the specified object, tracking already visited objects to handle circular references.
    /// </summary>
    /// <param name="originalObject">The object to copy.</param>
    /// <param name="visited">Dictionary tracking already copied objects.</param>
    /// <returns>A deep copy of the object.</returns>
    internal static object? InternalCopy(object? originalObject, IDictionary<object, object> visited)
    {
        if (originalObject == null) return null;
        var typeToReflect = originalObject.GetType();
        if (typeToReflect.IsPrimitive()) return originalObject;
        if (visited.ContainsKey(originalObject)) return visited[originalObject];
        if (typeof(Delegate).IsAssignableFrom(typeToReflect)) return null;
        var cloneObject = CloneMethod?.Invoke(originalObject, null);
        if (typeToReflect.IsArray)
        {
            var arrayElementType = typeToReflect.GetElementType();
            if (arrayElementType != null && arrayElementType.IsPrimitive() == false)
            {
                var clonedArray = (Array?)cloneObject;
                clonedArray?.ForEach((array, indices) =>
                    array.SetValue(InternalCopy(clonedArray!.GetValue(indices), visited), indices));
            }
        }

        if (cloneObject != null)
        {
            visited.Add(originalObject, cloneObject);
            CopyFields(originalObject, visited, cloneObject, typeToReflect);
            RecursiveCopyBaseTypePrivateFields(originalObject, visited, cloneObject, typeToReflect);
        }
        return cloneObject;
    }

    private static void RecursiveCopyBaseTypePrivateFields(object originalObject, IDictionary<object, object> visited,
        object cloneObject, Type typeToReflect)
    {
        if (typeToReflect.BaseType != null)
        {
            RecursiveCopyBaseTypePrivateFields(originalObject, visited, cloneObject, typeToReflect.BaseType);
            CopyFields(originalObject, visited, cloneObject, typeToReflect.BaseType,
                BindingFlags.Instance | BindingFlags.NonPublic, fieldInfo => fieldInfo.IsPrivate);
        }
    }

    private static void CopyFields(object originalObject, IDictionary<object, object> visited, object cloneObject,
        Type typeToReflect,
        BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public |
                                    BindingFlags.FlattenHierarchy, Func<FieldInfo, bool>? filter = null)
    {
        foreach (var fieldInfo in typeToReflect.GetFields(bindingFlags))
        {
            if (filter != null && filter(fieldInfo) == false) continue;
            if (fieldInfo.FieldType.IsPrimitive()) continue;
            var originalFieldValue = fieldInfo.GetValue(originalObject);
            var clonedFieldValue = InternalCopy(originalFieldValue, visited);
            fieldInfo.SetValue(cloneObject, clonedFieldValue);
        }
    }

    /// <summary>
    /// Creates a deep copy of the specified object.
    /// </summary>
    /// <typeparam name="T">The type of the object.</typeparam>
    /// <param name="original">The object to copy.</param>
    /// <returns>A deep copy of the object.</returns>
    internal static T Copy<T>(this T original)
    {
        return original.Copy();
    }
}
