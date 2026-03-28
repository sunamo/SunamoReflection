namespace SunamoReflection;

/// <summary>
/// Compares objects by reference equality only, ignoring value equality.
/// Used for deep copy cycle detection.
/// </summary>
public class ReferenceEqualityComparer : EqualityComparer<object>
{
    /// <summary>
    /// Determines whether the two objects are the same reference.
    /// </summary>
    /// <param name="x">The first object.</param>
    /// <param name="y">The second object.</param>
    /// <returns>True if both references point to the same object.</returns>
    public override bool Equals(object? x, object? y)
    {
        return ReferenceEquals(x, y);
    }

    /// <summary>
    /// Returns the hash code of the object.
    /// </summary>
    /// <param name="obj">The object to get the hash code for.</param>
    /// <returns>Hash code, or 0 if null.</returns>
    public override int GetHashCode([DisallowNull] object obj)
    {
        if (obj == null) return 0;
        return obj.GetHashCode();
    }
}
