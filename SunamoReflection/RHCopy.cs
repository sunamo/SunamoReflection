// EN: Variable names have been checked and replaced with self-descriptive names
// CZ: Názvy proměnných byly zkontrolovány a nahrazeny samopopisnými názvy
namespace SunamoReflection;

public static class RHCopy
{
    public static object Copy(object originalObject)
    {
        return ObjectExtensions.InternalCopy(originalObject,
            new Dictionary<object, object>(new ReferenceEqualityComparer()));
    }
}