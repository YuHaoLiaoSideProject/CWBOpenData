using CWBOpenData.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CWBOpenData.Utility
{
    public static class SQLUtility
    {

        public static string GetDeleteCommand<T>()
        {
            return GetDeleteCommand<T>(GetTableName<T>());
        }

        public static string GetDeleteCommand<T>(string tableName)
        {
            string sqlScript = $@"
                            DELETE FROM {tableName}
                            WHERE {GetKeyConditionScript<T>()}
                            ";
            return sqlScript;
        }

        public static string GetInsertCommand<T>()
        {
            return GetInsertCommand<T>(null);
        }

        public static string GetInsertCommand<T>(List<string> ignoreFields)
        {
            string tablename = GetTableName<T>();
            if (string.IsNullOrEmpty(tablename))
            {
                throw new Exception("No Table attribute was found.");
            }

            var query = GetInsertCommand<T>(tablename, ignoreFields);
            return query;
        }
        /// <summary>
        /// 取得單筆更新SQL
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tableName"></param>
        /// <returns>先只做單 Key 多 Key 之後要用到再補</returns>
        public static string GetUpdateCommand<T>(string tableName)
        {
            List<string> keys = GetKeyName<T>();
            //不包含 Key
            List<string> allFieldNames =
                typeof(T).GetProperties().Select(p => p.Name).Where(k => keys.Contains(k) == false).ToList();

            string values = string.Join(",", allFieldNames.Select(x => "[" + x + "] = " + "@" + x).ToArray());

            return string.Format("Update {0} set {1} where {2} = @{2}", tableName, values, keys.First());
        }

        public static string GetInsertCommandByIgnoreId<T>(string tableName)
        {
            return GetInsertCommand<T>(tableName, GetAutoKeyName<T>());
        }

        public static string GetInsertOrUpdateCommandByIgnoreId<T>(string tableName)
        {
            var fields = GetFields<T>(null, true);
            var fieldPatterns = GetFields<T>(null, false);

            var fieldsStr = string.Join(",", fields.Select(fieldName => $"@{fieldName} AS {fieldName}"));

            var joinStr = string.Join(" AND ", GetKeyName<T>().Select(fieldName => $"T.{fieldName} = S.{fieldName}"));

            var updateStr = string.Join(",", fieldPatterns.Select(fieldName => $"T.{fieldName} = S.{fieldName}"));

            string sql = $@"
MERGE INTO {tableName} as T
USING (SELECT {fieldsStr}) AS S ON {joinStr}
WHEN MATCHED THEN
    UPDATE 
	SET 
		{updateStr}
WHEN NOT MATCHED THEN
    INSERT({string.Join(",", fieldPatterns)}) 
    VALUES({string.Join(",", fieldPatterns.Select(e => $"S.{e}"))});";
            return sql;
        }


        public static string GetInsertCommand<T>(string tableName, List<string> ignoreFields = null)
        {
            var query = string.Format("INSERT INTO {0} ({1}) VALUES({2})",
                tableName,
                GetFieldList<T>(ignoreFields),
                GetFieldPatternList<T>(ignoreFields));

            return query;
        }


        public static string ConcatReturnIdentityCommand(string sql)
        {
            return sql + ";select @@IDENTITY";
        }


        public static string GetTableName<T>()
        {
            var tableAttr = typeof(T).Name;
            return tableAttr;
        }


        public static string ReplaceToTotalCountCommand(string sql)
        {
            if (sql.Contains("select") && sql.Contains("from"))
                throw new Exception("Please use upper case.");

            return System.Text.RegularExpressions.Regex.Replace(sql, "SELECT(.*)FROM", string.Format("SELECT Count(*) FROM"));
        }


        public static string ConcatPagingCommand(string sql, string sortColumn, string descOrder = "desc")
        {
            sql += @" ORDER BY {0} {1} 
                     OFFSET @skipRows Rows 
                     FETCH Next @count Rows only";

            string descOrNot = descOrder == "desc" ? "desc" : "";
            sql = string.Format(sql, sortColumn, descOrNot);
            return sql;
        }


        private static string GetFieldList<T>(List<string> ignoreFields)
        {
            var sb = new StringBuilder();

            foreach (var item in GetFields<T>(ignoreFields, true))
            {
                string columnName = item;
                //有些欄位命名一定要用符號刮起來
                if (columnName == "Date" ||
                   columnName == "Grant" ||
                   columnName == "Status")
                {
                    columnName = string.Format("[{0}]", columnName);
                }

                sb.Append(columnName + ",");
            }

            var query = sb.ToString();
            return query.Remove(query.Length - 1);
        }

        private static List<string> GetFields<T>(List<string> ignoreFields, bool isContainsAutoKey)
        {
            var result = new List<string>();

            var filterProperties = GetPropertiesAndFilterIgnore<T>(ignoreFields);

            foreach (var item in filterProperties.OrderBy(x => x.Name))
            {
                if (IsICollection(item.PropertyType))
                {
                    continue;
                }

                if (IsVirtual(item))
                {
                    continue;
                }

                if (isContainsAutoKey == false && IsAutoKey(item))
                {
                    continue;
                }
                result.Add(item.Name);
            }
            return result;
        }
        private static string GetFieldPatternList<T>(List<string> ignoreFields)
        {
            var sb = new StringBuilder();

            foreach (var item in GetFields<T>(ignoreFields, false))
            {
                sb.Append("@" + item + ",");
            }

            var query = sb.ToString();
            return query.Remove(query.Length - 1);
        }

        public static List<string> GetKeyName<T>()
        {
            List<string> keyName = new List<string>();
            PropertyInfo[] properties = typeof(T).GetProperties();
            foreach (PropertyInfo property in properties)
            {
                var pkAttribute = property.GetAttribute<PrimaryKeyAttribute>();

                if (pkAttribute != null)
                {
                    keyName.Add(property.Name);
                }
            }

            return keyName;
        }
        public static List<string> GetAutoKeyName<T>()
        {
            List<string> keyName = new List<string>();
            PropertyInfo[] properties = typeof(T).GetProperties();
            foreach (PropertyInfo property in properties)
            {
                var pkAttribute = property.GetAttribute<AutoKeyAttribute>();

                if (pkAttribute != null)
                {
                    keyName.Add(property.Name);
                }
            }

            return keyName;
        }
        private static T GetAttribute<T>(this PropertyInfo property) where T : System.Attribute
        {
            var attribute = Attribute.GetCustomAttribute(property, typeof(T)) as T;

            return attribute;
        }

        private static List<PropertyInfo> GetPropertiesAndFilterIgnore<T>(List<string> ignoreFields)
        {
            var properties = typeof(T).GetProperties();

            List<PropertyInfo> filterProperties = properties.ToList();
            if (ignoreFields != null && ignoreFields.Count > 0)
            {
                filterProperties = properties
                    .Where(a => !ignoreFields.Contains(a.Name)).ToList();
            }

            return filterProperties;
        }


        private static bool IsVirtual(PropertyInfo info)
        {
            if (info.PropertyType == typeof(decimal) ||
                info.PropertyType == typeof(byte))
            {
                //某些情況下 (不明原因) 這種基本型別會判斷是true...
                return false;
            }

            bool IsVirtual = info.GetGetMethod().IsVirtual;
            return IsVirtual;
        }



        private static bool IsAutoKey(PropertyInfo info)
        {
            var attr_skip = (AutoKeyAttribute)info.GetCustomAttribute(typeof(AutoKeyAttribute), false);

            if (attr_skip != null)
                return true;

            return false;
        }


        private static bool IsICollection(Type type)
        {
            if (type == null || !type.IsGenericType)
            {
                return false;
            }

            return typeof(ICollection<>).IsAssignableFrom(type
                .GetGenericTypeDefinition());

        }

        public static string GetKeyConditionScript<T>()
        {
            return string.Join(" and ", GetKeyName<T>().Select(x => "[" + x + "] = " + "@" + x).ToArray());
        }
    }
}
