using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Dapper;
using System.Text;
using CWBOpenData.Utility;
using CWBOpenData.Extensions;
using System.Transactions;

namespace CWBOpenData.Repositories.BaseRepositories
{
    public class BaseRepository
    {
        public BaseRepository(string connectionString)
        {
            _connectionString = connectionString;
        }
        public string _connectionString { get; set; }
        protected void ExecuteSQL(string sql, object pairs)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Execute(sql, pairs);
            }
        }
        protected S ExecuteSQL<S>(string sql, object pairs)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                return conn.ExecuteScalar<S>(sql, pairs);
            }
        }
        /// <summary>
        /// 依條件查詢第一筆第一個欄位資料
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="_SQLScript"></param>
        /// <param name="paras"></param>
        /// <returns></returns>
        protected S GetScalarBySQLScript<S>(string _SQLScript, object param = null)
        {
            using (SqlConnection _SqlConnection = new SqlConnection(_connectionString))
            {
                return _SqlConnection.ExecuteScalar<S>(_SQLScript, param);
            }
        }

        /// <summary>
        /// 依條件查詢多筆資料(分頁用)
        /// </summary>
        /// <typeparam name="S"></typeparam>
        /// <param name="_SQLScript"></param>
        /// <param name="currentPage">目前第幾頁</param>
        /// <param name="pageSize">一頁顯示幾筆資料</param>
        /// <param name="_CommandType"></param>
        /// <param name="paras"></param>
        /// <returns></returns>
        protected List<S> GetPageingEntities<S>(string _SQLScript, int currentPage, int pageSize, object param = null)
        {
            int startRowNum = (currentPage <= 1) ? 1 : 1 + (currentPage - 1) * pageSize;
            int endRowNum = (startRowNum - 1) + pageSize;
            string paggingSQL = null;
            if (currentPage.Equals(0))
            {
                paggingSQL = @"
                            SELECT
                                *
                            FROM 
                                ( " + _SQLScript + @" ) as pageData 
                            ";
            }
            else
            {
                paggingSQL = @"
                            SELECT
                                *
                            FROM 
                                ( " + _SQLScript + @" ) as pageData
                            WHERE
                                RowNum >= " + startRowNum + @" AND RowNum <= " + endRowNum + @"
                            ";
            }
            return GetEntitiesBySQLScript<S>(paggingSQL, param);
        }
        /// <summary>
        /// 依條件查詢多筆資料
        /// </summary>
        /// <typeparam name="S"></typeparam>
        /// <param name="_SQLScript"></param>
        /// <param name="paras"></param>
        /// <returns></returns>
        protected List<S> GetEntitiesBySQLScript<S>(string _SQLScript, object param = null)
        {
            using (SqlConnection _SqlConnection = new SqlConnection(_connectionString))
            {
                return _SqlConnection.Query<S>(_SQLScript, param).ToList();
            }
        }
        protected List<S> GetListBySQL<S>(string sql, object param = null)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                return conn.Query<S>(sql, param).ToList();
            }
        }
    }
    public class BaseRepository<T> : BaseRepository where T : class
    {
        public BaseRepository(string connectionString, string tableName) : base(connectionString)
        {
            _tableName = tableName;
        }

        protected string _tableName { get; set; }

        /// <summary>
        /// 取得列表(請斟酌使用、資料量太大可能會造成系統負荷過大)
        /// </summary>
        /// <param name="qs"></param>
        /// <returns></returns>
        public List<T> GetList()
        {
            return GetListByWhereSQL(string.Empty, null);
        }
        /// <summary>
        /// 取得單筆
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public T Get<S>(S id)
        {
            string key = SQLUtility.GetKeyName<T>().First();
            var sql = $@"
                        SELECT * 
                        FROM {_tableName}
                        WHERE {SQLUtility.GetKeyConditionScript<T>()}";

            Dictionary<string, object> pairs = new Dictionary<string, object> { };

            pairs.Add(key, id);

            using (var conn = new SqlConnection(_connectionString))
            {
                return conn.Query<T>(sql, pairs).FirstOrDefault();
            }
        }

        /// <summary>
        /// 新增單筆
        /// </summary>
        public bool Create(T model)
        {
            var sql = SQLUtility.GetInsertCommandByIgnoreId<T>(_tableName);

            using (var conn = new SqlConnection(_connectionString))
            {
                return conn.Execute(sql, model) == 1;
            }
        }

        public void CreateOrUpdate(T model)
        {
            CreateOrUpdate(new List<T> { model });
        }

        public void CreateOrUpdate(List<T> models)
        {
            var sql = SQLUtility.GetInsertOrUpdateCommandByIgnoreId<T>(_tableName);

            ExecuteSQL(sql, models);
        }

        public S CreateAndResultIdentity<S>(T model)
        {
            var sql = SQLUtility.GetInsertCommandByIgnoreId<T>(_tableName);

            sql = SQLUtility.ConcatReturnIdentityCommand(sql);

            using (var conn = new SqlConnection(_connectionString))
            {
                var result = conn.ExecuteScalar<S>(sql, model);
                return result;
            }
        }
        protected bool IsExistsByWhereSQL(string whereSql, object param = null)
        {
            var keys = SQLUtility.GetKeyName<T>();
            string sql = string.Empty;
            if (keys.IsNotEmpty())
            {
                string key = keys.First();
                sql = $@"SELECT TOP 1 {key}
                            FROM [{_tableName}] WITH(NOLOCK)
                            {whereSql}";
            }
            else
            {
                sql = $@"SELECT TOP 1 *
                            FROM [{_tableName}] WITH(NOLOCK)
                            {whereSql}";
            }
            return IsExistsBySQL(sql, param);
        }
        protected bool IsExistsBySQL(string sql, object param = null)
        {
            string querySQL = $@"
IF EXISTS 
(
	{sql}
)
BEGIN
     SELECT 1
END
ELSE
     SELECT 0
";
            return GetScalarBySQLScript<bool>(querySQL, param);
        }
        /// <summary>
        /// 新增多筆
        /// </summary>
        public bool Create(List<T> models)
        {
            var sql = SQLUtility.GetInsertCommandByIgnoreId<T>(_tableName);

            using (var conn = new SqlConnection(_connectionString))
            {
                return conn.Execute(sql, models) == 1;
            }
        }
        /// <summary>
        /// 新增多筆
        /// </summary>
        public bool CreateBySqlBulkCopy(List<T> models)
        {
            var pagesize = 500000;
            var pageCount = models.Count / pagesize;
            pageCount = models.Count % pagesize == 0 ? pageCount : pageCount + 1;

            for (int page = 1; page <= pageCount; page++)
            {
                using (var tx = new TransactionScope())
                {
                    using (var conn = new SqlConnection(_connectionString))
                    {
                        conn.Open();

                        var dt = models
                            .Skip(pagesize * (page - 1))
                            .Take(pagesize)
                            .ToDataTable();

                        using (var sqlBulkCopy = new SqlBulkCopy(conn))
                        {
                            sqlBulkCopy.BatchSize = 100000;
                            sqlBulkCopy.DestinationTableName = _tableName;
                            sqlBulkCopy.WriteToServer(dt);
                        }
                    }
                    tx.Complete();
                }
            }

            return true;

        }
        public bool Update(T model)
        {
            var sql = SQLUtility.GetUpdateCommand<T>(_tableName);
            using (var conn = new SqlConnection(_connectionString))
            {
                return conn.Execute(sql, model) == 1;
            }
        }

        public void Update(List<T> models)
        {
            var sql = SQLUtility.GetUpdateCommand<T>(_tableName);
            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Execute(sql, models);
            }
        }
        /// <summary>
        /// 刪除單筆
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool Delete<S>(S id) where S : IComparable
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                var sql = SQLUtility.GetDeleteCommand<T>(_tableName);

                Dictionary<string, object> pairs = new Dictionary<string, object> { };

                pairs.Add(SQLUtility.GetKeyName<T>().First(), id);

                return conn.Execute(sql, pairs) > 0;
            }
        }
        public bool Delete<S>(IEnumerable<S> Ids) where S : IComparable
        {
            string whereSQL = $@"Where {SQLUtility.GetKeyName<T>().First()} IN @Ids";
            return DeleteByWhereSQL(whereSQL, Ids);
        }
        protected bool DeleteByWhereSQL(string whereSql, object param = null)
        {
            string sql = $@"DELETE
                            FROM [{_tableName}]
                            {whereSql}";
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                return conn.Execute(sql, param) > 0;
            }
        }

        protected T GetByWhereSQL(string whereSql, object param = null)
        {
            string querySQL = $@"SELECT * 
                                 FROM [{_tableName}] WITH(NOLOCK)
                                 {whereSql}";
            return GetListBySQL<T>(querySQL, param).FirstOrDefault();
        }
        protected int GetCountByWhereSQL(string whereSql, object param = null)
        {
            var sql = $@"SELECT COUNT(1)
                         FROM {_tableName} WITH(NOLOCK)
                         {whereSql}";

            using (var conn = new SqlConnection(_connectionString))
            {
                return conn.QueryFirstOrDefault<int>(sql, param);
            }
        }

        protected List<T> GetListByWhereSQL(string whereSql, object param = null)
        {
            string querySQL = $@"SELECT * 
                                 FROM [{_tableName}] WITH(NOLOCK)
                                 {whereSql}";
            return GetListBySQL<T>(querySQL, param);
        }
    }
}
