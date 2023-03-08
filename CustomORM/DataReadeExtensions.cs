using System.Data;

namespace CustomORM;

static class DataReadeExtensions
{
    private static bool TryGetOrdinal(this IDataReader reader, string columnName, out int order)
    {
        order = -1;
        for (var i = 0; i < reader.FieldCount; i++)
        {
            if (reader.GetName(i) == columnName)
            {
                order = i;
                return true;
            }
        }

        return false;
    }

    public static string TryGetString(this IDataReader reader, string columnName)
    {
        return reader.TryGetOrdinal(columnName, out var order) ? reader.GetString(order) : default!;
    }

    public static int TryGetInt32(this IDataReader reader, string columnName)
    {
        return reader.TryGetOrdinal(columnName, out var order) ? reader.GetInt32(order) : default!;
    }
}