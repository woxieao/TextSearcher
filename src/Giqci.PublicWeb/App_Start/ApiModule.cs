using System.Web.Configuration;
using Autofac;
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
            builder.Register(context => new DatabaseSetting(connStr)).SingleInstance();
            builder.RegisterType<PublicRepository>().As<IPublicRepository>().InstancePerDependency();
            builder.RegisterType<MerchantRepository>().As<IMerchantRepository>().InstancePerDependency();
            builder.RegisterType<DictionaryRepository>().As<IDictionaryRepository>().InstancePerDependency();
            builder.RegisterType<LoggerRepository>().As<ILoggerRepository>().InstancePerDependency();
            builder.RegisterType<CachedDictionaryService>().As<ICachedDictionaryService>().InstancePerDependency();
            builder.RegisterType<ApplicationRepository>().As<IApplicationRepository>().InstancePerDependency();
            builder.RegisterType<CertificateRepository>().As<ICertificateRepository>().InstancePerDependency();
            builder.RegisterType<GoodsRepository>().As<IGoodsRepository>().InstancePerDependency();
            builder.RegisterType<ContainerInfoRepository>().As<IContainerInfoRepository>().InstancePerDependency();
            builder.RegisterType<ExampleCertRepository>().As<IExampleCertRepository>().InstancePerDependency();
        }
    }
}