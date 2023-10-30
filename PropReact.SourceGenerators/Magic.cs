namespace PropReact.SourceGenerators;

/// <summary>
/// Since source generators only support netstandard2.0, in order to avoid having to split the main project up and needlessly
/// complicate things, here's a bunch of magic strings that definitely won't change in the future.
/// </summary>
public static class Magic
{
    #region Prop interfaces

    public const string Mutable = "IMutable";
    public const string Computed = "IComputed";

    #endregion

    #region Attributes

    public const string DontExpose = "DontExpose";
    public const string GetOnly = "GetOnly";

    #endregion
}