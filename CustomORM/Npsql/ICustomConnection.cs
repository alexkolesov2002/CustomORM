namespace CustomORM.Npsql;

public interface ICustomConnection
{
    ICustomCommand CreateCommand(FormattableString commandText);
}