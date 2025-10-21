// EN: Variable names have been checked and replaced with self-descriptive names
// CZ: Názvy proměnných byly zkontrolovány a nahrazeny samopopisnými názvy
namespace SunamoReflection;

public class ReferenceEqualityComparer : EqualityComparer<object>
{
    public override bool Equals(object x, object y)
    {
        return ReferenceEquals(x, y);
    }

    public override int GetHashCode(object obj)
    {
        if (obj == null) return 0;
        return obj.GetHashCode();
    }
}