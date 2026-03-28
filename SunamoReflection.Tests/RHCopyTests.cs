namespace sunamo.Tests;

/// <summary>
/// Unit tests for the RHCopy deep copy functionality.
/// </summary>
public class RHCopyTests
{
    /// <summary>
    /// Verifies that RHCopy.Copy creates an equivalent deep copy of a collection of typed objects.
    /// </summary>
    [Fact]
    public void CopyTest()
    {
        var fixture = new Fixture();
        var expected = fixture.CreateMany<TypeWithProperties>();

        var actual = RHCopy.Copy(expected);

        Assert.Equivalent(expected, actual);
    }
}
