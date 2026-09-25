namespace SunamoReflection._public.SunamoArgs;

/// <summary>
/// Arguments controlling which members to retrieve via reflection.
/// </summary>
public class GetMemberArgs
{
    /// <summary>
    /// When true, only public members are retrieved.
    /// </summary>
    public bool IsOnlyPublic { get; set; } = true;
}
