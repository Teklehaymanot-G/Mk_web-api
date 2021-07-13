using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;

namespace Mk_WebApi2.Controllers
{
    public class ImportFunctions
    {
        public static int SafeGetInt(DbDataReader reader, string col)
        {
            if (reader is null)
                return -1;
            return reader.IsDBNull(col) ? -1 : reader.GetInt32(col);
        }

        public static string SafeGetString(DbDataReader reader, string col)
        {
            if (reader is null)
                return "";
            return reader.IsDBNull(col) ? "" : reader.GetString(col);
        }

        public static IEnumerable<Dictionary<string, object>> ConvertToDictionary(DbDataReader reader)
        {
            var columns = new List<string>();
            var rows = new List<Dictionary<string, object>>();

            for (var i = 0; i < reader.FieldCount; i++)
            {
                columns.Add(reader.GetName(i));
            }
            while (reader.Read())
            {
                rows.Add(columns.ToDictionary(column => column, column => reader.IsDBNull(column) ? null : reader[column]));
            }

            return rows;
        }
    }
}
