using System.Reflection;
using CustomORM.Npsql;

namespace CustomORM.CustomEF;

public class CustomContext : ICustomContext
{
    private readonly ICustomConnection _connection;

    protected CustomContext(ICustomConnection connection)
    {
        _connection = connection;
        var sets =
            GetType().GetProperties()
                .Where(x => x.PropertyType.GetGenericTypeDefinition() == typeof(CustomDbSet<>));

        foreach (var set in sets)
        {
            var constructedType = typeof(CustomDbSet<>).MakeGenericType(set.PropertyType.GetGenericArguments()[0]);
            set.SetValue(this,
                Activator.CreateInstance(constructedType,
                    new object[]
                        { new CustomQueryProvider(this) }));
            
            //set.SetValue(this, CreateSet(set.PropertyType.GetGenericArguments()[0]));
        }
    }

    private object? CreateSet(Type setPropertyType)
    {
        return typeof(CustomContext).GetMethods(BindingFlags.NonPublic | BindingFlags.Instance |
                                                BindingFlags.FlattenHierarchy | BindingFlags.Public)
            .Single(x => x.Name == nameof(CreateSetInternal))
            .MakeGenericMethod(setPropertyType)
            .Invoke(this, Array.Empty<object>());
    }

    private object CreateSetInternal<T>() => new CustomDbSet<T>(new CustomQueryProvider(this));

    public string ResolveTableName(Type EntityType)
    {
        return $"\"{EntityType.GetGenericArguments()[0].Name}s\"";
    }

    public TResult QueryAsync<TResult>(FormattableString sql)
    {
        if (typeof(TResult).IsGenericType && typeof(TResult).GetGenericTypeDefinition() == typeof(IEnumerable<>))
        {
            var entityType = typeof(TResult).GetGenericArguments()[0];
            var task = (Task)QueryMapper.QueryAsyncType(_connection, sql, entityType, new CancellationToken());
            task.Wait();
            dynamic dym = task;
            return dym.Result;
        }

        throw new InvalidOperationException();
    }
}