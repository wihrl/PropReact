namespace PropReact.Chain;

public static class ReactiveChain
{
    public static RootNode<T> Make<T>(Action<RootNode<T>> builder)
    {
        var root = new RootNode<T>();
        builder(root);
        return root;
    }
}