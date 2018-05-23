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
    public class RankController : Controller
    {
        private Rank rank;

        public RankController()
        {
            rank = new Rank();
        }

        public ActionResult List()
        {
            List<ListRankViewModel> ranks = rank.List();
            if (rank.Result.Type == ResultType.Error)
            {
                TempData["result"] = rank.Result;
                return RedirectToAction("Index", "Home", new { area = "Panel" });
            }
            return View(ranks);
        }

        public void GetRanksToViewBag(int selectedItem)
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                var ranks = db.Ranks.Select(r => new SelectListItem()
                {
                    Selected = (selectedItem == r.Id && selectedItem != 0) ? true : false,
                    Text = r.Name,
                    Value = r.Id.ToString()
                }).ToList();
                ViewBag.Ranks = ranks;
            }
        }

        public ActionResult Add()
        {
            GetRanksToViewBag(0);
            return View();
        }

        [HttpPost]
        public ActionResult Add(RankViewModel model, FormCollection formInfo)
        {
            if (ModelState.IsValid)
            {
                model.ParentRankId = int.Parse(formInfo["rank"]);
                rank.Add(model);
                TempData["result"] = rank.Result;
                return RedirectToAction("List");
            }
            return Add();
        }

        public ActionResult Edit(string name)
        {
            EditRankViewModel model = rank.Get(name);
            if (rank.Result.Type != ResultType.Success)
            {
                TempData["result"] = rank.Result;
                return RedirectToAction("List");
            }
            int selecteItem = 0;
            if (model.ParentRankId != null)
            {
                selecteItem = (int)model.ParentRankId;
            }
            GetRanksToViewBag(selecteItem);
            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(EditRankViewModel model)
        {
            if (ModelState.IsValid)
            {
                rank.Edit(model);
                TempData["result"] = rank.Result;
            }
            return Edit(model.Name);
        }

        public JsonResult Delete(string id)
        {
            rank.Delete(int.Parse(id));
            return Json(new { isSuccess = rank.Result.IsSuccess }, JsonRequestBehavior.AllowGet);
        }
    }
}