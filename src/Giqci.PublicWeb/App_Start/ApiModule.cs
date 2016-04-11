using System.Net.Http;
using System.Web.Configuration;
using Autofac;
using Autofac.Core;
using Giqci.ApiProxy;
using Giqci.ApiProxy.App;
using Giqci.ApiProxy.Logging;
using Giqci.ApiProxy.Services;
using Giqci.Interfaces;
using Giqci.PublicWeb.Services;

namespace Giqci.PublicWeb
{
    public class ApiModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            var dictApiUrl = WebConfigurationManager.AppSettings["dictApiUrl"];
            var prodApiUrl = WebConfigurationManager.AppSettings["prodApiUrl"];
            var custApiUrl = WebConfigurationManager.AppSettings["custApiUrl"];
            var logApiUrl = WebConfigurationManager.AppSettings["logApiUrl"];
            var appApiUrl = WebConfigurationManager.AppSettings["appApiUrl"];

            builder.Register(x => RestClientFactory.Create(dictApiUrl, false))
                .Keyed<HttpClient>(ApiType.Dict)
                .SingleInstance();
            builder.Register(x => RestClientFactory.Create(prodApiUrl, false))
                .Keyed<HttpClient>(ApiType.Products)
                .SingleInstance();
            builder.Register(x => RestClientFactory.Create(custApiUrl, false))
                .Keyed<HttpClient>(ApiType.Customers)
                .SingleInstance();
            builder.Register(x => RestClientFactory.Create(logApiUrl, false))
                .Keyed<HttpClient>(ApiType.Log)
                .SingleInstance();
            builder.Register(x => RestClientFactory.Create(appApiUrl, false))
                .Keyed<HttpClient>(ApiType.AppApiUrl)
                .SingleInstance();

            //builder.RegisterType<MerchantApplicationApiProxy>()
            //    .As<IMerchantApplicationApiProxy>()
            //    .InstancePerDependency();
            //builder.RegisterType<CertificateApiProxy>().As<ICertificateApiProxy>().InstancePerDependency();

            builder.RegisterType<ProductApiProxy>()
                .As<IProductApiProxy>()
                .WithParameter(ResolvedParameter.ForKeyed<HttpClient>(ApiType.Products))
                .InstancePerDependency();
            builder.RegisterType<MerchantApiProxy>()
                .As<IMerchantApiProxy>()
                .WithParameter(ResolvedParameter.ForKeyed<HttpClient>(ApiType.Customers))
                .InstancePerDependency();
            builder.RegisterType<MerchantProductApiProxy>()
                .As<IMerchantProductApiProxy>()
                .WithParameter(ResolvedParameter.ForKeyed<HttpClient>(ApiType.Customers))
                .InstancePerDependency();
            builder.RegisterType<ApiUserApiProxy>()
                .As<IApiUserApiProxy>()
                .WithParameter(ResolvedParameter.ForKeyed<HttpClient>(ApiType.Customers))
                .InstancePerDependency();
            //todo 
            builder.RegisterType<MerchantApplicationApiProxy>()
                .As<IMerchantApplicationApiProxy>()
                .WithParameter(ResolvedParameter.ForKeyed<HttpClient>(ApiType.AppApiUrl))
                .InstancePerDependency();
            builder.RegisterType<ApplicationViewModelApiProxy>()
                .As<IApplicationViewModelApiProxy>()
                .WithParameter(ResolvedParameter.ForKeyed<HttpClient>(ApiType.AppApiUrl))
                .InstancePerDependency();
            builder.RegisterType<CertificateApiProxy>()
                .As<ICertificateApiProxy>()
                .WithParameter(ResolvedParameter.ForKeyed<HttpClient>(ApiType.AppApiUrl))
                .InstancePerDependency();

            builder.RegisterType<CachedDictService>()
                .As<IDictService>()
                .WithParameter(ResolvedParameter.ForKeyed<HttpClient>(ApiType.Dict))
                .InstancePerDependency();
            builder.RegisterType<AuthService>().As<IAuthService>().InstancePerDependency();
            builder.RegisterType<LoggingApiProxy>()
                .As<ILoggingApiProxy>()
                .WithParameter(ResolvedParameter.ForKeyed<HttpClient>(ApiType.Log))
                .InstancePerDependency();
        }

        private enum ApiType
        {
            Dict,
            Products,
            Customers,
            Log,
            AppApiUrl
        }
    }
}