namespace SunamoReflection;

/// <summary>
/// Represents the RH copy type.
/// </summary>
public static class RHCopy
{
    /// <summary>
    /// Copy.
    /// </summary>
    /// <param name="originalObject">The original object.</param>
    /// <returns>The copy result.</returns>
    public static object? Copy(object originalObject)
    {
        return ObjectExtensions.InternalCopy(originalObject,
            new Dictionary<object, object>(new ReferenceEqualityComparer()));
    }
}
