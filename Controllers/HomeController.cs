using eDnevnik.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Security;

namespace eDnevnik.Controllers
{
    [AllowAnonymous]
    public class HomeController : Controller
    {
        public ActionResult Index(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }
        [HttpPost]
        public ActionResult Index(LogOnModel model, string returnUrl, string password)
        {

            //if (ModelState.IsValid)
            //{
            //    if (Membership.ValidateUser(model.UserName, model.Password))
            //    {
            //        FormsAuthentication.SetAuthCookie(model.UserName, model.RememberMe);
            //        if (Url.IsLocalUrl(returnUrl))
            //        {
            //            return Redirect(returnUrl);
            //        }
            //        else
            //        {
            //            return RedirectToAction("Index", "Home");
            //        }
            //    }
            //    else
            //    {
            //        ModelState.AddModelError("", "Неправильный пароль или логин");
            //    }
            //}

            if (ModelState.IsValid)
            {

                // поиск пользователя в бд
                Admin admin = null;
                Director director = null;
                Teacher teacher = null;
                Parent parent = null;
                Pupil pupil = null;
                using (DnevnikEntities db = new DnevnikEntities())
                {
                    admin = db.Admins.FirstOrDefault(a => a.Email == model.UserName || a.UserName == model.UserName);
                    director = db.Directors.FirstOrDefault(d => d.Email == model.UserName || d.UserName == model.UserName);                    
                    teacher = db.Teachers.FirstOrDefault(t => t.Email == model.UserName || t.UserName == model.UserName);
                    parent = db.Parents.FirstOrDefault(p => p.Email == model.UserName || p.UserName == model.UserName);
                    pupil = db.Pupils.FirstOrDefault(p => p.Username == model.UserName);

                }
                if (admin != null && Crypto.VerifyHashedPassword(admin.Password, model.Password))
                {
                    FormsAuthentication.SetAuthCookie(model.UserName, true);
                    return RedirectToAction("Index", "Admin");
                }
                else
                {
                    ModelState.AddModelError("", "Пользователя с таким логином и паролем нет");
                }

                if (director != null && Crypto.VerifyHashedPassword(director.Password, model.Password))
                {
                    if (Crypto.VerifyHashedPassword(director.Password, "1111"))
                    {
                        FormsAuthentication.SetAuthCookie(model.UserName, true);
                        return RedirectToAction("EditFirstTime", "ForDirector", new { id = director.Id });
                    }
                    else
                    {
                        FormsAuthentication.SetAuthCookie(model.UserName, true);
                        return RedirectToAction("Index", "ForDirector", new { id = director.Id });
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Пользователя с таким логином и паролем нет");
                }

                if (teacher != null && Crypto.VerifyHashedPassword(teacher.Password, model.Password))
                {
                    if (Crypto.VerifyHashedPassword(teacher.Password, "1111"))
                    {
                        FormsAuthentication.SetAuthCookie(model.UserName, true);
                        return RedirectToAction("EditFirstTime", "Teacher", new { id = teacher.Id }); }
                    else
                    {
                        FormsAuthentication.SetAuthCookie(model.UserName, true);
                        return RedirectToAction("Index", "Teacher", new { id = teacher.Id });
                    }

                }
                else
                {
                    ModelState.AddModelError("", "Пользователя с таким логином и паролем нет");
                }

                if (parent != null && Crypto.VerifyHashedPassword(parent.Password, model.Password))
                {
                    if (Crypto.VerifyHashedPassword(parent.Password, "1111"))
                    {
                        FormsAuthentication.SetAuthCookie(model.UserName, true);
                        return RedirectToAction("EditFirstTime", "ForParent", new { id = parent.Id });
                    }
                    else
                    {
                        FormsAuthentication.SetAuthCookie(model.UserName, true);
                        return RedirectToAction("Index", "ForParent", new { id = parent.Id });
                    }

                }
                else
                {
                    ModelState.AddModelError("", "Пользователя с таким логином и паролем нет");
                }

            }


            return View(model);
        }
        public ActionResult LogOff()
        {
            FormsAuthentication.SignOut();

            return RedirectToAction("Login", "Home");
        }

        public ActionResult Register()
        {
            return View();
        }

        //[HttpPost]
        //public ActionResult Register(RegisterModel model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        MembershipUser membershipUser = ((CustomMembershipProvider)Membership.Provider).CreateUser(model.Email, model.Password);

        //        if (membershipUser != null)
        //        {
        //            FormsAuthentication.SetAuthCookie(model.Email, false);
        //            return RedirectToAction("Index", "Home");
        //        }
        //        else
        //        {
        //            ModelState.AddModelError("", "Ошибка при регистрации");
        //        }
        //    }
        //    return View(model);
        //}

        [Authorize(Roles = "Admin")]

        public ActionResult AdminPage()
        {
            return View();
        }

        
       
    }
}