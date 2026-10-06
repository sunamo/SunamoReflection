namespace SunamoReflection;

public class ReferenceEqualityComparer : EqualityComparer<object>
{
    public override bool Equals(object? x, object? y)
    {
        return ReferenceEquals(x, y);
    }

    public override int GetHashCode([DisallowNull] object obj)
    {
        if (obj == null) return 0;
        return obj.GetHashCode();
    }
}
