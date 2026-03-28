namespace SunamoReflection;

/// <summary>
/// Extension methods for the Array type.
/// </summary>
public static class ArrayExtensions
{
    /// <summary>
    /// Iterates over every element in a multi-dimensional array and invokes the specified action with the array and current indices.
    /// </summary>
    /// <param name="array">The array to iterate over.</param>
    /// <param name="action">The action to invoke for each element, receiving the array and current indices.</param>
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
