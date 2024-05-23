namespace SunamoReflection;

using System.Xml.Serialization;

public class RHSE
{
    /// <summary>
    ///     Usage: some methods just dump exceptions object
    /// </summary>
    /// <param name="empty"></param>
    /// <param name="output"></param>
    /// <returns></returns>
    public static string DumpAsXml(object output)
    {
        string objectAsXmlString;
        XmlSerializer xs = new(output.GetType());
        using (StringWriter sw = new())
        {
            try
            {
                xs.Serialize(sw, output);
                objectAsXmlString = sw.ToString();
            }
            catch (Exception ex)
            {
                objectAsXmlString = ex.ToString();
            }
        }
        return objectAsXmlString;
    }
    public static bool IsOrIsDeriveFromBaseClass(Type children, Type parent, bool a1CanBeString = true)
    {
        if (children == Types.tString && !a1CanBeString) return false;
        if (children == null) ThrowEx.IsNull("children", children);
        while (true)
        {
            if (children == null) return false;
            if (children == parent) return true;
            foreach (var inter in children.GetInterfaces())
                if (inter == parent)
                    return true;
            children = children.BaseType;
        }
    }
    #region from RHShared64.cs
    #region Get types of class
    ///// <summary>
    ///// Return FieldInfo, so will be useful extract Name etc.
    ///// </summary>
    ///// <param name="type"></param>
    //public static List<FieldInfo> GetConsts(Type type, GetMemberArgs a = null)
    //{
    //    if (a == null)
    //    {
    //        a = new GetMemberArgs();
    //    }
    //    IEnumerable<FieldInfo> fieldInfos = null;
    //    if (a.onlyPublic)
    //    {
    //        fieldInfos = type.GetFields(BindingFlags.Public | BindingFlags.Static |
    //        // return protected/public but not private
    //        BindingFlags.FlattenHierarchy).ToList();
    //    }
    //    else
    //    {
    //        ///fieldInfos = type.GetFields(BindingFlags.Static);//.Where(f => f.IsLiteral);
    //        fieldInfos = type.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.NonPublic |
    //          BindingFlags.FlattenHierarchy).ToList();
    //    }
    //    var withType = fieldInfos.Where(fi => fi.IsLiteral && !fi.IsInitOnly).ToList();
    //    return withType;
    //}
    #endregion
    #endregion
}