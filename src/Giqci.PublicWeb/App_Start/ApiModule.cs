using System.Collections.Generic;
using System.Net.Http;
using System.Web.Configuration;
using Autofac;
using Autofac.Core;
using Giqci.ApiProxy;
using Giqci.ApiProxy.App;
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
            builder.Register(x => logApiUrl)
                .Keyed<string>(ApiType.Log)
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
                .WithParameters(
                    new List<ResolvedParameter>()
                    {
                        ResolvedParameter.ForKeyed<HttpClient>(ApiType.Products),
                        ResolvedParameter.ForKeyed<string>(ApiType.Log),
                    })
                .InstancePerDependency();

            builder.RegisterType<UserProfileApiProxy>()
                .As<IUserProfileApiProxy>().WithParameters(
                    new List<ResolvedParameter>()
                    {
                        ResolvedParameter.ForKeyed<HttpClient>(ApiType.Customers),
                        ResolvedParameter.ForKeyed<string>(ApiType.Log),
                    })
                .InstancePerDependency();

            builder.RegisterType<MerchantApiProxy>()
                .As<IMerchantApiProxy>().WithParameters(
                    new List<ResolvedParameter>()
                    {
                        ResolvedParameter.ForKeyed<HttpClient>(ApiType.Customers),
                        ResolvedParameter.ForKeyed<string>(ApiType.Log),
                    })
                .InstancePerDependency();
            builder.RegisterType<MerchantProductApiProxy>()
                .As<IMerchantProductApiProxy>().WithParameters(
                    new List<ResolvedParameter>()
                    {
                        ResolvedParameter.ForKeyed<HttpClient>(ApiType.Customers),
                        ResolvedParameter.ForKeyed<string>(ApiType.Log),
                    })
                .InstancePerDependency();
            builder.RegisterType<ApiUserApiProxy>()
                .As<IApiUserApiProxy>().WithParameters(
                    new List<ResolvedParameter>()
                    {
                        ResolvedParameter.ForKeyed<HttpClient>(ApiType.Customers),
                        ResolvedParameter.ForKeyed<string>(ApiType.Log),
                    })
                .InstancePerDependency();
            builder.RegisterType<MerchantApplicationApiProxy>()
                .As<IMerchantApplicationApiProxy>().WithParameters(
                    new List<ResolvedParameter>()
                    {
                        ResolvedParameter.ForKeyed<HttpClient>(ApiType.AppApiUrl),
                        ResolvedParameter.ForKeyed<string>(ApiType.Log),
                    })
                .InstancePerDependency();
            builder.RegisterType<ApplicationViewModelApiProxy>()
                .As<IApplicationViewModelApiProxy>().WithParameters(
                    new List<ResolvedParameter>()
                    {
                        ResolvedParameter.ForKeyed<HttpClient>(ApiType.AppApiUrl),
                        ResolvedParameter.ForKeyed<string>(ApiType.Log),
                    })
                .InstancePerDependency();
            builder.RegisterType<CertificateApiProxy>()
                .As<ICertificateApiProxy>().WithParameters(
                    new List<ResolvedParameter>()
                    {
                        ResolvedParameter.ForKeyed<HttpClient>(ApiType.AppApiUrl),
                        ResolvedParameter.ForKeyed<string>(ApiType.Log),
                    })
                .InstancePerDependency();

            builder.RegisterType<CachedDictService>()
                .As<IDictService>().WithParameters(
                    new List<ResolvedParameter>()
                    {
                        ResolvedParameter.ForKeyed<HttpClient>(ApiType.Dict),
                        ResolvedParameter.ForKeyed<string>(ApiType.Log),
                    })
                .InstancePerDependency();
            builder.RegisterType<AuthService>().As<IAuthService>().InstancePerDependency();
            builder.RegisterType<DataChecker>()
                .As<IDataChecker>()
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