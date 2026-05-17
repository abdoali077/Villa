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
        private const string KeyRegistry = "cache_key_registry";

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
            RemoveFromRegistry(key);
        }

        public void RemoveByPrefix(string prefix)
        {
            var keys = GetRegistry();
            if (keys == null || !keys.Any())
                return;

            var matchedKeys = keys.Where(k => k.StartsWith(prefix, StringComparison.OrdinalIgnoreCase)).ToList();

            foreach (var key in matchedKeys)
            {
                _cashe.Remove(key);
                keys.Remove(key);
            }

            SetRegistry(keys);
        }

        public void Set<T>(string key, T value, TimeSpan expiration)
        {
            var option = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(expiration);
            _cashe.Set(key, value, option);
            AddToRegistry(key);
        }

        private HashSet<string>? GetRegistry()
        {
            _cashe.TryGetValue(KeyRegistry, out HashSet<string>? registeredKeys);
            return registeredKeys;
        }

        private void SetRegistry(HashSet<string>? keys)
        {
            if (keys == null)
            {
                _cashe.Remove(KeyRegistry);
                return;
            }

            _cashe.Set(KeyRegistry, keys, TimeSpan.FromHours(2));
        }

        private void AddToRegistry(string key)
        {
            var keys = GetRegistry() ?? new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            keys.Add(key);
            SetRegistry(keys);
        }

        private void RemoveFromRegistry(string key)
        {
            var keys = GetRegistry();
            if (keys == null)
                return;

            if (keys.Remove(key))
                SetRegistry(keys);
        }
    }
}