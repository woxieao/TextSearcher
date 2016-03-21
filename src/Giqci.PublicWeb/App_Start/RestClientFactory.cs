using System;
using System.Net.Cache;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace Giqci.PublicWeb
{
    public static class RestClientFactory
    {
        public static HttpClient Create(string baseUrl, bool cacheEnabled)
        {
            HttpClient client;
            if (cacheEnabled)
            {
                client = new HttpClient(new WebRequestHandler
                {
                    CachePolicy = new HttpRequestCachePolicy(HttpRequestCacheLevel.CacheIfAvailable)
                });
            }
            else
            {
                client = new HttpClient();
            }
            client.BaseAddress = new Uri(baseUrl);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            return client;
        }

        public static HttpClient Create(string baseUrl, string username, string password, bool cacheEnabled)
        {
            var client = Create(baseUrl, cacheEnabled);
            var authorization = string.Format("{0}:{1}", username, password);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.ASCII.GetBytes(authorization)));
            return client;
        }
    }
}