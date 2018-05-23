using PagedList;
using System.Web.Mvc;
using WebNeYapsam.Models;

namespace WebNeYapsam.Controllers
{
    public class HomeController : Controller
    {
        private Title title;

        public HomeController()
        {
            title = new Title();
        }

        public ActionResult Index(int? page, int? CategoryId, string SearchString)
        {
            if (CategoryId != null)
            {
                ViewBag.CategoryId = CategoryId;
            }
            IPagedList<ListTitleViewModelWeb> myView;
            myView = title.List(page, CategoryId, SearchString);
            return View(myView);
        }
    }

}