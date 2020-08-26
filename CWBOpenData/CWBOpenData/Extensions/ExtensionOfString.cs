using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CWBOpenData.Extensions
{
    /// <summary>
    /// string的擴充方法
    /// </summary>
    public static class ExtensionOfString
    {
        public static DateTime? TryToDateTime(this string str)
        {
            DateTime date;
            if (DateTime.TryParse(str, out date))
                return date;

            return null;
        }

        public static Nullable<T> TryToEnum<T>(this string str) where T : struct
        {
            T result = default(T);
            if (Enum.TryParse(str, true, out result))
            {
                return result;
            }
            return null;
        }
    }
}
