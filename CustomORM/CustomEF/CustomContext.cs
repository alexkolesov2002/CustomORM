using System.Reflection;

namespace CustomORM.CustomEF;

public class CustomContext : ICustomContext
{
    public CustomContext()
    {
        var sets =
            GetType().GetProperties()
                .Where(x => x.PropertyType.GetGenericTypeDefinition() == typeof(CustomDbSet<>));

        foreach (var set in sets)
        {
            set.SetValue(this, CreateSet(set.PropertyType.GetGenericArguments()[0]));
        }
    }

    private object? CreateSet(Type setPropertyType)
    {
    
        return typeof(CustomContext).GetMethods( BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy | BindingFlags.Public)
            .Single(x=>x.Name == "CreateSetInternal")
            .MakeGenericMethod(setPropertyType)
            .Invoke(this, Array.Empty<object>());
    }

    private object CreateSetInternal<T>() => new CustomDbSet<T>(new CustomQueryProvider());
}