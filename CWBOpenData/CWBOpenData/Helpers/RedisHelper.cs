using CWBOpenData.Enums;
using StackExchange.Redis;
using System.Collections.Generic;

namespace CWBOpenData.Helpers
{
    public static class RedisHelper
    {
        static ConnectionMultiplexer _redis = ConnectionMultiplexer.Connect("127.0.0.1:6379");
        private static Dictionary<RedisDBNumberEnum, IDatabase> _dbs = new Dictionary<RedisDBNumberEnum, IDatabase>();
        public static IDatabase GetDatabase(RedisDBNumberEnum number = RedisDBNumberEnum.DB00)
        {
            if (_dbs.ContainsKey(number) == false)
            {
                _dbs.Add(number, _redis.GetDatabase((int)number));
            }
            return _dbs[number];
        }
    }
}
