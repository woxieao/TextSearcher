using System;
using System.Runtime.Caching;
using Giqci.Models.Dict;
using Giqci.Repositories;

namespace Giqci.PublicWeb.Services
{
    public class CacheService : ICacheService
    {
        private readonly IDictionaryRepository _repo;
        private readonly CacheItemPolicy _policy;

        public CacheService(IDictionaryRepository repo)
        {
            _repo = repo;
            _policy = new CacheItemPolicy {  SlidingExpiration = new TimeSpan(0, 10, 0) };
        }

        public Country[] GetCountries()
        {
            return GetCache("COUNTRIES", () => _repo.GetCountryDictionary());
        }

        public HSCode[] GetCommonHSCodes()
        {
            return GetCache("HSCODES", () => _repo.GetCommonHSCodes());
        }

        public Port[] GetDestPorts()
        {
            return GetCache("DESTPORTS", () => _repo.GetDestPorts());
        }

        public Port[] GetLoadingPorts()
        {
            return GetCache("LOADINGPORTS", () => _repo.GetLoadingPorts());
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
        Country[] GetCountries();

        HSCode[] GetCommonHSCodes();

        Port[] GetDestPorts();

        Port[] GetLoadingPorts();
    }
}