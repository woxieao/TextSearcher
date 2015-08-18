using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Giqci.Models;
using Giqci.PublicWeb.Models.Application;
using Giqci.Repositories;

namespace Giqci.PublicWeb.Controllers
{
    public class FormsController : Controller
    {
        private IGiqciRepository _repo;

        public FormsController(IGiqciRepository repo)
        {
            _repo = repo;
        }

        [Route("forms/app")]
        [HttpGet]
        public ActionResult Application(string applicantCode)
        {
            var model = new ApplicationViewModel
            {
                Application = new Application
                {
                    ApplicantCode = applicantCode,
                    Goods = new List<GoodsItem>()
                }
            };
            return View(model);
        }

        [Route("forms/app")]
        [HttpPost]
        public ActionResult SubmitApplication(ApplicationViewModel model)
        {
            if (validateApplication(model) && Request.Form["submit"] == "submit")
            {
                // submit
                _repo.CreateApplication(new[] { model.Application }, null);
                return View("Created");
            }
            return View("Application", model);
        }

        private bool validateApplication(ApplicationViewModel model)
        {
            for (var i = model.Application.Goods.Count - 1; i >= 0; i--)
            {
                if (string.IsNullOrEmpty(model.Application.Goods[i].Description))
                    model.Application.Goods.RemoveAt(i);
                else if (string.IsNullOrEmpty(model.Application.Goods[i].Code)
                    || string.IsNullOrEmpty(model.Application.Goods[i].HSCode)
                    || string.IsNullOrEmpty(model.Application.Goods[i].BatchNo)
                    || string.IsNullOrEmpty(model.Application.Goods[i].ExpiryDate)
                    || string.IsNullOrEmpty(model.Application.Goods[i].Manufacturer)
                    || string.IsNullOrEmpty(model.Application.Goods[i].ManufacturerDate)
                    || string.IsNullOrEmpty(model.Application.Goods[i].Brand)
                    || model.Application.Goods[i].Quantity <= 0
                    || string.IsNullOrEmpty(model.Application.Goods[i].Package))
                {
                    model.ErrorMessage.Add("请填写完整商品资料");
                    model.Application.Goods.RemoveAt(i);
                }
            }
            return !model.ErrorMessage.Any();
        }
    }
}