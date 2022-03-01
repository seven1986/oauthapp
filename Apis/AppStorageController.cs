using OAuthApp.ApiModels.StorageController;
using OAuthApp.Data;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Swashbuckle.AspNetCore.Annotations;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.Data.Sqlite;

namespace OAuthApp.Apis
{
    /// <summary>
    /// 应用数据库
    /// </summary>
    [SwaggerTag("应用数据库")]
    public class AppStorageController : BaseController
    {
        #region services
        private readonly AppDbContext _context;
        #endregion

        #region construct
        public AppStorageController(AppDbContext context)
        {
            _context = context;
        }
        #endregion

        string StorageID
        {
            get
            {
                return "_AppStorage_" + Request.RouteValues["appId"].ToString();
            }
        }

        #region 创建数据库
        //        const string _sql_create = @"IF NOT EXISTS (SELECT [name] FROM sys.tables WHERE [name] = N'{0}')
        //BEGIN 
        //CREATE TABLE {0}
        //(
        //    ID bigint IDENTITY(1,1) NOT NULL,
        //    DocumentKey nvarchar(200),
        //    Content nvarchar(max),
        //	LastChange ROWVERSION NOT NULL
        //);
        //END

        //SELECT 
        //--t.Name AS TableName,
        //p.rows AS RowCounts, 
        //CAST(ROUND((SUM(a.used_pages) / 128.00), 2) AS NUMERIC(36, 2)) AS Used_MB, 
        //CAST(ROUND((SUM(a.total_pages) - SUM(a.used_pages)) / 128.00, 2) AS NUMERIC(36, 2)) AS Unused_MB, 
        //CAST(ROUND((SUM(a.total_pages) / 128.00), 2) AS NUMERIC(36, 2)) AS Total_MB 
        //FROM sys.tables t 
        //INNER JOIN sys.indexes i ON t.OBJECT_ID = i.object_id 
        //INNER JOIN sys.partitions p ON i.object_id = p.OBJECT_ID AND i.index_id = p.index_id 
        //INNER JOIN sys.allocation_units a ON p.partition_id = a.container_id 
        //INNER JOIN sys.schemas s ON t.schema_id = s.schema_id 
        //WHERE t.Name = N'{0}'
        //GROUP BY t.Name, s.Name, p.Rows 
        //ORDER BY s.Name, t.Name 
        //";

        /// <summary>
        /// 创建数据库
        /// </summary>
        /// <param name="appID"></param>
        /// <returns></returns>
        [HttpPost("{appId}")]
        [SwaggerOperation(OperationId = "AppStorageCreate")]
        public IActionResult Create()
        {
            var tName = _context.QueryFirstOrDefault<string>(
                "select name from sqlite_master WHERE TYPE = 'table' AND tbl_name = '" + StorageID + "'");

            if (string.IsNullOrWhiteSpace(tName))
            {
                _context.Execute(@"CREATE TABLE " + StorageID
                    + " (ID INTEGER PRIMARY KEY NOT NULL,DocumentKey TEXT,Content TEXT,LastChange TEXT)");
            }

            return Ok(true);

            //var result = _context.Query<CreateResponse>(string.Format(_sql_create, StorageID))
            //  .FirstOrDefault();

            //return OK(result);
        }
        #endregion

        #region 获取数据表
        /// <summary>
        /// 获取数据表
        /// </summary>
        /// <param name="appID"></param>
        /// <returns></returns>
        [HttpGet("{appId}/Tables")]
        [SwaggerOperation(OperationId = "AppStorageTables")]
        public IActionResult Tables()
        {
            var cmd = @$"SELECT DocumentKey as Name, COUNT(1) as RowsCount FROM {StorageID} GROUP BY DocumentKey";

            var result = _context.Query<StorageTableResponse>(cmd).ToList();

            return OK(result);
        }
        #endregion

        #region 查询数据
        /// <summary>
        /// 查询数据
        /// </summary>
        /// <param name="tName">表名称</param>
        /// <param name="filter">json格式</param>
        /// <param name="take">默认为10</param>
        /// <param name="skip">默认为0</param>
        /// <param name="sort">json格式</param>
        /// <returns></returns>
        [HttpGet("{appId}/Query")]
        [SwaggerOperation(OperationId = "AppStorageQuery")]
        public IActionResult Query([Required][FromQuery] string tName,
            [FromQuery] string filter,
            [FromQuery] string sort,
            [FromQuery] int take = 10,
            [FromQuery] int skip = 0)
        {
            var (whereCondition, whereConditionParams,
                whereConditionParams2, orderBy) = QueryBuilder(filter, sort);

            whereConditionParams.Add(new SqliteParameter("@DocumentKey", tName));
            whereConditionParams2.Add(new SqliteParameter("@DocumentKey", tName));

            var data = new JArray();

            _context.ExecuteReader(
                @$"SELECT ID,Content FROM {StorageID} WHERE DocumentKey = @DocumentKey {whereCondition} " +
             orderBy +
                @$" LIMIT {take} OFFSET {skip} ",
                (reader) =>
                {
                    while (reader.Read())
                    {
                        data.Add(JToken.FromObject(new
                        {
                            id = long.Parse(reader["ID"].ToString()),
                            content = JObject.Parse(reader["Content"].ToString())
                        }));
                    }
                },
                whereConditionParams.ToArray());

            var total = 0;
            
            _context.ExecuteReader(
                $"SELECT COUNT(1) AS Total FROM {StorageID} WHERE DocumentKey = @DocumentKey {whereCondition}",
                reader => {
                    while (reader.Read())
                    {
                        total = int.Parse(reader["Total"].ToString());
                    }
                },
                whereConditionParams2.ToArray());

            return OK(new
            {
                total,
                data,
                take,
                skip,
            });
        }
        #endregion

        #region 数据详情
        /// <summary>
        /// 数据详情
        /// </summary>
        /// <param name="id">数据ID</param>
        /// <returns></returns>
        [HttpGet("{appId}/Detail")]
        [SwaggerOperation(OperationId = "AppStorageDetail")]
        public IActionResult Detail([Required][FromQuery] long id)
        {
            var sqlCmd = $"SELECT Content FROM {StorageID} WHERE ID = {id}";

            var result = _context.QueryFirstOrDefault<string>(sqlCmd);

            return OK(JObject.Parse(result));
        }
        #endregion

        #region 添加数据
        /// <summary>
        /// 添加数据
        /// </summary>
        /// <param name="tName">表名称（小写字母加数字,1-50位）</param>
        /// <param name="value">json格式</param>
        /// <returns></returns>
        [HttpPost("{appId}/Post")]
        [SwaggerOperation(OperationId = "AppStoragePost")]
        public IActionResult Post([Required][FromQuery][RegularExpression("[a-z0-9]{1,50}")] string tName,
            [FromBody] JObject value)
        {
            var sqlCmd = $"INSERT INTO {StorageID} (DocumentKey,Content) VALUES(@tName,@value)";

            _context.Execute(sqlCmd,
                new
                {
                    tName,
                    value = value.ToString()
                });

            return OK(true);
        }
        #endregion

        #region 删除数据
        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="id">数据ID</param>
        /// <returns></returns>
        [HttpDelete("{appId}/Delete")]
        [SwaggerOperation(OperationId = "AppStorageDelete")]
        public IActionResult Delete([Required][FromQuery] long id)
        {
            var sqlCmd = $"DELETE FROM {StorageID} WHERE ID = {id} ";

            _context.Execute(sqlCmd);

            return OK(true);
        }
        #endregion

        #region 更新数据
        /// <summary>
        /// 更新数据
        /// </summary>
        /// <param name="id">数据ID</param>
        /// <param name="value">json格式</param>
        /// <returns></returns>
        [HttpPut("{appId}/Put")]
        [SwaggerOperation(OperationId = "AppStoragePut")]
        public IActionResult Put([Required][FromQuery] long id,
            [Required][FromBody] JObject value)
        {
            var sqlCmd = $@"UPDATE {StorageID} SET Content=@Content WHERE ID={id}";

            _context.Execute(sqlCmd,
                new { Content = value.ToString() });

            return OK(true);
        }
        #endregion

        static (string, List<SqliteParameter>, List<SqliteParameter>, string) QueryBuilder(string filter, string sort)
        {
            var whereCondition = string.Empty;
            var whereConditionParams = new List<SqliteParameter>();
            var whereConditionParams2 = new List<SqliteParameter>();
            var orderBy = " ORDER BY ID DESC ";

            if (!string.IsNullOrWhiteSpace(filter))
            {
                var Properties = JObject.Parse(filter).Properties().ToList();

                Properties.ForEach(p =>
                {
                    whereCondition += $" AND JSON_EXTRACT(Content,'$.{p.Name}') = @{p.Name}";
                    whereConditionParams.Add(new SqliteParameter($"@{p.Name}", p.Value.ToString()));
                    whereConditionParams2.Add(new SqliteParameter($"@{p.Name}", p.Value.ToString()));
                });
            }

            if (!string.IsNullOrWhiteSpace(sort))
            {
                var sortProperty = JObject.Parse(sort).Properties().FirstOrDefault();

                orderBy = @$" ORDER BY JSON_EXTRACT(Content,'$.{sortProperty.Name}') ";

                if (sortProperty.Value.ToString().Equals("False"))
                {
                    orderBy += " DESC ";
                }
                else
                {
                    orderBy += " ASC ";
                }
            }

            return (whereCondition, whereConditionParams, whereConditionParams2, orderBy);
        }
    }
}
