using System;
using System.Collections.Generic;
using System.Web.Caching;
using Giqci.Entities.Core;

using System.Runtime.Caching;
using Giqci.Repositories;

namespace Giqci.PublicWeb.Services
{
    public class CacheService : ICacheService
    {
        private readonly IGiqciRepository _repo;
        private readonly CacheItemPolicy _policy;

        public CacheService(IGiqciRepository repo)
        {
            _repo = repo;
            _policy = new CacheItemPolicy { SlidingExpiration = new TimeSpan(0, 2, 0) };
        }

        public KeyValuePair<string, string>[] GetCountries()
        {
            return GetCache("COUNTRIES", () => _repo.GetCountryDictionary());
        }

        private T GetCache<T>(string key, Func<T> func) where T : class
        {
            var value = MemoryCache.Default.Get(key) as T;
            if (value == null)
            {
                value = func();
                MemoryCache.Default.Add(key, value, _policy);
            }
            return value;
        }
    }

    public interface ICacheService
    {
        KeyValuePair<string, string>[] GetCountries();
    }
}