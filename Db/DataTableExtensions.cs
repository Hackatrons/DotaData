using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Reflection;

namespace DotaData.Db;

internal static class DataTableExtensions
{
    /// <summary>
    /// Copies the set of values to a database table.
    /// Truncates the existing data before copying.
    /// </summary>
    public static async Task BulkLoad<T>(this SqlConnection connection, IEnumerable<T> values, string tableName, SqlTransaction transaction, CancellationToken cancellationToken = new())
    {
        await connection.ExecuteAsync($"truncate table {tableName}", transaction: transaction);

        var bulkCopy = new SqlBulkCopy(connection, SqlBulkCopyOptions.TableLock, transaction)
        {
            DestinationTableName = tableName
        };

        // must specify the column mappings
        // as by default it uses ordinal positions which may differ between the sql table and the c# type
        bulkCopy.LoadColumnMappings<T>();

        var dt = values.ToDataTable();
        await bulkCopy.WriteToServerAsync(dt, cancellationToken);
    }

    static DataTable ToDataTable<T>(this IEnumerable<T> items)
    {
        var dt = new DataTable(typeof(T).Name);
        var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

        foreach (var prop in properties)
        {
            dt.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
        }

        foreach (var item in items)
        {
            // https://stackoverflow.com/questions/6651809/sqlbulkcopy-insert-with-identity-column
            // skip the first column if the table has an auto generated id
            var values = properties.Select(x => x.GetValue(item, null)).ToArray();
            dt.Rows.Add(values);
        }

        return dt;
    }

    static void LoadColumnMappings<T>(this SqlBulkCopy copy)
    {
        var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

        foreach (var prop in properties)
        {
            copy.ColumnMappings.Add(prop.Name, prop.Name);
        }
    }
}