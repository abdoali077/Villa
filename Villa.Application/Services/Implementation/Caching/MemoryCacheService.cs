using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Villla.Application.Services.Interface.Cashing;

namespace Villla.Application.Caching
{
    public class MemoryCacheService : ICacheService
    {
        private readonly IMemoryCache _cashe;
        public MemoryCacheService(IMemoryCache cashe)
        {
            _cashe = cashe;
        }

        public T? Get<T>(string key)
        {
            _cashe.TryGetValue(key, out T? value);
            return value;
        }

        public void Remove(string key)
        {
            _cashe.Remove(key);
        }

        public void Set<T>(string key, T value, TimeSpan expiration)
        {
            var option = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(expiration);
            _cashe.Set(key, value, option);
        }
    }
}