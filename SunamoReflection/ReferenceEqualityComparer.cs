namespace SunamoReflection;

/// <summary>
/// Represents the reference equality comparer type.
/// </summary>
public class ReferenceEqualityComparer : EqualityComparer<object>
{
    /// <summary>
    /// Equals.
    /// </summary>
    /// <param name="x">The x.</param>
    /// <param name="y">The y.</param>
    /// <returns>The equals result.</returns>
    public override bool Equals(object? x, object? y)
    {
        return ReferenceEquals(x, y);
    }

    /// <summary>
    /// Get hash code.
    /// </summary>
    /// <param name="obj">The obj.</param>
    /// <returns>The get hash code result.</returns>
    public override int GetHashCode([DisallowNull] object obj)
    {
        if (obj == null) return 0;
        return obj.GetHashCode();
    }
}
