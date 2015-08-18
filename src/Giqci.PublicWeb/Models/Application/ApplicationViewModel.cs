using System.Collections.Generic;

namespace Giqci.PublicWeb.Models.Application
{
    public class ApplicationViewModel
    {
        public Giqci.Models.Application Application { get; set; }

        public List<string> ErrorMessage { get; set; }

        public ApplicationViewModel()
        {
            ErrorMessage = new List<string>();
        }
    }
}