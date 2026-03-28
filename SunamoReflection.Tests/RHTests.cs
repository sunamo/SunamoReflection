using SunamoReflection;
using System.Text;

/// <summary>
/// Unit tests for the RH (ReflectionHelper) class.
/// </summary>
public class RHTests
{
    /// <summary>
    /// Simple base class for testing property reflection.
    /// </summary>
    public class BaseClass
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; } = string.Empty;
    }

    /// <summary>
    /// Derived class with an internal property for testing declared-only property retrieval.
    /// </summary>
    public class ClassWithDerived : BaseClass
    {
        /// <summary>
        /// Gets or sets the age.
        /// </summary>
        internal int Age { get; set; }
    }

    class SubSubClass
    {
        public string Detail { get; set; } = string.Empty;
    }

    class SubClass
    {
        public string Description { get; set; } = string.Empty;
        public SubSubClass Nested { get; set; } = new();
    }

    class ClassWithSubclasses
    {
        public int Count { get; set; }
        public SubClass Child { get; set; } = new();
    }

    /// <summary>
    /// Verifies that PrintPublicPropertiesRecursively outputs the correct type hierarchy.
    /// </summary>
    [Fact]
    public void PrintPublicPropertiesRecursivelyTest()
    {
        ClassWithSubclasses testObject = new();
        StringBuilder stringBuilder = new();

        RH.PrintPublicPropertiesRecursively(stringBuilder, testObject.GetType(), "  ");

        var text = stringBuilder.ToString();
        Assert.Contains("ClassWithSubclasses", text);
        Assert.Contains("Count", text);
        Assert.Contains("Child", text);
        Assert.Contains("SubClass", text);
    }

    /// <summary>
    /// Verifies that GetPropertyNames returns only declared properties, not inherited ones.
    /// </summary>
    [Fact]
    public void GetPropertyNamesTest()
    {
        var baseClassProperties = RH.GetPropertyNames(typeof(BaseClass));
        var derivedClassProperties = RH.GetPropertyNames(typeof(ClassWithDerived));

        Assert.Equal(["Name"], baseClassProperties);
        Assert.Equal(["Age"], derivedClassProperties);
    }

    /// <summary>
    /// Verifies that GetValuesOfPropertyOrField returns property values from an object.
    /// </summary>
    [Fact]
    public void GetValuesOfPropertyOrFieldTest()
    {
        var testObject = new BaseClass { Name = "TestName" };
        var values = RH.GetValuesOfPropertyOrField(testObject);

        Assert.Single(values);
        Assert.Contains("Name", values[0]);
        Assert.Contains("TestName", values[0]);
    }
}
