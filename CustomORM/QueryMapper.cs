using System.Collections.Concurrent;
using System.Data;
using System.Linq.Expressions;
using System.Reflection;
using CustomORM.Npsql;

namespace CustomORM;

static class QueryMapper
{
    private static readonly MethodInfo? GetStringMethodInfo = typeof(DataReadeExtensions).GetMethod("TryGetString");
    private static readonly MethodInfo? GetInt32MethodInfo = typeof(DataReadeExtensions).GetMethod("TryGetInt32");

    private static readonly ConcurrentDictionary<Type, Delegate> MapperFunc = new();

    /*public static async Task<List<Document>> QueryAsync(this ICustomCommand connection, string sql,
    CancellationToken cancellationToken)
{
    await using var command = connection.CreateCommand();
    command.CommandText = sql;
    await using var reader = await command.ExecuteReaderAsync(cancellationToken);

    var documents = new List<Document>();

    while (await reader.ReadAsync(cancellationToken))
    {
        documents.Add(new Document()
        {
            Id = reader.TryGetInt32("Id"),
            Content = reader.TryGetString("Content")
        });
    }

    return documents;
}*/

    public static async Task<List<T>> QueryAsync<T>(this ICustomConnection connection, FormattableString sql,
        CancellationToken cancellationToken)
    {
        await using var command = connection.CreateCommand(sql);

        using var reader = await command.ExecuteReaderAsync(cancellationToken);

        var list = new List<T>();
        var func = (Func<IDataReader, T>)MapperFunc.GetOrAdd(typeof(T), x => Build<T>());

        while (reader.Read())
        {
            list.Add(func(reader));
        }

        return list;
    }


    private static Func<IDataReader, T> Build<T>()
    {
        var readerParam = Expression.Parameter(typeof(IDataReader));

        var newExpression = Expression.New(typeof(T));
        var memberInitExpression = Expression.MemberInit(newExpression,
            typeof(T).GetProperties()
                .Select(prop => Expression.Bind(prop, BuildReadColumnExpression(readerParam, prop))));

        return Expression.Lambda<Func<IDataReader, T>>(memberInitExpression, readerParam).Compile();
    }

    private static Expression BuildReadColumnExpression(ParameterExpression reader, PropertyInfo propertyInfo)
    {
        if (propertyInfo.PropertyType == typeof(string))
        {
            if (GetStringMethodInfo != null)
                return Expression.Call(null, GetStringMethodInfo, reader, Expression.Constant(propertyInfo.Name));
        }

        if (propertyInfo.PropertyType == typeof(int))
        {
            if (GetInt32MethodInfo != null)
                return Expression.Call(null, GetInt32MethodInfo, reader, Expression.Constant(propertyInfo.Name));
        }

        throw new InvalidOperationException();
    }
}