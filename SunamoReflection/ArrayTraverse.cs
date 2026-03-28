namespace SunamoReflection;

/// <summary>
/// Traverses a multi-dimensional array by stepping through all index combinations.
/// </summary>
public class ArrayTraverse
{
    private readonly int[] maxLengths;

    /// <summary>
    /// The current position in the multi-dimensional array.
    /// </summary>
    public int[] Position;

    /// <summary>
    /// Initializes a new instance for traversing the specified array.
    /// </summary>
    /// <param name="array">The array to traverse.</param>
    public ArrayTraverse(Array array)
    {
        maxLengths = new int[array.Rank];
        for (var i = 0; i < array.Rank; ++i) maxLengths[i] = array.GetLength(i) - 1;
        Position = new int[array.Rank];
    }

    /// <summary>
    /// Advances the position to the next element in the array.
    /// </summary>
    /// <returns>True if there are more elements, false if traversal is complete.</returns>
    public bool Step()
    {
        for (var i = 0; i < Position.Length; ++i)
            if (Position[i] < maxLengths[i])
            {
                Position[i]++;
                for (var j = 0; j < i; j++) Position[j] = 0;
                return true;
            }

        return false;
    }
}
