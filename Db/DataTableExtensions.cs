using Microsoft.Data.SqlClient;
using System.Data;
using System.Reflection;

namespace DotaData.Db
{
    internal static class DataTableExtensions
    {
        public static DataTable ToDataTable<T>(this IEnumerable<T> items)
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

        public static void LoadColumnMappings<T>(this SqlBulkCopy copy)
        {
            var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var prop in properties)
            {
                copy.ColumnMappings.Add(prop.Name, prop.Name);
            }
        }
    }
}
