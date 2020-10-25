using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace CWBOpenData.Extensions
{
    public static class ExtensionOfCollections
    {
        public static DataTable ToDataTable<TResult>(this IEnumerable<TResult> inputArray) where TResult : class
        {
            //建立一個回傳用的 DataTable
            DataTable dt = new DataTable();

            //宣告一個 PropertyInfo 陣列，來接取 Type 所有的共用屬性
            PropertyInfo[] PI_List = null;

            foreach (var item in inputArray)
            {
                //判斷 DataTable 是否已經定義欄位名稱與型態
                if (dt.Columns.Count == 0)
                {
                    //取得 Type 所有的共用屬性
                    PI_List = item.GetType().GetProperties();

                    //將 List 中的 名稱 與 型別，定義 DataTable 中的欄位 名稱 與 型別
                    foreach (var item1 in PI_List)
                    {
                        dt.Columns.Add(item1.Name, item1.PropertyType);
                    }
                }

                //在 DataTable 中建立一個新的列
                DataRow dr = dt.NewRow();

                //將資料足筆新增到 DataTable 中
                foreach (var item2 in PI_List)
                {
                    dr[item2.Name] = item2.GetValue(item, null);
                }

                dt.Rows.Add(dr);
            }

            dt.AcceptChanges();

            return dt;
        }
        /// <summary>
        /// 取得中位數的值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static decimal Median<T>(
            this IEnumerable<T> source)
        {
            if (Nullable.GetUnderlyingType(typeof(T)) != null)
                source = source.Where(x => x != null);

            int count = source.Count();

            if (count == 0)
                return 0;

            source = source.OrderBy(n => n);

            int midpoint = count / 2;

            if (count % 2 == 0)
                return (Convert.ToDecimal(source.ElementAt(midpoint - 1)) + Convert.ToDecimal(source.ElementAt(midpoint))) / 2.0M;
            else
                return Convert.ToDecimal(source.ElementAt(midpoint));
        }

        public static HashSet<T> ToHashSet<T>(this IEnumerable<T> list)
        {
            if (list == null)
            {
                return null;
            }
            return new HashSet<T>(list);
        }
        public static void AddRange<T>(this HashSet<T> hashSet, IEnumerable<T> list)
        {
            foreach (var item in list)
            {
                hashSet.Add(item);
            }
        }

        public static IEnumerable<T> GetPartialByTargetRate<T>(this IEnumerable<T> list, decimal rate)
        {
            //rate 請填寫 0.01~0.99 之間的數字  
            //e.g. 假如填寫0.6 代表是取前60%的資料
            if (rate > 1 || rate <= 0)
                return list;

            decimal total = list.Count();
            int takeSize = (int)(total * rate);

            if (takeSize <= 0)
                takeSize = 1;

            return list.Take(takeSize);
        }



        /// <summary>
        /// 將單筆 int、long、string 等型別轉成 list
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="inputValue"></param>
        /// <returns></returns>
        public static List<T> ToListCollection<T>(this T inputValue) where T : IComparable
        {
            List<T> listResult = new List<T>();
            listResult.Add(inputValue);

            return listResult;
        }


        /// <summary>
        /// 將集合插入指定的筆數，但會做防呆
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="index"></param>
        /// <param name="insertItems"></param>
        public static void InsertWithCatch<T>(this List<T> list, int index, List<T> insertItems)
        {
            try
            {
                list.InsertRange(index, insertItems);
            }
            catch
            {
                int numberOfList = list.Count();
                list.InsertRange((numberOfList), insertItems);
            }
        }



        public static T GetMatchOrFirstDefault<T>(this IEnumerable<T> list, Func<T, bool> func) where T : class
        {
            if (list.IsEmptyOrNull())
                return null;

            var matchFirst = list.FirstOrDefault(func);

            if (matchFirst != null)
                return matchFirst;


            return list.FirstOrDefault();
        }




        public static string GetDictionaryOrEmptyValue<T>(this Dictionary<T, string> dic, T key)
        {
            if (dic.ContainsKey(key) == false)
                return string.Empty;

            return dic[key];
        }


        /// <summary>
        /// 若找不到指定的值，返回預設第一筆
        /// </summary>
        /// <param name="dic"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetDictionaryOrDefaultFirstValue(this Dictionary<int, string> dic, int key)
        {
            if (dic.IsEmptyOrNull())
                return string.Empty;

            if (dic.ContainsKey(key) == false)
                return dic.FirstOrDefault().Value;


            return dic[key];
        }

        /// <summary>
        /// 找出有重複的集合
        /// </summary>
        public static IList<T> FindDuplicateItems<T, TKey>(this IList<T> list, Func<T, TKey> func)
        {
            var query = list.GroupBy(func).Select(x => new
            {
                count = x.Count(),
                model = x.First()

            }).ToList();

            return query.Where(x => x.count > 1).Select(x => x.model).ToList();
        }


        /// <summary>
        /// Distanct第一筆
        /// </summary>
        public static List<T> DistinctBy<T, TKey>(this IList<T> list, Func<T, TKey> func)
        {
            if (list.IsEmptyOrNull())
                return new List<T>();


            return list.GroupBy(func).Select(x => x.First()).ToList();
        }


        /// <summary>
        /// Distanct第一筆
        /// </summary>
        public static List<T> DistinctOrderBy<T, TKey, TColumn>(this IList<T> list, Func<T, TKey> func, Func<T, TColumn> orderBy)
        {
            if (list.IsEmptyOrNull())
                return new List<T>();


            return list.GroupBy(func).Select(x => x.OrderBy(orderBy).First()).ToList();
        }


        /// <summary>
        /// Distanct 後 每組group隨機取一筆
        /// </summary>
        public static IList<T> SelectDistinctRandomOne<T, TKey>(this IList<T> list, Func<T, TKey> func)
        {
            if (list.IsEmptyOrNull())
                return new List<T>();

            return list.GroupBy(func).Select(x => x.ToList().SortByRandom().First()).ToList();
        }




        /// <summary>
        /// 取得這是哪個Type的List
        /// </summary>
        public static System.Type GetListType<T>(this IEnumerable<T> _)
        {
            return typeof(T);
        }


        /// <summary>
        /// 亂數排序List 
        /// </summary>
        public static List<T> SortByRandom<T>(this IList<T> input, int displayLength = 0)
        {
            if (input.IsEmptyOrNull())
                return new List<T>();

            Random r = new Random();

            if (displayLength > 0)
                return input.OrderBy(x => r.Next()).Take(displayLength).ToList();
            else
                return input.OrderBy(x => r.Next()).ToList();
        }

        public static int CountOrZero<T>(this IEnumerable<T> source)
        {
            return source.OrEmptyIfNull().Count();
        }
        /// <summary>
        /// 可用於 foreach , 若來源為 null 時直接回傳空陣列
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static IEnumerable<T> OrEmptyIfNull<T>(this IEnumerable<T> source)
        {
            return source ?? Enumerable.Empty<T>();
        }
        /// <summary>
        /// 判斷List是否為null或Count == 0
        /// </summary>
        public static bool IsEmptyOrNull<T>(this IEnumerable<T> source)
        {
            if (source == null)
                return true; // or throw an exception

            if (source.Any())
                return false;

            return true;
        }


        /// <summary>
        /// 判斷List是否不為空 (不為null ＆＆　筆數大於０)
        /// </summary>
        public static bool IsNotEmpty<T>(this IEnumerable<T> source)
        {
            if (source == null)
                return false; // or throw an exception

            return source.Any();
        }
    }
}
