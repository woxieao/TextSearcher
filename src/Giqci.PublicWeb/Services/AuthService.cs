using System;
using System.Web;
using System.Web.Security;
using Giqci.Chapi.Models.Customer;
using Giqci.Interfaces;

namespace Giqci.PublicWeb.Services
{
    public class AuthService : IAuthService
    {
        private readonly IMerchantApiProxy _proxy;

        public AuthService(IMerchantApiProxy proxy)
        {
            _proxy = proxy;
        }

        private const string SESSION_KEY = "MUSER";

        public void SetAuth(Merchant input)
        {
            FormsAuthentication.SetAuthCookie(input.Email, false);
            HttpContext.Current.Session[SESSION_KEY] = new AuthModel
            {
                MerchantEmail = input.Email,
                MerchantId = input.Id
            };
        }

        public void Signout()
        {
            HttpContext.Current.Session.Clear();
            FormsAuthentication.SignOut();
        }

        public void Renew()
        {
            var user = HttpContext.Current.User;
            if (!user.Identity.IsAuthenticated)
                return;

            FormsAuthentication.SetAuthCookie(user.Identity.Name, false);
        }

        public AuthModel GetAuth()
        {
            var user = HttpContext.Current.User;
            if (!user.Identity.IsAuthenticated)
                return null;

            var auth = HttpContext.Current.Session[SESSION_KEY] as AuthModel;
            if (auth == null)
            {
                var m = _proxy.GetMerchant(HttpContext.Current.User.Identity.Name);
                if (m == null)
                    throw new ApplicationException("Invalid Authentication");
                auth = new AuthModel { MerchantId = m.Id, MerchantEmail = m.Email };
                HttpContext.Current.Session[SESSION_KEY] = auth;
            }
            return auth;
        }
    }

    public interface IAuthService
    {
        void SetAuth(Merchant input);

        AuthModel GetAuth();

        void Signout();

        void Renew();
    }

    public class AuthModel
    {
        public int MerchantId { get; set; }

        public string MerchantEmail { get; set; }
    }
}