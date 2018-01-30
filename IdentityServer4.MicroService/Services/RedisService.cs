using System;
using System.Threading.Tasks;
using StackExchange.Redis;
using Microsoft.Extensions.Options;

namespace IdentityServer4.MicroService.Services
{
    public class RedisService
    {
        string configurationString { get; set; }

        ConnectionMultiplexer _Connection;
        public ConnectionMultiplexer Connection
        {
            get
            {
                if (_Connection == null || !_Connection.IsConnected)
                {
                    _Connection = ConnectionMultiplexer.Connect(configurationString);
                }

                return _Connection;
            }
        }

        public RedisService(IOptions<ConnectionStrings> _config)
        {
            configurationString = _config.Value.RedisConnection;
        }

        #region get
        /// <summary>
        /// 根据Key获取值
        /// </summary>
        /// <param name="key">键</param>
        /// <returns></returns>
        public async Task<string> Get(string key)
        {
            if (await KeyExists(key) == false)
            {
                return string.Empty;
            }

            var db = Connection.GetDatabase();

            return await Get(db, key);
        }
        /// <summary>
        /// 根据Key获取值
        /// </summary>
        /// <param name="db">数据库</param>
        /// <param name="key">键</param>
        /// <returns></returns>
        public async Task<string> Get(IDatabase db, string key)
        {
            try
            {
                return await db.StringGetAsync(key);
            }
            catch
            {

            }

            return string.Empty;
        }
        #endregion

        #region set
        /// <summary>
        /// 设置缓存key和value
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <param name="expire">过期时间</param>
        /// <returns></returns>
        public async Task<bool> Set(string key, string value, TimeSpan? expire)
        {
            var db = Connection.GetDatabase();

            return await Set(db, key, value, expire);
        }
        /// <summary>
        /// 设置缓存key和value
        /// </summary>
        /// <param name="db">数据库</param>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <param name="expire">过期时间</param>
        /// <returns></returns>
        public async Task<bool> Set(IDatabase db, string key, string value, TimeSpan? expire)
        {
            try
            {
                return await db.StringSetAsync(key, value, expire);
            }
            catch
            {

            }

            return false;
        }
        #endregion

        #region Remove
        /// <summary>
        /// 删除指定的Key
        ///</summary>
        ///<param name="key">键</param>
        ///<returns></returns>
        public async Task<bool> Remove(string key)
        {
            var db = Connection.GetDatabase();

            return await db.KeyDeleteAsync(key);
        }
        #endregion

        #region Increment
        /// <summary>
        /// 将指定的Key的值叠加value
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值，默认1</param>
        /// <returns></returns>
        public async Task<long> Increment(string key, long value = 1)
        {
            var db = Connection.GetDatabase();

            return await Increment(db, key, value);
        }
        /// <summary>
        /// 将指定的Key的值叠加value
        /// </summary>
        /// <param name="db">数据库</param>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public async Task<long> Increment(IDatabase db, string key, long value)
        {
            try
            {
                return await db.StringIncrementAsync(key, value);
            }
            catch
            {
            }

            return value;
        }
        #endregion

        #region KeyExists
		/// <summary>
        /// 检测Key是否存在
        /// </summary>
        /// <param name="key">键</param>
        /// <returns></returns>
        public async Task<bool> KeyExists(string key)
        {
            var db = Connection.GetDatabase();

            var result = await db.KeyExistsAsync(key);

            return result;
        } 
        #endregion
    }
}
