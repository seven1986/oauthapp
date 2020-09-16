using Microsoft.Extensions.Configuration;
using StackExchange.Redis;
using System;
using System.Threading.Tasks;

namespace OAuthApp.Services
{
    public class RedisService : IDisposable
    {
        string RedisConnectionString { get; set; }

        ConnectionMultiplexer _Connection;
        public ConnectionMultiplexer Connection
        {
            get
            {
                if (_Connection == null || !_Connection.IsConnected)
                {
                    _Connection = ConnectionMultiplexer.Connect(RedisConnectionString);
                }

                return _Connection;
            }
        }

        public RedisService(IConfiguration config)
        {
            RedisConnectionString = config["ConnectionStrings:RedisConnection"];
        }

        /// <summary>
        /// 根据Key获取值
        /// </summary>
        /// <param name="key">键</param>
        public async Task<string> GetAsync(string key) => await GetAsync(key, Connection.GetDatabase());
        public async Task<string> GetAsync(string key, IDatabase db)
        {
            var result = await db.StringGetAsync(key);
            return result;
        }

        /// <summary>
        /// 设置缓存key和value
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <param name="expire">过期时间</param>
        public async Task<bool> SetAsync(string key, string value, TimeSpan? expire) => await SetAsync(key, value, expire, Connection.GetDatabase());
        public async Task<bool> SetAsync(string key, string value, TimeSpan? expire, IDatabase db)
        {
            var result = await db.StringSetAsync(key, value, expire);
            return result;
        }

        /// <summary>
        /// 删除指定的Key
        ///</summary>
        ///<returns></returns>
        public async Task<bool> RemoveAsync(string key) => await RemoveAsync(key, Connection.GetDatabase());
        public async Task<bool> RemoveAsync(string key, IDatabase db)
        {
            var result = await db.KeyDeleteAsync(key);
            return result;
        }

        /// <summary>
        /// 将指定的Key的值叠加value
        /// </summary>
        /// <returns></returns>
        public async Task<long> IncrementAsync(string key, long value = 1) => await IncrementAsync(key, value, Connection.GetDatabase());
        public async Task<long> IncrementAsync(string key, long value, IDatabase db)
        {
            return await db.StringIncrementAsync(key, value);
        }

        /// <summary>
        /// 检测Key是否存在
        /// </summary>
        public async Task<bool> KeyExistsAsync(string key) => await KeyExistsAsync(key, Connection.GetDatabase());
        public async Task<bool> KeyExistsAsync(string key, IDatabase db)
        {
            var result = await db.KeyExistsAsync(key);

            return result;
        }

        public void Dispose()
        {
            if (Connection != null)
            {
                Connection.Dispose();
            }
        }
    }
}
