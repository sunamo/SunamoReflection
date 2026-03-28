namespace SunamoReflection;

/// <summary>
/// Provides deep copy functionality for objects using reflection-based cloning.
/// </summary>
public static class RHCopy
{
    /// <summary>
    /// Creates a deep copy of the specified object, handling circular references.
    /// </summary>
    /// <param name="originalObject">The object to copy.</param>
    /// <returns>A deep copy of the object.</returns>
    public static object? Copy(object originalObject)
    {
        return ObjectExtensions.InternalCopy(originalObject,
            new Dictionary<object, object>(new ReferenceEqualityComparer()));
    }
}
