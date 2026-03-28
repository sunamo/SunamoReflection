namespace RunnerReflection;

/// <summary>
/// Entry point for the RunnerReflection console application.
/// </summary>
internal class Program
{
    static void Main()
    {
        RHTests t = new();
        t.PrintPublicPropertiesRecursivelyTest();
    }
}
