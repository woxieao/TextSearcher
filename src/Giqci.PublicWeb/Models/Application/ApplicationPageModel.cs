using Giqci.Chapi.Dict.Models;

namespace Giqci.PublicWeb.Models.Application
{
    public class 
        ApplicationPageModel
    {
        public Application Application { get; set; }

        public bool AcceptTerms { get; set; }

        public Country[] Countries { get; set; }

        public HSCode[] CommonHSCodes { get; set; }

        public Port[] DestPorts { get; set; }

        public Port[] LoadingPorts { get; set; }

    }
}