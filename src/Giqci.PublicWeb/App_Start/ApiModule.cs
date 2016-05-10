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
        private bool GetLogSwitch(string configName)
        {
            var logCinfig = WebConfigurationManager.AppSettings[configName] ?? string.Empty;
            return logCinfig == "1" || logCinfig.ToLower() == "true";
        }

        private bool GetLogSwitch(LogSwitch logName)
        {
            var logCinfig = WebConfigurationManager.AppSettings[string.Format("{0}LogSwitch", logName)] ??
                            string.Empty;
            return logCinfig == "1" || logCinfig.ToLower() == "true";
        }

        protected override void Load(ContainerBuilder builder)
        {
            var dictApiUrl = WebConfigurationManager.AppSettings["dictApiUrl"];
            var prodApiUrl = WebConfigurationManager.AppSettings["prodApiUrl"];
            var custApiUrl = WebConfigurationManager.AppSettings["custApiUrl"];
            var logApiUrl = WebConfigurationManager.AppSettings["logApiUrl"];
            var appApiUrl = WebConfigurationManager.AppSettings["appApiUrl"];

            #region init httpClient

            builder.Register(x => RestClientFactory.Create(dictApiUrl, false))
                .Keyed<HttpClient>(ApiType.Dict)
                .SingleInstance();
            builder.Register(x => RestClientFactory.Create(prodApiUrl, false))
                .Keyed<HttpClient>(ApiType.Products)
                .SingleInstance();
            builder.Register(x => RestClientFactory.Create(custApiUrl, false))
                .Keyed<HttpClient>(ApiType.Customers)
                .SingleInstance();
            builder.Register(x => RestClientFactory.Create(appApiUrl, false))
                .Keyed<HttpClient>(ApiType.AppApi)
                .SingleInstance();

            #endregion

            #region init str

            builder.Register(x => logApiUrl)
                .Keyed<string>(StrList.LogUrl)
                .SingleInstance();

            #endregion

            //builder.RegisterType<MerchantApplicationApiProxy>()
            //    .As<IMerchantApplicationApiProxy>()
            //    .InstancePerDependency();
            //builder.RegisterType<CertificateApiProxy>().As<ICertificateApiProxy>().InstancePerDependency();

            #region init log switch 

            builder.Register(x => GetLogSwitch(LogSwitch.Dict)).Keyed<bool>(LogSwitch.Dict).SingleInstance();
            builder.Register(x => GetLogSwitch(LogSwitch.Products)).Keyed<bool>(LogSwitch.Products).SingleInstance();
            builder.Register(x => GetLogSwitch(LogSwitch.Customers)).Keyed<bool>(LogSwitch.Customers).SingleInstance();
            builder.Register(x => GetLogSwitch(LogSwitch.AppApi)).Keyed<bool>(LogSwitch.AppApi).SingleInstance();

            #endregion

            #region init interface

            builder.RegisterType<ProductApiProxy>()
                .As<IProductApiProxy>()
                .WithParameters(
                    new List<ResolvedParameter>()
                    {
                        ResolvedParameter.ForKeyed<HttpClient>(ApiType.Products),
                        ResolvedParameter.ForKeyed<string>(StrList.LogUrl),
                        ResolvedParameter.ForKeyed<bool>(LogSwitch.Products),
                    })
                .InstancePerDependency();

            builder.RegisterType<UserProfileApiProxy>()
                .As<IUserProfileApiProxy>().WithParameters(
                    new List<ResolvedParameter>()
                    {
                        ResolvedParameter.ForKeyed<HttpClient>(ApiType.Customers),
                        ResolvedParameter.ForKeyed<string>(StrList.LogUrl),
                        ResolvedParameter.ForKeyed<bool>(LogSwitch.Customers),
                    })
                .InstancePerDependency();

            builder.RegisterType<MerchantApiProxy>()
                .As<IMerchantApiProxy>().WithParameters(
                    new List<ResolvedParameter>()
                    {
                        ResolvedParameter.ForKeyed<HttpClient>(ApiType.Customers),
                        ResolvedParameter.ForKeyed<string>(StrList.LogUrl),
                        ResolvedParameter.ForKeyed<bool>(LogSwitch.Customers),
                    })
                .InstancePerDependency();
            builder.RegisterType<MerchantProductApiProxy>()
                .As<IMerchantProductApiProxy>().WithParameters(
                    new List<ResolvedParameter>()
                    {
                        ResolvedParameter.ForKeyed<HttpClient>(ApiType.Customers),
                        ResolvedParameter.ForKeyed<string>(StrList.LogUrl),
                        ResolvedParameter.ForKeyed<bool>(LogSwitch.Customers),
                    })
                .InstancePerDependency();
            builder.RegisterType<ApiUserApiProxy>()
                .As<IApiUserApiProxy>().WithParameters(
                    new List<ResolvedParameter>()
                    {
                        ResolvedParameter.ForKeyed<HttpClient>(ApiType.Customers),
                        ResolvedParameter.ForKeyed<string>(StrList.LogUrl),
                        ResolvedParameter.ForKeyed<bool>(LogSwitch.Customers),
                    })
                .InstancePerDependency();
            builder.RegisterType<MerchantApplicationApiProxy>()
                .As<IMerchantApplicationApiProxy>().WithParameters(
                    new List<ResolvedParameter>()
                    {
                        ResolvedParameter.ForKeyed<HttpClient>(ApiType.AppApi),
                        ResolvedParameter.ForKeyed<string>(StrList.LogUrl),
                        ResolvedParameter.ForKeyed<bool>(LogSwitch.AppApi),
                    })
                .InstancePerDependency();
            builder.RegisterType<ApplicationViewModelApiProxy>()
                .As<IApplicationViewModelApiProxy>().WithParameters(
                    new List<ResolvedParameter>()
                    {
                        ResolvedParameter.ForKeyed<HttpClient>(ApiType.AppApi),
                        ResolvedParameter.ForKeyed<string>(StrList.LogUrl),
                        ResolvedParameter.ForKeyed<bool>(LogSwitch.AppApi),
                    })
                .InstancePerDependency();
            builder.RegisterType<CertificateApiProxy>()
                .As<ICertificateApiProxy>().WithParameters(
                    new List<ResolvedParameter>()
                    {
                        ResolvedParameter.ForKeyed<HttpClient>(ApiType.AppApi),
                        ResolvedParameter.ForKeyed<string>(StrList.LogUrl),
                        ResolvedParameter.ForKeyed<bool>(LogSwitch.AppApi),
                    })
                .InstancePerDependency();

            builder.RegisterType<CachedDictService>()
                .As<IDictService>().WithParameters(
                    new List<ResolvedParameter>()
                    {
                        ResolvedParameter.ForKeyed<HttpClient>(ApiType.Dict),
                        ResolvedParameter.ForKeyed<string>(StrList.LogUrl),
                        ResolvedParameter.ForKeyed<bool>(LogSwitch.Dict),
                    })
                .InstancePerDependency();
            builder.RegisterType<AuthService>().As<IAuthService>().InstancePerDependency();
            builder.RegisterType<DataChecker>()
                .As<IDataChecker>()
                .InstancePerDependency();

            #endregion
        }

        private enum ApiType
        {
            Dict,
            Products,
            Customers,
            AppApi
        }

        private enum StrList
        {
            LogUrl,
        }

        private enum LogSwitch
        {
            Dict,
            Products,
            Customers,
            AppApi
        }
    }
}