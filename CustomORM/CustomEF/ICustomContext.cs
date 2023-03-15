namespace CustomORM.CustomEF;

public interface ICustomContext
{
    string ResolveTableName(Type EntityType);
    TResult QueryAsync<TResult>(FormattableString sql);
}