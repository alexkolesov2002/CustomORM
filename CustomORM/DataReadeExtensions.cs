using System.Data;

namespace CustomORM;

static class DataReadeExtensions
{
    /// <summary>
    /// Возвращает номер столбца в таблице
    /// </summary>
    /// <param name="reader">Database reader</param>
    /// <param name="columnName"></param>
    /// <param name="order">Номер столбца</param>
    /// <returns>Если такой столбец есть в БД true, иначе false</returns>
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
        return reader.TryGetOrdinal(columnName, out var order) ? reader.GetString(order) : default!; // получает  строку
    }

    public static int TryGetInt32(this IDataReader reader, string columnName)
    {
        return reader.TryGetOrdinal(columnName, out var order) ? reader.GetInt32(order) : default!; // получает  число
    }
}