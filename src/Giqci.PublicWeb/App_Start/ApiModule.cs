using System.Collections.Generic;
using System.Net.Http;
using Autofac;
using Autofac.Core;
using Giqci.ApiProxy;
using Giqci.ApiProxy.App;
using Giqci.ApiProxy.File;
using Giqci.ApiProxy.Services;
using Giqci.Chapi.Models.File;
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
                .Keyed<string>(ValueList.LogUrl)
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

            builder.Register(x => Config.LogSwitch.DictLogSwitch).Keyed<bool>(LogSwitch.Dict).SingleInstance();
            builder.Register(x => Config.LogSwitch.ProductsLogSwitch).Keyed<bool>(LogSwitch.Products).SingleInstance();
            builder.Register(x => Config.LogSwitch.CustomersLogSwitch).Keyed<bool>(LogSwitch.Customers).SingleInstance();
            builder.Register(x => Config.LogSwitch.AppLogSwitch).Keyed<bool>(LogSwitch.App).SingleInstance();
            builder.Register(x => Config.LogSwitch.FileLogSwitch).Keyed<bool>(LogSwitch.File).SingleInstance();

            #endregion

            #region init interface

            builder.RegisterType<ProductApiProxy>()
                .As<IProductApiProxy>()
                .WithParameters(
                    new List<ResolvedParameter>()
                    {
                        ResolvedParameter.ForKeyed<HttpClient>(ApiType.Products),
                        ResolvedParameter.ForKeyed<string>(ValueList.LogUrl),
                        ResolvedParameter.ForKeyed<bool>(LogSwitch.Products),
                    })
                .InstancePerDependency();

            builder.RegisterType<UserProfileApiProxy>()
                .As<IUserProfileApiProxy>().WithParameters(
                    new List<ResolvedParameter>()
                    {
                        ResolvedParameter.ForKeyed<HttpClient>(ApiType.Customers),
                        ResolvedParameter.ForKeyed<string>(ValueList.LogUrl),
                        ResolvedParameter.ForKeyed<bool>(LogSwitch.Customers),
                    })
                .InstancePerDependency();

            builder.RegisterType<MerchantApiProxy>()
                .As<IMerchantApiProxy>().WithParameters(
                    new List<ResolvedParameter>()
                    {
                        ResolvedParameter.ForKeyed<HttpClient>(ApiType.Customers),
                        ResolvedParameter.ForKeyed<string>(ValueList.LogUrl),
                        ResolvedParameter.ForKeyed<bool>(LogSwitch.Customers),
                    })
                .InstancePerDependency();
            builder.RegisterType<MerchantProductApiProxy>()
                .As<IMerchantProductApiProxy>().WithParameters(
                    new List<ResolvedParameter>()
                    {
                        ResolvedParameter.ForKeyed<HttpClient>(ApiType.Customers),
                        ResolvedParameter.ForKeyed<string>(ValueList.LogUrl),
                        ResolvedParameter.ForKeyed<bool>(LogSwitch.Customers),
                    })
                .InstancePerDependency();
            builder.RegisterType<ApiUserApiProxy>()
                .As<IApiUserApiProxy>().WithParameters(
                    new List<ResolvedParameter>()
                    {
                        ResolvedParameter.ForKeyed<HttpClient>(ApiType.Customers),
                        ResolvedParameter.ForKeyed<string>(ValueList.LogUrl),
                        ResolvedParameter.ForKeyed<bool>(LogSwitch.Customers),
                    })
                .InstancePerDependency();
            builder.RegisterType<MerchantApplicationApiProxy>()
                .As<IMerchantApplicationApiProxy>().WithParameters(
                    new List<ResolvedParameter>()
                    {
                        ResolvedParameter.ForKeyed<HttpClient>(ApiType.App),
                        ResolvedParameter.ForKeyed<string>(ValueList.LogUrl),
                        ResolvedParameter.ForKeyed<bool>(LogSwitch.App),
                    })
                .InstancePerDependency();
            builder.RegisterType<ApplicationViewModelApiProxy>()
                .As<IApplicationViewModelApiProxy>().WithParameters(
                    new List<ResolvedParameter>()
                    {
                        ResolvedParameter.ForKeyed<HttpClient>(ApiType.App),
                        ResolvedParameter.ForKeyed<string>(ValueList.LogUrl),
                        ResolvedParameter.ForKeyed<bool>(LogSwitch.App),
                    })
                .InstancePerDependency();
            builder.RegisterType<CertificateApiProxy>()
                .As<ICertificateApiProxy>().WithParameters(
                    new List<ResolvedParameter>()
                    {
                        ResolvedParameter.ForKeyed<HttpClient>(ApiType.App),
                        ResolvedParameter.ForKeyed<string>(ValueList.LogUrl),
                        ResolvedParameter.ForKeyed<bool>(LogSwitch.App),
                    })
                .InstancePerDependency();

            builder.RegisterType<CachedDictService>()
                .As<IDictService>().WithParameters(
                    new List<ResolvedParameter>()
                    {
                        ResolvedParameter.ForKeyed<HttpClient>(ApiType.Dict),
                        ResolvedParameter.ForKeyed<string>(ValueList.LogUrl),
                        ResolvedParameter.ForKeyed<bool>(LogSwitch.Dict),
                    })
                .InstancePerDependency();

            builder.RegisterType<FileApiProxy>()
                .As<IFileApiProxy>().WithParameters(
                    new List<ResolvedParameter>()
                    {
                        ResolvedParameter.ForKeyed<HttpClient>(ApiType.File),
                        ResolvedParameter.ForKeyed<string>(ValueList.LogUrl),
                        ResolvedParameter.ForKeyed<bool>(LogSwitch.File),
                        ResolvedParameter.ForKeyed<UploadInitInfo>(ValueList.UploadInitInfo),
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
            App,
            File
        }

        private enum ValueList
        {
            LogUrl,
            UploadInitInfo,
        }

        private enum LogSwitch
        {
            Dict,
            Products,
            Customers,
            App,
            File
        }
    }
}