using System.Net.Http;
using System.Web.Configuration;
using Autofac;
using Giqci.ApiProxy;
using Giqci.ApiProxy.Services;
using Giqci.Interfaces;
using Giqci.Repositories;
using Giqci.Services;

namespace Giqci.PublicWeb
{
    public class ApiModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            var connStr = WebConfigurationManager.AppSettings["connStr"];
            var dictApiUrl = WebConfigurationManager.AppSettings["dictApiUrl"];

            builder.Register(x => RestClientFactory.Create(dictApiUrl, false)).Keyed<HttpClient>(ApiType.Dict).SingleInstance();

            builder.Register(context => new DatabaseSetting(connStr)).SingleInstance();
            builder.RegisterType<PublicRepository>().As<IPublicRepository>().InstancePerDependency();
            builder.RegisterType<ProductRepository>().As<IProductRepository>().InstancePerDependency();
            builder.RegisterType<MerchantRepository>().As<IMerchantRepository>().InstancePerDependency();
            builder.RegisterType<LoggerRepository>().As<ILoggerRepository>().InstancePerDependency();
            builder.RegisterType<ApplicationRepository>().As<IApplicationRepository>().InstancePerDependency();
            builder.RegisterType<CertificateRepository>().As<ICertificateRepository>().InstancePerDependency();
            builder.RegisterType<ContainerInfoRepository>().As<IContainerInfoRepository>().InstancePerDependency();
            builder.RegisterType<ExampleCertRepository>().As<IExampleCertRepository>().InstancePerDependency();
            builder.RegisterType<CacheService>().As<ICacheService>().InstancePerDependency();

            builder.RegisterType<DictApiProxy>().As<IDictApiProxy>().InstancePerDependency();

            builder.RegisterType<CachedDictService>().As<IDictService>().InstancePerDependency();
        }

        private enum ApiType
        {
            Dict,
        }
    }
}