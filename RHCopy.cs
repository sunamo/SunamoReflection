
namespace SunamoReflection;
using SunamoReflection._sunamo;

public static class RHCopy
{
    public static Object Copy(Object originalObject)
    {
        return ObjectExtensions.InternalCopy(originalObject, new Dictionary<Object, Object>(new ReferenceEqualityComparer()));
    }
}