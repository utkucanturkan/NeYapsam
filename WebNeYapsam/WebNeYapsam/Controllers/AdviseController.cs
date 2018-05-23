using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebNeYapsam.Classes;
using WebNeYapsam.Models;
namespace WebNeYapsam.Controllers
{
    public class AdviseController : Controller
    {
        private Advise advise;

        public AdviseController()
        {
            advise = new Advise();
        }

        public JsonResult Add(AddAdviseViewModelWeb model)
        {
            if (!string.IsNullOrEmpty(model.Description))
            {
                advise.Add(model);
                string userImage = "";
                if (advise.Result.IsSuccess)
                {
                    using (ApplicationDbContext db = new ApplicationDbContext())
                    {
                        userImage = db.Users.Find(model.UserId).Image;
                    }
                }
                return Json(new { isSuccess = advise.Result.IsSuccess, userImage = userImage }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { isSuccess = false, userImage = "" }, JsonRequestBehavior.AllowGet);
        }
    }
}