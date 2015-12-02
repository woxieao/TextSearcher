namespace Giqci.PublicWeb.Models.Account
{
    public class ChangePasswordPageModel
    {
        public string OldPassword { get; set; }

        public string NewPassword { get; set; }

        public string ConfirmPassword { get; set; }
    }
}