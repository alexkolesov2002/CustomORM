using System.Collections;
using System.Linq.Expressions;

namespace CustomORM.CustomEF;

public class CustomQueryable<T> : IQueryable<T>
{
    public CustomQueryable(IQueryProvider provider)
    {
        Expression = Expression.Constant(this);
        Provider = provider;
        ElementType = typeof(T);
    }

    public CustomQueryable(Expression expression, CustomQueryProvider customQueryProvider)
    {
        Expression = expression;
        Provider = customQueryProvider;
        ElementType = typeof(T);
    }

    public IEnumerator<T> GetEnumerator()
    {
       return Provider.Execute<IEnumerable<T>>(Expression).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public Type ElementType { get; }
    public Expression Expression { get; }
    public IQueryProvider Provider { get; }
}