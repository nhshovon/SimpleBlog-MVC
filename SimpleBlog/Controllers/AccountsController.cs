using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using SimpleBlog.Models.ViewModels;
using WebMatrix.WebData;

namespace SimpleBlog.Controllers
{
    [AllowAnonymous]
    public class AccountsController : Controller
    {
        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterVm model)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMsg"] = ModelState.ToString();
                return View(model);
            }

            if (WebSecurity.UserExists(model.Email))
            {
                TempData["ErrorMsg"] = "Username already in user";
                return View(model);
            }

            try
            {
                var result = WebSecurity.CreateUserAndAccount(model.Email, model.Password,
                    new
                    {
                        Email = model.Email,
                        FullName = model.FullName,
                        Gender = model.Gender,
                        ProfileImageUrl = model.ProfileImageLocation,
                        DateOfBirth = DateTime.Today
                    },false);

                if (!Roles.RoleExists("User"))
                {
                    Roles.CreateRole("User");
                }
                Roles.AddUserToRole(model.Email,"User");

                //Send Email From here
                ModelState.Clear();
                TempData["SuccessMsg"] = "User Created Successfully. Please <a href='/Accounts/Login'>Login</a>";
                return View();
            }
            catch (Exception e)
            {
                TempData["ErrorMsg"] = e.ToString();
                return View();
            }
        }

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginVm model)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMsg"] = ModelState.ToString();
                return View(model);
            }

            if (WebSecurity.Login(model.Email,model.Password))
            {
                return RedirectToAction("Index","Dashboard");
            }
            else
            {
                TempData["ErrorMsg"] = "Invalid Login attempt";
                return View();
            }
        }

        public ActionResult SignOut()
        {
            WebSecurity.Logout();
            return RedirectToAction("Login","Accounts");
        }
    }
}