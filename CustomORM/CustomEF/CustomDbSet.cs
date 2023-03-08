namespace CustomORM.CustomEF;

public class CustomDbSet<T> : CustomQueryable<T>
{
    public CustomDbSet(IQueryProvider provider) : base(provider)
    {
    }
}