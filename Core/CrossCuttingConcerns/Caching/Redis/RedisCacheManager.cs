using System;
using System.Collections.Generic;
using System.Text;
using Core.Utilities.IoC;
using Microsoft.Extensions.Configuration;
using ServiceStack.Redis;

namespace Core.CrossCuttingConcerns.Caching.Redis
{
    public class RedisCacheManager : ICacheManager
    {
        private readonly RedisEndpoint _redisEndpoint;

        private void RedisInvoker(Action<RedisClient> redisAction)
        {
            if (_redisEndpoint == null)
            {
                throw  new NullReferenceException();
            }

            using (var client = new RedisClient(_redisEndpoint))
            {
                redisAction.Invoke(client);
            }
        }

        public RedisCacheManager()
        {
            var configuration = (IConfiguration)ServiceTool.ServiceProvider.GetService(typeof(IConfiguration));

            var redisConnectionInfo = configuration.GetValue<string>("RedisHostInformation").Split(':');

            if (redisConnectionInfo.Length != 2 || int.TryParse(redisConnectionInfo[1], out var port) == false || port == 0)
            {
                throw new NullReferenceException();
            }

            _redisEndpoint = new RedisEndpoint(redisConnectionInfo[0], port);
        }

        public T Get<T>(string key)
        {
            var result = default(T);
            RedisInvoker(x => { result = x.Get<T>(key); });
            return result;
        }

        public object Get(string key)
        {
            var result = default(object);
            RedisInvoker(x => { result = x.Get<object>(key); });
            return result;
        }

        public void Add(string key, object data, int duration)
        {
            RedisInvoker(x => x.Add(key, data, TimeSpan.FromMinutes(duration)));
        }

        public bool IsAdd(string key)
        {
            var isAdded = false;
            RedisInvoker(x => isAdded = x.ContainsKey(key));
            return isAdded;
        }

        public void Remove(string key)
        {
            RedisInvoker(x => x.Remove(key));
        }

        public void RemoveByPattern(string pattern)
        {
            RedisInvoker(x => x.RemoveByPattern(pattern));
        }

        public void Clear()
        {
            RedisInvoker(x => x.FlushAll());
        }
    }
}