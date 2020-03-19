using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Web;

namespace prjNewPet.Models
{
    public static class IEnumerableExtensions
    {
       

        public static DataTable ToDataTable<T>(this IEnumerable<T> list)
        {
            DataTable dt = new DataTable();
            
            PropertyInfo[] props = null;

            bool schemaIsBuild = false;

            foreach (object item in list)
            {
                if (!schemaIsBuild)
                {
                    props = item.GetType().GetProperties();
                    foreach (var pi in props)
                    {
                        dt.Columns.Add(new DataColumn(pi.Name, Nullable.GetUnderlyingType( pi.PropertyType) ?? pi.PropertyType  ));//??pi.PropertyType
                    }

                    schemaIsBuild = true;
                }

                var row = dt.NewRow();
                foreach (var pi in props)
                {
                    row[pi.Name] = pi.GetValue(item, null )??DBNull.Value;
                }

                dt.Rows.Add(row);
            }

            dt.AcceptChanges();
            return dt;
        }
    }
}