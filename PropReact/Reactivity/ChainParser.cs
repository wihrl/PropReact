using System.Linq.Expressions;
using System.Reflection;

namespace PropReact.Reactivity;

partial class PropChain
{
    private class ChainParser : ExpressionVisitor
    {
        int _depth, _lastLambdaDepth;
        ChainNode? _currentNode, _lastLambdaNode;

        private readonly Expression _expression;
        private readonly Action _action;

        private readonly PropChain _chain;

        public ChainParser(object owner, Expression expression, Action action)
        {
            _expression = expression;
            _action = action;
            _chain = new(owner);
        }

        public PropChain ParseChain()
        {
            Visit(_expression);
            _chain.Init();
            return _chain;
        }

        protected override Expression VisitMember(MemberExpression node)
        {
            var expr = base.VisitMember(node);

            IMember member = node.Member switch
            {
                FieldInfo fieldInfo => new FieldMember(fieldInfo),
                PropertyInfo propertyInfo => new PropertyMember(propertyInfo),
                _ => throw new ArgumentOutOfRangeException()
            };

            // prevent dangling
            if (!member.IsReadOnly &&
                !member.DeclaringType.IsAssignableTo(typeof(IValueOwner)) &&
                member.MemberInfo.CustomAttributes.All(x => x.AttributeType.Name != "InjectAttribute"))
                throw new ArgumentException(
                    "Selector expression cannot contain mutable non-reactive properties unless marked with [Immutable].");

            _currentNode = _currentNode is null
                ? _chain.GetStartingNode(member, _action)
                : _currentNode.Continue(member);

            _depth++;
            return expr;
        }

        protected override Expression VisitLambda<T>(Expression<T> node)
        {
            var previousDepth = _lastLambdaDepth = _depth;
            var previousNode = _lastLambdaNode = _currentNode;

            var res = base.VisitLambda(node);

            _lastLambdaDepth = previousDepth;
            _lastLambdaNode = previousNode;

            return res;
        }

        protected override Expression VisitParameter(ParameterExpression node)
        {
            _depth = _lastLambdaDepth;
            _currentNode = _lastLambdaNode;
            return base.VisitParameter(node);
        }
    }
}