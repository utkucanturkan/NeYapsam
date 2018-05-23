using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebNeYapsam.Classes;
using WebNeYapsam.Models;

namespace WebNeYapsam.Areas.Panel.Controllers
{
    [Authorize]
    public class TitleController : Controller
    {
        private Title title;

        public TitleController()
        {
            title = new Title();
        }

        [Authorize(Roles = "Admin")]
        public ActionResult List()
        {
            List<ListTitleViewModelPanel> titles = title.List();
            if (title.Result.Type == ResultType.Error)
            {
                TempData["result"] = title.Result;
                return RedirectToAction("Index", "Home", new { area = "Panel" });

            }
            return View(title.List());
        }

        public ActionResult Edit(int id)
        {
            EditTitleViewModelPanel editTitle = title.Get(id);
            if (title.Result.Type != ResultType.Success)
            {
                TempData["result"] = title.Result;
                return RedirectToAction("List");
            }

            if (User.IsInRole("User"))
            {
                if (editTitle.UserName == User.Identity.Name)
                {
                    return View(editTitle);
                }
                TempData["result"] = new Result(ResultType.Error, "Hata oluştu");
                return RedirectToAction("Page", "User", new { area = "Panel", name = User.Identity.Name });
            }
            else
            {
                return View(editTitle);
            }
        }

        [HttpPost]
        public ActionResult Edit(EditTitleViewModelPanel model)
        {
            if (ModelState.IsValid)
            {
                if (User.IsInRole("User"))
                {
                    if (model.UserName == User.Identity.Name)
                    {
                        title.Edit(model, User.Identity.GetUserId());
                        TempData["result"] = title.Result;
                        return Edit(model.Id);
                    }
                    return RedirectToAction("Page", "User", new { area = "Panel", name = User.Identity.Name });
                }
                else
                {
                    title.Edit(model, User.Identity.GetUserId());
                    TempData["result"] = title.Result;
                    return Edit(model.Id);
                }
            }
            return Edit(model.Id);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int id)
        {
            title.Delete(id, User.Identity.GetUserId());
            TempData["result"] = title.Result;
            return RedirectToAction("List");
        }

        public JsonResult ChangeState(string id)
        {
            title.ChangeState(int.Parse(id));
            return Json(new { isSuccess = title.Result.IsSuccess }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteAdvise(int id)
        {
          // Delete method id düzenlemesi  
            title.Delete(id, User.Identity.GetUserId());
            return Json(new { isSuccess = title.Result.IsSuccess }, JsonRequestBehavior.AllowGet);
        }

    }
}