using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace CustomORM.CustomEF;

public class QueryBuilder : ExpressionVisitor
{
    private Expression _selectListExpression, _whereExression;

    protected override Expression VisitMethodCall(MethodCallExpression node)
    {
        if (node.Method.IsGenericMethod)
        {
            var basicMethod = node.Method.GetGenericMethodDefinition();
            if (basicMethod == QueryableMethods.Select)
            {
                VisitorSelect(node);
            }
            else if (basicMethod == QueryableMethods.Where)
            {
                VisitorWhere(node);
            }
        }

        return base.VisitMethodCall(node);
    }

    private void VisitorWhere(MethodCallExpression node)
    {
        _whereExression = ((UnaryExpression)(node.Arguments[1])).Operand;
    }

    private void VisitorSelect(MethodCallExpression node)
    {
        _selectListExpression = ((UnaryExpression)(node.Arguments[1])).Operand;
    }

    public FormattableString Compile(Expression expression)
    {
        Visit(expression);
        var whereVisitor = new WhereVisitor();
        whereVisitor.Visit(_whereExression);
        var resultPredicate = whereVisitor.Result;
        
        var selectVisitor = new SelectVisitor();
        selectVisitor.Visit(_selectListExpression);
        var selectList = selectVisitor.Result;
        
        return FormattableStringFactory.Create("");
    }

    internal class StringExpression : Expression
    {
        public StringExpression(string @string, ExpressionType nodeType, Type type)
        {
            String = @string;
            NodeType = nodeType;
            Type = type;
        }

        public string String { get; }
        public override ExpressionType NodeType { get; }
        public override Type Type { get; }
    }

    internal class WhereVisitor : ExpressionVisitor
    {
        public string Result { get; set; }

        protected override Expression VisitBinary(BinaryExpression node)
        {
            var @operator = node.NodeType switch
            {
                ExpressionType.GreaterThan => ">",
                ExpressionType.Equal => "=",
                ExpressionType.OrElse => "OR",
                ExpressionType.AndAlso => "AND",
                ExpressionType.LessThan => "<"
            };
            var left = ToString(node.Left);
            var right = ToString(node.Right);
            Result = $"{left} {@operator} {right}";
            return base.VisitBinary(node);
        }

        private string ToString(Expression expression)
        {
            if (expression is ConstantExpression cs)
                return cs.Value.ToString();
            return $"\"{((MemberExpression)expression).Member.Name}\"";
        }
    }

    internal class SelectVisitor : ExpressionVisitor
    {
        public string Result { get; set; }

        protected override Expression VisitMemberInit(MemberInitExpression node)
        {
            var nodes = node.Bindings
                .Cast<MemberAssignment>()
                .Select(x => ToString(x.Expression));
            Result = string.Join(", ", nodes);
            return base.VisitMemberInit(node);
        }

        private string ToString(Expression expression)
        {
            if (expression is ConstantExpression cs)
                return cs.Value.ToString();
            return $"\"{((MemberExpression)expression).Member.Name}\"";
        }
    }
}