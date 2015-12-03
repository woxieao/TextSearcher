using System.Web.Configuration;
using Autofac;
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
            builder.Register(context => new DatabaseSetting(connStr)).SingleInstance();
            builder.RegisterType<ApplicationRepository>().As<IApplicationRepository>().InstancePerDependency();
            builder.RegisterType<MerchantRepository>().As<IMerchantRepository>().InstancePerDependency();
            builder.RegisterType<DictionaryRepository>().As<IDictionaryRepository>().InstancePerDependency();
            builder.RegisterType<LoggerRepository>().As<ILoggerRepository>().InstancePerDependency();
            builder.RegisterType<CacheService>().As<ICacheService>().InstancePerDependency();
        }
    }
}