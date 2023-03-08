using System.Linq.Expressions;

namespace CustomORM.CustomEF;

public class CustomQueryProvider : IQueryProvider
{
    public IQueryable CreateQuery(Expression expression)
    {
        return CreateQuery<object>(expression);
    }

    public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
    {
        return new CustomQueryable<TElement>( expression, this);
    }

    public object? Execute(Expression expression)
    {
        return Execute<object>(expression);
    }

    public TResult Execute<TResult>(Expression expression)
    {
        var result = new QueryBuilder();
        var sql = result.Compile(expression);

      return  QueryMapper.QueryAsync<TResult>(null, null, default).Result.Single();
        
    }
}