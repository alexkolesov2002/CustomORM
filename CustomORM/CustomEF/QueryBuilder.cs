using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace CustomORM.CustomEF;

public class QueryBuilder : ExpressionVisitor
{
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
    }

    private void VisitorSelect(MethodCallExpression node)
    {
    }

    public FormattableString Compile(Expression expression)
    {
        Visit(expression);
        return FormattableStringFactory.Create("");
    }
}