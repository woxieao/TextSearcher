using System.Collections.Generic;
using System.Web.Mvc;
using Giqci.Models;

namespace Giqci.PublicWeb.Models.Application
{
    public class ApplicationViewModel
    {
        public Giqci.Models.Application Application { get; set; }

        public bool AcceptTerms { get; set; }

        public GoodsItem NewItem { get; set; }

        public List<string> ErrorMessage { get; set; }

        public IEnumerable<SelectListItem> Countries { get; set; }

        public List<SelectListItem> CommonHSCodes { get; set; }

        public List<SelectListItem> DestPorts { get; set; }

        public List<SelectListItem> LoadingPorts { get; set; }
        
        public ApplicationViewModel()
        {
            ErrorMessage = new List<string>();
        }
    }
}