using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebNeYapsam.Models;

namespace WebNeYapsam.Areas.Panel.Controllers
{
    [Authorize(Roles = "Admin")]
    public class HomeController : Controller
    {
        private PanelHomeViewModel model;

        public HomeController()
        {
            model = new PanelHomeViewModel();
        }

        public ActionResult Index()
        {
            try
            {
                using (ApplicationDbContext db = new ApplicationDbContext())
                {
                    model.DataCount = db.Datas.Count();
                    model.TitleCount = db.Titles.Count();
                    model.AdviseCount = model.DataCount - model.TitleCount;
                    model.ComplaintCount = db.Complaints.Count();
                    model.UserCount = db.Users.Count();
                }
                return View(model);
            }
            catch (Exception)
            {
                throw;
            }           
        }
    }

    public class PanelHomeViewModel
    {
        public int DataCount { get; set; }
        public int TitleCount { get; set; }
        public int AdviseCount { get; set; }
        public int UserCount { get; set; }
        public int ComplaintCount { get; set; }
    }
}