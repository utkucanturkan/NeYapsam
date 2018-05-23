using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebNeYapsam.Classes;
using WebNeYapsam.Models;

namespace WebNeYapsam.Areas.Panel.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CategoryController : Controller
    {
        private Category category;

        public CategoryController()
        {
            category = new Category();
        }

        public ActionResult List()
        {
            List<ListCategoryViewModelShared> categories = category.List();
            if (category.Result.Type == ResultType.Error)
            {
                TempData["result"] = category.Result;
                return RedirectToAction("Index", "Home", new { area = "Panel" });
            }
            return View(categories);
        }

        public ActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Add(CategoryViewModel model)
        {
            if (ModelState.IsValid)
            {
                category.Add(model);
                TempData["result"] = category.Result;
                return RedirectToAction("List");
            }
            return Add();
        }

        public ActionResult Edit(string name)
        {
            EditCategoryViewModel cat = category.Get(name);
            if (category.Result.Type != ResultType.Success)
            {
                TempData["result"] = category.Result;
                return RedirectToAction("List");
            }
            return View(cat);
        }

        [HttpPost]
        public ActionResult Edit(EditCategoryViewModel model)
        {
            if (ModelState.IsValid)
            {
                category.Edit(model);
                TempData["result"] = category.Result;
            }
            return Edit(model.Name);
        }

        public JsonResult Delete(string id)
        {
            category.Delete(int.Parse(id));
            return Json(new { isSuccess = category.Result.IsSuccess }, JsonRequestBehavior.AllowGet);
        }

    }
}