using Giqci.Models;

namespace Giqci.PublicWeb.Models.Account
{
    public class RegistrationViewModel : MerchantReg
    {
        public string ConfirmPassword { get; set; }

        public string ErrorMessage { get; set; }
    }
}