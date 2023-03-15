using System.Linq.Expressions;

namespace CustomORM.CustomEF;

public class CustomQueryProvider : IQueryProvider
{

    private readonly ICustomContext _customContext;

    public CustomQueryProvider(ICustomContext customContext)
    {
        _customContext = customContext;
    }

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
        var result = new QueryBuilder(_customContext);
        var sql = result.Compile(expression);

      return  _customContext.QueryAsync<TResult>(sql);
        
    }
}