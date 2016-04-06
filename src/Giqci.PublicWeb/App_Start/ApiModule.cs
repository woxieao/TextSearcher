using System.Net.Http;
using System.Web.Configuration;
using Autofac;
using Autofac.Core;
using Giqci.ApiProxy;
using Giqci.ApiProxy.Services;
using Giqci.Interfaces;
using Giqci.PublicWeb.Services;
using Giqci.Repositories;

namespace Giqci.PublicWeb
{
    public class ApiModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            var connStr = WebConfigurationManager.AppSettings["connStr"];
            var dictApiUrl = WebConfigurationManager.AppSettings["dictApiUrl"];
            var prodApiUrl = WebConfigurationManager.AppSettings["prodApiUrl"];
            var custApiUrl = WebConfigurationManager.AppSettings["custApiUrl"];

            builder.Register(x => RestClientFactory.Create(dictApiUrl, false))
                .Keyed<HttpClient>(ApiType.Dict)
                .SingleInstance();
            builder.Register(x => RestClientFactory.Create(prodApiUrl, false))
                .Keyed<HttpClient>(ApiType.Products)
                .SingleInstance();
            builder.Register(x => RestClientFactory.Create(custApiUrl, false))
                .Keyed<HttpClient>(ApiType.Customers)
                .SingleInstance();

            builder.Register(context => new DatabaseSetting(connStr)).SingleInstance();
            builder.RegisterType<LoggerRepository>().As<ILoggerRepository>().InstancePerDependency();
            builder.RegisterType<ApplicationRepository>().As<IApplicationRepository>().InstancePerDependency();
            builder.RegisterType<CertificateRepository>().As<ICertificateRepository>().InstancePerDependency();
            builder.RegisterType<ContainerInfoRepository>().As<IContainerInfoRepository>().InstancePerDependency();
            builder.RegisterType<ExampleCertRepository>().As<IExampleCertRepository>().InstancePerDependency();

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

            builder.RegisterType<CachedDictService>()
                .As<IDictService>()
                .WithParameter(ResolvedParameter.ForKeyed<HttpClient>(ApiType.Dict))
                .InstancePerDependency();
            builder.RegisterType<AuthService>().As<IAuthService>().InstancePerDependency();
        }

        private enum ApiType
        {
            Dict,
            Products,
            Customers,
        }
    }
}