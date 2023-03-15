using System.Collections;
using System.Collections.Concurrent;
using System.Data;
using System.Linq.Expressions;
using System.Reflection;
using CustomORM.Npsql;

namespace CustomORM;

/// <summary>
/// Позволяет конфигурировать создавать листы объектов из базы данных
/// </summary>
public static class QueryMapper
{
    /// <summary>
    /// Достаем метод для его вызове в Expression (метод возвращает строковое значение)
    /// </summary>
    private static readonly MethodInfo? GetStringMethodInfo = typeof(DataReadeExtensions).GetMethod("TryGetString");

    /// <summary>
    /// Достаем метод для его вызове в Expression (метод возвращает  значение типа Int32)
    /// </summary>
    private static readonly MethodInfo? GetInt32MethodInfo = typeof(DataReadeExtensions).GetMethod("TryGetInt32");

    /// <summary>
    /// Словарь для кеширования делегатов
    /// </summary>
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

    /// <summary>
    /// Возвращает лист отпределенной сущности БД в соответсвии с запросом Select * from  "Documents" ( в качестве примера)
    /// </summary>
    /// <param name="connection">Кастомный DB connection</param>
    /// <param name="sql">SQL запрос</param>
    /// <param name="cancellationToken"></param>
    /// <typeparam name="T">Класс в который будет маппиться SQL запрос</typeparam>
    /// <returns>Лист класса T из БД</returns>
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


    public static async Task<IList> QueryAsyncType(this ICustomConnection connection, FormattableString sql,
        Type entityType,
        CancellationToken cancellationToken)
    {
        var result =  (Task)typeof(QueryMapper).GetMethod("QueryAsync")!
            .MakeGenericMethod(entityType)
            .Invoke(null, new object [] { connection, sql, cancellationToken });

        if (result != null) await result;
        dynamic? res = result;
        return res.Result;
    }


    /// <summary>
    /// Метод для создание объектов класса T
    /// </summary>
    /// <typeparam name="T">Класс</typeparam>
    /// <returns>возвращает делегат, который принимает в себя DataReader  и возвращает объект класса T</returns>
    private static Func<IDataReader, T> Build<T>()
    {
        var readerParam = Expression.Parameter(typeof(IDataReader)); // Параметр для Expression DataReader

        var newExpression = Expression.New(typeof(T)); // Expression создания новой сущности
        var memberInitExpression = Expression.MemberInit(newExpression,
            typeof(T).GetProperties()
                .Select(prop => Expression.Bind(prop,
                    BuildReadColumnExpression(readerParam, prop)))); // Инициализация  свойств

        return Expression.Lambda<Func<IDataReader, T>>(memberInitExpression, readerParam)
            .Compile(); // создание делегата
    }

    /// <summary>
    /// Заполняет свойство объекта
    /// </summary>
    /// <param name="reader">DataReader </param>
    /// <param name="propertyInfo"> Информация о свойстве объекта</param>
    /// <returns>Возвращает Expression, который заполняет объект</returns>
    /// <exception cref="InvalidOperationException"></exception>
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