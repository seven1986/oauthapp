using System;
using System.Linq;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using IdentityServer4.MicroService.Models.Apis.Common;

namespace IdentityServer4.MicroService.Services
{
    public class PagingService<T> where T :  class, new()
    {
        static readonly ConcurrentDictionary<string, List<string>> Columns =
            new ConcurrentDictionary<string, List<string>>();

        string TableName { get; set; }

        IPagingRequest value { get; set; }

        /// <summary>
        /// 自定义筛选
        /// </summary>
        public Action<List<string>, List<SqlParameter>> where { get; set; }

        /// <summary>
        /// 添加CreateDate字段的查询条件
        /// </summary>
        public bool AutoFilter_CreateDate { get; set; } = true;

        /// <summary>
        /// 默认的可排序字段只有当前表，这里可以设置额外的字段组合比如：A字段 - B字段
        /// </summary>
        public List<string> OrderByFieldsExtension { get; set; }

        DbContext db { get; set; }

        public PagingService(DbContext _db, IPagingRequest value, string tableName = "")
        {
            TableName = string.IsNullOrWhiteSpace(tableName) ? typeof(T).Name : tableName;

            db = _db;

            #region 缓存每张表的列，用于检测orderby的合法性
            if (!Columns.ContainsKey(tableName))
            {
                Columns[tableName] = typeof(T).GetProperties().Select(x => x.Name).ToList();
            }
            #endregion

            this.value = value;
        }

        /// <summary>
        /// 执行分页查询
        /// </summary>
        /// <param name="sql">分页数据查询语句</param>
        /// <param name="sql_dataCount">数据总数查询语句</param>
        /// <param name="propConverter">返回实体属性转换委托</param>
        /// <returns></returns>
        public async Task<PagingResult<T>> ExcuteAsync(
            string sql = @"SELECT * FROM {0} {1} ORDER BY {2} OFFSET {3} ROW FETCH NEXT {4} ROW ONLY",
            string sql_dataCount = "SELECT COUNT(1) FROM {0} {1}",
            Func<PropertyInfo,Object, Object> propConverter = null)
        {
            #region filters
            #region custom
            var WhereBuilder = new List<string>();
            var WhereParameters = new List<SqlParameter>();
            where?.Invoke(WhereBuilder, WhereParameters);
            #endregion

            #region createDate
            if (AutoFilter_CreateDate)
            {
                #region 根据时间过滤
                #region 大于等于 开始时间 && 小于等于 结束时间
                if (value.startTime.HasValue && value.endTime.HasValue)
                {
                    WhereBuilder.Add(" CreateDate >= '" + value.startTime.Value.ToString("yyyy-MM-dd") + " 00:00:00' AND CreateDate <= '" + value.endTime.Value.ToString("yyyy-MM-dd") + " 23:59:59'");
                }
                #endregion
                #region 大于等于开始时间
                else if (value.startTime.HasValue)
                {
                    WhereBuilder.Add(" CreateDate >= '" + value.startTime.Value.ToString("yyyy-MM-dd") + " 00:00:00'");
                }
                #endregion
                #region 小于等于结束时间
                else if (value.endTime.HasValue)
                {
                    WhereBuilder.Add(" CreateDate <= '" + value.endTime.Value.ToString("yyyy-MM-dd") + " 23:59:59'");
                }
                #endregion
                #endregion
            }
            #endregion

            var Where = WhereBuilder.Count > 0 ?
             " WHERE " + string.Join(" AND ", WhereBuilder) :
             string.Empty;
            #endregion

            #region orderBy
            var OrderBy = " ID DESC ";
            if (!string.IsNullOrWhiteSpace(value.orderby) &&
                (Columns[TableName].Contains(value.orderby) ||
                 OrderByFieldsExtension.Contains(value.orderby)))
            {
                OrderBy = " " + value.orderby + " " + (!value.asc.Value ? "DESC" : "ASC");
            }
            #endregion

            sql_dataCount = string.Format(sql_dataCount, TableName, Where);
            var total = 0;

            sql = string.Format(sql, TableName, Where, OrderBy, 
                value.skip, 
                value.take);

            var entities = new List<T>();

            var entityType = typeof(T);

            using (var connection = db.Database.GetDbConnection())
            {
                if (connection.State != System.Data.ConnectionState.Open)
                {
                    connection.Open();
                }

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = sql_dataCount;
                    command.Parameters.AddRange(WhereParameters.ToArray());
                    var _total = command.ExecuteScalar();
                    if (_total != null)
                    {
                        total = int.Parse(_total.ToString());
                    }
                }

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = sql;

                    //fix error:The SqlParameter is already contained by another SqlParameterCollection.
                    var clonedParameters = new SqlParameter[WhereParameters.Count];

                    for (int i = 0, j = WhereParameters.Count; i < j; i++)
                    {
                        clonedParameters[i] = (SqlParameter)((ICloneable)WhereParameters[i]).Clone();
                    }

                    command.Parameters.AddRange(clonedParameters);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (reader.Read())
                        {
                            var item = new T();

                            for (var i = 0; i < Columns[TableName].Count; i++)
                            {
                                var PropertyName = Columns[TableName][i];

                                var PropertyValue = reader[PropertyName];

                                if (PropertyValue != DBNull.Value)
                                {
                                    var Property = entityType.GetProperty(PropertyName);

                                    try
                                    {
                                        if (propConverter != null)
                                        {
                                            var _PropertyValue = propConverter.Invoke(Property, PropertyValue);

                                            Property.SetValue(item, _PropertyValue);
                                        }

                                        else
                                        {
                                            Property.SetValue(item, PropertyValue);
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        throw ex;
                                    }
                                }
                            }

                            entities.Add(item);
                        }
                    }

                    command.Parameters.Clear();
                }
            }

            var result = new PagingResult<T>(entities,
                total, value.skip.Value, value.take.Value);

            return result;
        }
    }
}
