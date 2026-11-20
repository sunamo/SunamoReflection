namespace SunamoReflection;

/// <summary>
/// Represents the array extensions type.
/// </summary>
public static class ArrayExtensions
{
    /// <summary>
    /// For each.
    /// </summary>
    /// <param name="array">The array.</param>
    /// <param name="action">The action.</param>
    public static void ForEach(this Array array, Action<Array, int[]> action)
    {
        if (array.LongLength == 0) return;
        var walker = new ArrayTraverse(array);
        do
        {
            action(array, walker.Position);
        } while (walker.Step());
    }
}
