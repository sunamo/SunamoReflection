namespace SunamoReflection;


public partial class RH
{
    #region For easy copy


    public static object SetValueOfProperty(string name, Type type, object instance, bool ignoreCase, object v)
    {
        PropertyInfo[] pis = type.GetProperties();
        return SetValue(name, type, instance, pis, ignoreCase, v);
    }

    public static object SetValue(string name, Type type, object instance, IList pis, bool ignoreCase, object v)
    {
        return GetOrSetValue(name, type, instance, pis, ignoreCase, SetValue, v);
    }

    private static object SetValue(object instance, MemberInfo[] property, object v)
    {
        var val = property[0];
        if (val is PropertyInfo)
        {
            var pi = (PropertyInfo)val;
            pi.SetValue(instance, v);
        }
        else if (val is FieldInfo)
        {
            var pi = (FieldInfo)val;
            pi.SetValue(instance, v);
        }
        return null;
    }

    public static bool ExistsAssemblyNotFullName(string v)
    {
        var execAssembly = Assembly.GetEntryAssembly();
        var refAss = RH.AllReferencedAssemblies(execAssembly);
#if DEBUG
        //var names = refAss.Select(d => d.Name).ToList();
        refAss.Sort();
        if (v == "Aps.Xlf")
        {

        }
#endif

        if (refAss.Contains(v))
        {
            return true;
        }
        return false;
    }

    static List<string> allReferencedAssemblies = new List<string>();

    public static List<string> AllReferencedAssemblies(Assembly execAssembly, bool useCache = true)
    {
        if (!useCache)
        {
            allReferencedAssemblies.Clear();
        }
        else
        {
            if (allReferencedAssemblies.Count != 0)
            {
                return allReferencedAssemblies;
            }
        }

        var refAss = execAssembly.GetReferencedAssemblies();
        foreach (var item in refAss)
        {
            AllReferencedAssemblies(allReferencedAssemblies, item);
        }
        //result.AddRange(refAss.Select(d => d.Name));
        return allReferencedAssemblies;
    }

    private static void AllReferencedAssemblies(List<string> n, AssemblyName execAssembly)
    {
        if (n.Contains(execAssembly.Name))
        {
            return;
        }
        n.Add(execAssembly.Name);
        Assembly ass = null;
        try
        {
            ass = Assembly.Load(execAssembly);
        }
        catch (Exception ex)
        {
            return;
        }
        var refAss = ass.GetReferencedAssemblies();
        foreach (var item in refAss)
        {
            AllReferencedAssemblies(n, item);
        }
    }



    public static bool ExistsClass(string className)
    {
        var type2 = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                     from type in assembly.GetTypes()
                     where type.Name == className
                     select type).FirstOrDefault();

        return type2 != null;
    }
    #endregion
}
