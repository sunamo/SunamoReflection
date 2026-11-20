namespace SunamoReflection;

/// <summary>
/// Represents the array traverse type.
/// </summary>
public class ArrayTraverse
{
    private readonly int[] maxLengths;

    /// <summary>
    /// The position.
    /// </summary>
    public int[] Position;

    /// <summary>
    /// Array traverse.
    /// </summary>
    /// <param name="array">The array.</param>
    /// <returns>The array traverse result.</returns>
    public ArrayTraverse(Array array)
    {
        maxLengths = new int[array.Rank];
        for (var i = 0; i < array.Rank; ++i) maxLengths[i] = array.GetLength(i) - 1;
        Position = new int[array.Rank];
    }

    /// <summary>
    /// Step.
    /// </summary>
    /// <returns>The step result.</returns>
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
