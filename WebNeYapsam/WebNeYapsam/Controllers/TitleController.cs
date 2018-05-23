using Microsoft.AspNet.Identity;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebNeYapsam.Classes;
using WebNeYapsam.Models;

namespace WebNeYapsam.Controllers
{
    public class TitleController : Controller
    {
        private Title title;
        private Advise advise;
        private Category category;
        public TitleController()
        {
            title = new Title();
            advise = new Advise();
            category = new Category();
        }

        public ActionResult Index(int? page, int TitleId)
        {
            TitleDetailViewModel myView = new TitleDetailViewModel();
            string currentUserId = null;
            if (User.Identity.IsAuthenticated)
            {
                currentUserId = User.Identity.GetUserId();
            }
            myView.ListAdviseViewModelWeb = advise.List(page, TitleId, currentUserId);
            myView.GetTitleViewModelWeb = title.Get(TitleId, currentUserId);
            return View(myView);
        }

        [Authorize]
        [ValidateInput(false)]
        [HttpPost]
        public ActionResult Add(AddTitleViewModel model, FormCollection formInfo)
        {

            model.CategoryId = int.Parse(formInfo["category"]);
            model.UserId = User.Identity.GetUserId();
            if (!string.IsNullOrEmpty(model.Name) && !string.IsNullOrEmpty(model.Description))
            {
                //model.Description = model.Description.Replace("\r\n", "<br>");
                title = title.Add(model);
                if (title.Result.Type != ResultType.Success)
                {
                    TempData["result"] = title.Result;
                    return RedirectToAction("Index", "Home", new { area = "" });
                }
                return RedirectToAction("Index", "Title", new { TitleId = title.Id, CategoryId = title.CategoryId });
            }
            TempData["result"] = new Result(ResultType.Error, "Başlık Eklenemedi. Gerekli alanları doldurunuz.");
            return RedirectToAction("Index", "Home", new { area = "" });
        }

        [Authorize]
        public JsonResult Evaluation(DataEvaluationModel model)
        {
            if (!string.IsNullOrEmpty(model.user) && !string.IsNullOrEmpty(model.id) && !string.IsNullOrEmpty(model.datatype) && !string.IsNullOrEmpty(model.evaluation))
            {
                string currentUserId = User.Identity.GetUserId();
                int id;
                if (int.TryParse(model.id, out id))
                {
                    switch (model.evaluation)
                    {
                        case "like":
                            title.SetEvaluation(id, model.user, currentUserId, model.datatype, EvaluationType.Like);
                            return Json(new { isSuccess = title.Result.IsSuccess }, JsonRequestBehavior.AllowGet);
                        case "dislike":
                            switch (model.datatype)
                            {
                                case "advise":
                                case "title":
                                    title.SetEvaluation(id, model.user, currentUserId, model.datatype, EvaluationType.Dislike);
                                    return Json(new { isSuccess = title.Result.IsSuccess }, JsonRequestBehavior.AllowGet);
                                default:
                                    return Json(new { isSuccess = false }, JsonRequestBehavior.AllowGet);
                            }
                        default:
                            return Json(new { isSuccess = false }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    return Json(new { isSuccess = false }, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(new { isSuccess = false }, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        public ActionResult Complaint(SetComplaintViewModel model)
        {
            if (!string.IsNullOrEmpty(model.DataId.ToString()))
            {
                model.ApplicationUserId = User.Identity.GetUserId();
                title.SetComplaint(model);
                TempData["result"] = title.Result;
            }
            else
            {
                TempData["result"] = new Result(ResultType.UnSuccessful, "Beklenmedik hata oluştu.");
            }
            return Redirect(Request.UrlReferrer.ToString());
        }
    }

    public class TitleDetailViewModel
    {
        public GetTitleViewModelWeb GetTitleViewModelWeb { get; set; }
        public IPagedList<ListAdviseViewModelWeb> ListAdviseViewModelWeb { get; set; }
    }

    public class DataEvaluationModel
    {
        public string user { get; set; }
        public string id { get; set; }
        public string datatype { get; set; }
        public string evaluation { get; set; }
    }
}