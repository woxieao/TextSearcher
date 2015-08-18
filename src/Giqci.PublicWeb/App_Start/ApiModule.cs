using System;
using System.Web.Configuration;
using Autofac;
using Giqci.Interfaces;
using Giqci.Repositories;

namespace Giqci.PublicWeb
{
    public class ApiModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            var connStr = WebConfigurationManager.AppSettings["connStr"];
            builder.Register<Func<GiqciDbContext>>(x => () => new GiqciDbContext(connStr));
            builder.RegisterType<GiqciRepository>().As<IGiqciRepository>().InstancePerDependency();
            builder.RegisterType<LoggerRepository>().As<ILoggerRepository>().InstancePerDependency();
        }
    }
}