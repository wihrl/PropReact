namespace PropReact.Chain;

public class RootNode<TRoot> : ChainNode<TRoot>
{
    public void AddReaction(TRoot root, Action action)
    {
        foreach (var chainNode in _next) chainNode.ChangeSource(default, root, new[] { action });
    }

    public static RootNode<TRoot> Make(out RootNode<TRoot> root)
    {
        return root = new();
    }
}