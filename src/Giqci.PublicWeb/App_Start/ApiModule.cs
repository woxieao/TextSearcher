using System.Collections.Generic;
using System.Net.Http;
using System.Xml;
using Autofac;
using Autofac.Core;
using Giqci.ApiProxy;
using Giqci.ApiProxy.App;
using Giqci.ApiProxy.Cust;
using Giqci.ApiProxy.Dict;
using Giqci.ApiProxy.File;
using Giqci.ApiProxy.Logging;
using Giqci.ApiProxy.Services;
using Giqci.Chapi.Models.File;
using Giqci.Chapi.Models.Logging;
using Giqci.Interfaces;
using Giqci.PublicWeb.Models;
using Giqci.PublicWeb.Services;

namespace Giqci.PublicWeb
{
    public class ApiModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            #region init httpClient

            builder.Register(x => RestClientFactory.Create(Config.ApiUrl.DictApiUrl, false))
                .Keyed<HttpClient>(ApiType.Dict)
                .SingleInstance();
            builder.Register(x => RestClientFactory.Create(Config.ApiUrl.IpApiUrl, false))
             .Keyed<HttpClient>(ApiType.Ip)
             .SingleInstance();
            builder.Register(x => RestClientFactory.Create(Config.ApiUrl.ProdApiUrl, false))
                .Keyed<HttpClient>(ApiType.Products)
                .SingleInstance();
            builder.Register(x => RestClientFactory.Create(Config.ApiUrl.CustApiUrl, false))
                .Keyed<HttpClient>(ApiType.Customers)
                .SingleInstance();
            builder.Register(x => RestClientFactory.Create(Config.ApiUrl.AppApiUrl, false))
                .Keyed<HttpClient>(ApiType.App)
                .SingleInstance();
            builder.Register(x => RestClientFactory.Create(Config.ApiUrl.FileApiUrl, false))
                .Keyed<HttpClient>(ApiType.File)
                .SingleInstance();

            #endregion

            #region init value list

            builder.Register(x => Config.ApiUrl.LogApiUrl)
                .Keyed<string>(ValueList.LogApiUrl)
                .SingleInstance();
            builder.Register(x => new UploadInitInfo
            {
                AllowTypeList = Config.Filer.AllowTypeList,
                MaxLength = Config.Filer.FileMaxLength
            }).Keyed<UploadInitInfo>(ValueList.UploadInitInfo).SingleInstance();

            #endregion

            //builder.RegisterType<MerchantApplicationApiProxy>()
            //    .As<IMerchantApplicationApiProxy>()
            //    .InstancePerDependency();
            //builder.RegisterType<CertificateApiProxy>().As<ICertificateApiProxy>().InstancePerDependency();

            #region init log switch 

            builder.Register(x => new LoggingApiProxy(new LogConfig()
            {
                Log = Config.LogSwitch.AppCacheLogSwitch,
                LogUrl = Config.ApiUrl.LogApiUrl
            })).Keyed<ILoggingApiProxy>(LogSwitch.AppCache).SingleInstance();
            builder.Register(x => new LoggingApiProxy(new LogConfig()
            {
                Log = Config.LogSwitch.DictLogSwitch,
                LogUrl = Config.ApiUrl.LogApiUrl
            })).Keyed<ILoggingApiProxy>(LogSwitch.Dict).SingleInstance();
            builder.Register(x => new LoggingApiProxy(new LogConfig()
            {
                Log = Config.LogSwitch.IpLogSwitch,
                LogUrl = Config.ApiUrl.LogApiUrl
            })).Keyed<ILoggingApiProxy>(LogSwitch.Ip).SingleInstance();
            builder.Register(x => new LoggingApiProxy(new LogConfig()
            {
                Log = Config.LogSwitch.ProductsLogSwitch,
                LogUrl = Config.ApiUrl.LogApiUrl
            })).Keyed<ILoggingApiProxy>(LogSwitch.Products).SingleInstance();

            builder.Register(x => new LoggingApiProxy(new LogConfig()
            {
                Log = Config.LogSwitch.CustomersLogSwitch,
                LogUrl = Config.ApiUrl.LogApiUrl
            })).Keyed<ILoggingApiProxy>(LogSwitch.Customers).SingleInstance();

            builder.Register(x => new LoggingApiProxy(new LogConfig()
            {
                Log = Config.LogSwitch.AppLogSwitch,
                LogUrl = Config.ApiUrl.LogApiUrl
            })).Keyed<ILoggingApiProxy>(LogSwitch.App).SingleInstance();
            builder.Register(x => new LoggingApiProxy(new LogConfig()
            {
                Log = Config.LogSwitch.FileLogSwitch,
                LogUrl = Config.ApiUrl.LogApiUrl
            })).Keyed<ILoggingApiProxy>(LogSwitch.File).SingleInstance();

            #endregion

            #region init interface

            builder.RegisterType<ProductApiProxy>()
                .As<IProductApiProxy>()
                .WithParameters(
                    new List<ResolvedParameter>()
                    {
                        ResolvedParameter.ForKeyed<HttpClient>(ApiType.Products),
                        ResolvedParameter.ForKeyed<ILoggingApiProxy>(LogSwitch.Products)
                    })
                .InstancePerDependency();

            builder.RegisterType<UserProfileApiProxy>()
                .As<IUserProfileApiProxy>().WithParameters(
                    new List<ResolvedParameter>()
                    {
                        ResolvedParameter.ForKeyed<HttpClient>(ApiType.Customers),
                        ResolvedParameter.ForKeyed<ILoggingApiProxy>(LogSwitch.Customers)
                    })
                .InstancePerDependency();

            builder.RegisterType<MerchantApiProxy>()
                .As<IMerchantApiProxy>().WithParameters(
                    new List<ResolvedParameter>()
                    {
                        ResolvedParameter.ForKeyed<HttpClient>(ApiType.Customers),
                        ResolvedParameter.ForKeyed<ILoggingApiProxy>(LogSwitch.Customers)
                    })
                .InstancePerDependency();
            builder.RegisterType<MerchantProductApiProxy>()
                .As<IMerchantProductApiProxy>().WithParameters(
                    new List<ResolvedParameter>()
                    {
                        ResolvedParameter.ForKeyed<HttpClient>(ApiType.Customers),
                        ResolvedParameter.ForKeyed<ILoggingApiProxy>(LogSwitch.Customers)
                    })
                .InstancePerDependency();
            builder.RegisterType<ApiUserApiProxy>()
                .As<IApiUserApiProxy>().WithParameters(
                    new List<ResolvedParameter>()
                    {
                        ResolvedParameter.ForKeyed<HttpClient>(ApiType.Customers),
                        ResolvedParameter.ForKeyed<ILoggingApiProxy>(LogSwitch.Customers)
                    })
                .InstancePerDependency();
            builder.RegisterType<MerchantApplicationApiProxy>()
                .As<IMerchantApplicationApiProxy>().WithParameters(
                    new List<ResolvedParameter>()
                    {
                        ResolvedParameter.ForKeyed<HttpClient>(ApiType.App),
                        ResolvedParameter.ForKeyed<ILoggingApiProxy>(LogSwitch.App)
                    })
                .InstancePerDependency();
            builder.RegisterType<ApplicationViewModelApiProxy>()
                .As<IApplicationViewModelApiProxy>().WithParameters(
                    new List<ResolvedParameter>()
                    {
                        ResolvedParameter.ForKeyed<HttpClient>(ApiType.App),
                        ResolvedParameter.ForKeyed<ILoggingApiProxy>(LogSwitch.App)
                    })
                .InstancePerDependency();
            builder.RegisterType<CertificateApiProxy>()
                .As<ICertificateApiProxy>().WithParameters(
                    new List<ResolvedParameter>()
                    {
                        ResolvedParameter.ForKeyed<HttpClient>(ApiType.App),
                        ResolvedParameter.ForKeyed<ILoggingApiProxy>(LogSwitch.App)
                    })
                .InstancePerDependency();

            builder.RegisterType<CachedDictService>()
                .As<IDictService>().WithParameters(
                    new List<ResolvedParameter>()
                    {
                        ResolvedParameter.ForKeyed<HttpClient>(ApiType.Dict),
                        ResolvedParameter.ForKeyed<ILoggingApiProxy>(LogSwitch.Dict)
                    })
                .InstancePerDependency();

            builder.RegisterType<FileApiProxy>()
                .As<IFileApiProxy>().WithParameters(
                    new List<ResolvedParameter>()
                    {
                        ResolvedParameter.ForKeyed<HttpClient>(ApiType.File),
                        ResolvedParameter.ForKeyed<ILoggingApiProxy>(LogSwitch.File),
                        ResolvedParameter.ForKeyed<UploadInitInfo>(ValueList.UploadInitInfo)
                    })
                .InstancePerDependency();
            builder.RegisterType<AuthService>().As<IAuthService>().InstancePerDependency();
           


            builder.RegisterType<ApplicationCacheApiProxy>()
                .As<IApplicationCacheApiProxy>().WithParameters(
                    new List<ResolvedParameter>()
                    {
                        ResolvedParameter.ForKeyed<HttpClient>(ApiType.App),
                        ResolvedParameter.ForKeyed<ILoggingApiProxy>(LogSwitch.AppCache)
                    })
                .InstancePerDependency();

            builder.RegisterType<IpDictionaryService>()
             .As<IIpDictionaryService>().WithParameters(
                 new List<ResolvedParameter>()
                 {
                        ResolvedParameter.ForKeyed<HttpClient>(ApiType.Ip),
                        ResolvedParameter.ForKeyed<ILoggingApiProxy>(LogSwitch.Ip)
                 })
             .InstancePerDependency();

            builder.RegisterType<ZcodeApplyLogApiProxy>()
           .As<IZcodeApplyLogApiProxy>().WithParameters(
               new List<ResolvedParameter>()
               {
                        ResolvedParameter.ForKeyed<HttpClient>(ApiType.Customers),
                        ResolvedParameter.ForKeyed<ILoggingApiProxy>(LogSwitch.Customers)
               })
           .InstancePerDependency();
            #endregion
        }

        private enum ApiType
        {
            Dict,
            Products,
            Customers,
            App,
            File,
            Ip
        }

        private enum ValueList
        {
            LogApiUrl,
            UploadInitInfo,
        }

        private enum LogSwitch
        {
            Dict,
            Products,
            Customers,
            App,
            File,
            AppCache,
            Ip,
        }
    }
}