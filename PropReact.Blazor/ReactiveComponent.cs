using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Components;
using PropReact.Properties;

namespace PropReact.Blazor;

public class ReactiveComponent<TComponent> : ComponentBase, IValueOwner
    where TComponent : ReactiveComponent<TComponent>
{
    private readonly List<IDisposable> _disposables = new();
    private readonly Dictionary<int, object> _lambdas = new();

    private TComponent This => (TComponent) this;
    
    // todo: custom ComponentBase that queues renders until transaction finished

    public void Dispose()
    {
        foreach (var disposable in _disposables) disposable.Dispose();
        OnDispose();
    }

    protected virtual void OnDispose()
    {
    }

    protected TValue Watch<TValue>(Expression<Func<TComponent, TValue>> selector, out TValue value, [CallerLineNumber] int id = 0)
    {
        // ReSharper disable once ExplicitCallerInfoArgument
        value = Watch(selector, id);
        return value;
    }
    
    protected TValue Watch<TValue>(Expression<Func<TComponent, TValue>> selector, [CallerLineNumber] int id = 0)
    {
        // todo: disable chains that were not used in last rerender to prevent false conditionals triggering rerenders

        if (_lambdas.TryGetValue(id, out var getter))
            return ((Func<TComponent, TValue>) getter)(This);

        // var prependedSelector =
        //     (Expression<Func<ReactiveComponent<TComponent>, TValue>>)
        //     new Visitor<TComponent, TValue>(x => x.This, selector).Visit(selector);

        var backingFieldSelector =
            (Expression<Func<TComponent, TValue>>) new ParamToBackingVisitor(this).Visit(selector);

        var compiledSelector = backingFieldSelector.Compile();

        // todo: dispose
        Prop.Watch((TComponent) this, backingFieldSelector, _ => Transaction.PostOrExecute(InvokeChanged),
            compiledSelector);

        _lambdas[id] = compiledSelector;
        return compiledSelector(This);
    }

    // protected TValue Watch<TProp, TValue>(IProp<TProp> prop, Expression<Func<TProp, TValue>> selector,
    //     [CallerLineNumber] int id = 0)
    // {
    //     if (_lambdas.TryGetValue(id, out var getter))
    //         return ((Func<TComponent, TValue>) getter)(This);
    //     
    //     var prependedSelector =
    //         (Expression<Func<ReactiveComponent<TComponent>, TValue>>)
    //              new Visitor<IProp<TProp>, TValue>(x => x.This, selector).Visit(selector);
    //     
    //     var compiledSelector = selector.Compile();
    //     
    //     // todo: dispose
    //     Prop.Watch(prop, selector, _ => Transaction.OnFinalize(InvokeChanged), () => compiledSelector(prop.V));
    //     
    //     _lambdas[id] = compiledSelector;
    //     return compiledSelector(prop.V);
    // }

    protected new void StateHasChanged() => InvokeChanged();

    void InvokeChanged()
    {
        _dirty = true;
        Dirtied();
        InvokeAsync(base.StateHasChanged);
    }

    protected virtual void Dirtied(){}

    private bool _dirty;

    protected override bool ShouldRender()
    {
        // prevent parent render from re-rendering the child
        if (!_dirty) return false;
        _dirty = false;
        return true;
    }

    protected override void OnParametersSet()
    {
        // todo: measure time spent rendering
        // StateHasChanged();
        base.OnParametersSet();
    }

    void IValueOwner.Sub(IChangeObserver changeObserver)
    {
        // todo: implement, add prop to ichangeobserver to send notifications on ParametersSet()....
    }

    void IValueOwner.Unsub(IChangeObserver changeObserver)
    {
    }

    private class PrependVisitor<TComponent, TValue> : ExpressionVisitor
        where TComponent : ReactiveComponent<TComponent>
    {
        private readonly Expression<Func<ReactiveComponent<TComponent>, TComponent>> _rootGetter;
        private readonly ParameterExpression _rootParam;
        private Expression _rootLambda;

        public PrependVisitor(Expression<Func<ReactiveComponent<TComponent>, TComponent>> rootGetter,
            Expression<Func<TComponent, TValue>> selector)
        {
            _rootGetter = rootGetter;
            _rootParam = selector.Parameters.First();
            _rootLambda = selector;
        }

        protected override Expression VisitParameter(ParameterExpression node)
        {
            return node == _rootParam ? _rootGetter.Body : base.VisitParameter(node);
        }

        protected override Expression VisitLambda<T>(Expression<T> node)
        {
            return node == _rootLambda
                ? Expression.Lambda(Visit(node.Body), _rootGetter.Parameters)
                : base.VisitLambda(node);
        }
    }

    // todo: will no longer be necessary after switch to source generator proxy properties and chain parsers!
    class ParamToBackingVisitor : ExpressionVisitor
    {
        private object _root;

        public ParamToBackingVisitor(object root)
        {
            _root = root;
        }

        protected override Expression VisitMember(MemberExpression node)
        {
            if (node.Member.DeclaringType.IsAssignableTo(typeof(ComponentBase)) &&
                node.Member.GetCustomAttribute<ParameterAttribute>() is not null)
            {
                var backingFieldName = "_" + char.ToLower(node.Member.Name[0]) + node.Member.Name[1..];
                var fields = _root.GetType().GetFields();
                var backingField = _root.GetType().GetField(backingFieldName,
                    BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
                if (backingField is null)
                    throw new Exception("Parameter backing field not found. Expected: " + backingFieldName);

                var valueProp = backingField.FieldType.GetProperty(nameof(IProp<int>.V));
                if(valueProp is null)
                    throw new Exception("Invalid backing field. (" + backingFieldName + ")");
                
                var res = Expression.MakeMemberAccess(Expression.MakeMemberAccess(Visit(node.Expression), backingField), valueProp);
                return res;
            }

            return base.VisitMember(node);
        }
    }
}