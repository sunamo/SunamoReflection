// EN: Variable names have been checked and replaced with self-descriptive names
// CZ: Názvy proměnných byly zkontrolovány a nahrazeny samopopisnými názvy

using SunamoReflection;
using System.Text;

public class RHTests
{
    public class BaseClass
    {
        public string Name { get; set; }

    }

    public class ClassWithDerived : BaseClass
    {
        internal int Age { get; set; }
    }

    class SubSubClass
    {
        public string E { get; set; }
    }

    class SubClass
    {
        public string D { get; set; }
        public SubSubClass F { get; set; }
    }

    class ClassWithSubclasses
    {
        public int A { get; set; }
        public SubClass C { get; set; }
    }

    [Fact]
    public void PrintPublicPropertiesRecursivelyTest()
    {
        ClassWithSubclasses c = new();
        StringBuilder stringBuilder = new();

        RH.PrintPublicPropertiesRecursively(stringBuilder, c.GetType(), "  ");

        var text = stringBuilder.ToString();
    }

    [Fact]
    public void GetPropertyNamesTest()
    {
        var actual1 = RH.GetPropertyNames(typeof(BaseClass));
        var actual2 = RH.GetPropertyNames(typeof(ClassWithDerived));

        Assert.Equal(["Name"], actual1);
        Assert.Equal(["Age"], actual2);
    }

    [Fact]
    public void GetValuesOfPropertyOrFieldTest()
    {
        //// FromTo - simple variables
        //FromTo ft = new FromTo(0, 1);

        //List<string> onlyNames = new List<string>();

        //var result = string.Join(Environment.NewLine, RH.GetValuesOfPropertyOrField(ft, onlyNames.ToArray()));

        //onlyNames.Add("from");
        //var r2 = string.Join(Environment.NewLine, RH.GetValuesOfPropertyOrField(ft, onlyNames.ToArray()));

        //// UploadFile - properties
        //UploadFile uf = new UploadFile();
        //uf.Filename = "d";
        //uf.Name = "name";

        //onlyNames.Clear();

        //var r3 = string.Join(Environment.NewLine, RH.GetValuesOfPropertyOrField(uf, onlyNames.ToArray()));

        //onlyNames.Add("Name");
        //var r4 = string.Join(Environment.NewLine, RH.GetValuesOfPropertyOrField(uf, onlyNames.ToArray()));

        //int i = 0;
    }
}
