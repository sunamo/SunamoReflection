namespace sunamo.Tests;

public class RHCopyTests
{
    [Fact]
    public void CopyTest()
    {
        var fixture = new Fixture();
        var expected = fixture.CreateMany<TypeWithProperties>();

        var actual = RHCopy.Copy(expected);

        Assert.Equivalent(expected, actual);
    }
}
