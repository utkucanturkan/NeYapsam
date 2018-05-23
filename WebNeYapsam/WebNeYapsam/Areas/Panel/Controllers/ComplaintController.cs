using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebNeYapsam.Models;

namespace WebNeYapsam.Areas.Panel.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ComplaintController : Controller
    {
        private Complaint complaint;

        public ComplaintController()
        {
            complaint = new Complaint();
        }

        public ActionResult List()
        {
            return View(complaint.List());
        }

        public ActionResult Detail(int id)
        {
            return View(complaint.Get(id));
        }

        public ActionResult Confirm(int id)
        {
            complaint.Confirm(id);
            TempData["result"] = complaint.Result;
            if (complaint.Result.Type != Classes.ResultType.Success)
            {
                return Detail(id);
            }
            return RedirectToAction("List");
        }

        public ActionResult Delete(int id)
        {
            complaint.Delete(id);
            TempData["result"] = complaint.Result;
            if (complaint.Result.Type != Classes.ResultType.Success)
            {
                return Detail(id);
            }
            return RedirectToAction("List");
        }
    }
}