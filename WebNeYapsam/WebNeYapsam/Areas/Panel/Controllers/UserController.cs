using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using WebNeYapsam.Classes;
using WebNeYapsam.Models;

namespace WebNeYapsam.Areas.Panel.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        private ApplicationUser applicationUser;

        public UserController()
        {
            applicationUser = new ApplicationUser();
        }

        [Authorize(Roles = "Admin")]
        public ActionResult List()
        {
            List<ListUserViewModelPanel> users = applicationUser.List(/*User.Identity.Name*/);
            if (applicationUser.Result.Type == ResultType.Error)
            {
                TempData["result"] = applicationUser.Result;
                return RedirectToAction("Index", "Home", new { area = "Panel" });
            }
            return View(users);
        }

        [Authorize(Roles = "Admin,User")]
        public ActionResult Page(string name)
        {
            bool isAllowable = false;
            if (User.IsInRole("Admin"))
            {
                isAllowable = true;
            }
            else
            {
                if (name == User.Identity.GetUserName())
                {
                    isAllowable = true;
                }
            }

            if (isAllowable)
            {
                EditUserViewModelPanel user = applicationUser.Get(name);
                if (applicationUser.Result.Type != ResultType.Success)
                {
                    TempData["result"] = applicationUser.Result;
                    return RedirectToAction("List");
                }
                return View(user);
            }
            return RedirectToAction("Index", "Home", new { area = "" });
        }

        [Authorize(Roles = "Admin,User")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Page(EditUserViewModelPanel model, HttpPostedFileBase ProfileImage)
        {
            if (ModelState.IsValid)
            {
                // Kullanıcı fotoğraf kontrolü
                if (ProfileImage != null)
                {
                    string profileImage = model.Image;
                    if (System.IO.File.Exists(Server.MapPath("~/Images/" + profileImage)) && profileImage != "avatar.jpg")
                    {
                        System.IO.File.Delete(Server.MapPath("~/Images/" + profileImage));
                    }
                    profileImage = ProfileImage.FileName.Substring(0, ProfileImage.FileName.Length - 4) + Guid.NewGuid().ToString() + ProfileImage.FileName.Substring(ProfileImage.FileName.Length - 4, 4);
                    WebImage img = new WebImage(ProfileImage.InputStream);
                    img.Resize(270, 340, true, false);
                    img.Save(Path.Combine(Server.MapPath(@"~/Images"), Path.GetFileName(profileImage)));
                    model.Image = profileImage;
                }
                applicationUser.Edit(model);
                TempData["result"] = applicationUser.Result;
            }
            return Page(model.UserName);
        }

        [Authorize(Roles = "Admin")]
        public JsonResult ChangeState(string id)
        {
            applicationUser.ChangeState(id);
            return Json(new { isSuccess = applicationUser.Result.IsSuccess }, JsonRequestBehavior.AllowGet);
        }

    }
}