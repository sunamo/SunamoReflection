namespace SunamoReflection;

public static class RHCopy
{
    public static object Copy(object originalObject)
    {
        return ObjectExtensions.InternalCopy(originalObject,
            new Dictionary<object, object>(new ReferenceEqualityComparer()));
    }
}