using System.Collections.Generic;
using System.Web.Mvc;

namespace Giqci.PublicWeb.Models.Application
{
    public class ApplicationViewModel
    {
        public Giqci.Models.Application Application { get; set; }

        public bool AcceptTerms { get; set; }

        public List<string> ErrorMessage { get; set; }

        public IEnumerable<SelectListItem> Countries { get; set; }

        public ApplicationViewModel()
        {
            ErrorMessage = new List<string>();
        }
    }
}