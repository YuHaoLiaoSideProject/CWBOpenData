using CWBOpenData.Enums;
using CWBOpenData.Helpers;
using CWBOpenData.IServices;
using System;

namespace CWBOpenData.Services
{
    public class RedisService : IRedisService
    {
        public void Test()
        {
            var db = RedisHelper.GetDatabase();
            //String
            db.StringSet("foo", 1688);

            //StringIncrement
            db = RedisHelper.GetDatabase(RedisDBNumberEnum.DB06);
            db.StringIncrement("count");

            //HashSet
            db = RedisHelper.GetDatabase(RedisDBNumberEnum.DB15);
            db.HashSet("members", 1, DateTime.Now.ToString());
            db.HashSet("members", 2, DateTime.Now.ToString());
            db.HashSet("members", 3, DateTime.Now.ToString());
            db.HashSet("members", 4, DateTime.Now.ToString());

            
            var t =db.HashGet("members", 1);
            
        }

        public (string T, string U) T()
        {
            return ("1", "2");
        }
    }
}
