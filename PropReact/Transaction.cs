// using PropReact.Properties;
//
// namespace PropReact;
//
// public sealed class Transaction : IDisposable
// {
//     public static bool Enforce { get; set; }= true;
//     public static int TransactionId { get; private set; }
//     
//     private static object _lock = new();
//     private static Transaction? _currentTransaction;
//     private static Queue<Action> _pendingChanges = new();
//
//     private int _stackDepth;
//
//     private Transaction()
//     {
//         TransactionId++;
//     }
//
//     public static Transaction Begin()
//     {
//         Monitor.Enter(_lock);
//         var transaction = _currentTransaction ??= new();
//         transaction._stackDepth++;
//         return transaction;
//     }
//
//     public void Dispose()
//     {
//         // transaction is closed including all subtranscations
//         if (_stackDepth == 1)
//         {
//             while (_pendingChanges.TryDequeue(out var action)) action();
//             
//             // NOTE: during executing actions more actions can be added
//             // for example if a parent blazor component re-renders and changes parameters of its children, those
//             // children need to be rendered again despite already having been rendered before
//             // as such, it is also impossible to use a hashset since that would prevent multiple uses of the same action
//
//             _currentTransaction = null;
//         }
//
//         _stackDepth--;
//         Monitor.Exit(_lock);
//     }
//
//     internal static void PostOrExecute(Action action)
//     {
//         if (Monitor.IsEntered(_lock)) _pendingChanges.Enqueue(action);
//         else if (Enforce) throw new("An attempt was made to mutate a prop outside of a transaction.");
//         else action();
//     }
//
//     // /// <summary>
//     // /// Registers a callback to execute when the current transaction ends. Useful to prevent multiple invokes.
//     // /// If no transaction is pending, executes the action immediately.
//     // /// </summary>
//     // public static void OnFinalize(Action action)
//     // {
//     //     if (Monitor.IsEntered(_lock)) _finalizeActions.Add(action);
//     //     else action();
//     // }
//
//     public static void Run(Action action)
//     {
//         using var _ = Begin();
//         action();
//     }
//
//     public static void Set<T>(IMutable<T> mutable, T value)
//     {
//         using var _ = Begin();
//         mutable.Value = value;
//     }
// }