using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace eDnevnik.Controllers
{
    
    public class ForParentController : Controller
    {
        // GET: ForParent
        public ActionResult Index(int id)
        {
            DnevnikEntities _db = new DnevnikEntities();
            string directorName = HttpContext.User.Identity.Name;
            int directorId = _db.Parents.Where(p => (p.Email.Equals(directorName)) || (p.UserName.Equals(directorName))).FirstOrDefault().Id;
            if (id == directorId)
            {
                ViewBag.Id = directorId;
                Parent d = _db.Parents.Find(id);
                return View(d);
            }



            return View();
        }

        public ActionResult EditFirstTime(int id)
        {
            DnevnikEntities _db = new DnevnikEntities();
            string directorName = HttpContext.User.Identity.Name;
            int directorId = _db.Parents.Where(p => (p.Email.Equals(directorName)) || (p.UserName.Equals(directorName))).FirstOrDefault().Id;
            if (id == directorId)
            {
                Parent d = _db.Parents.Find(id);
                return View(d);
            }



            return View();
        }

        [HttpPost]
        public ActionResult EditFirstTime(HttpPostedFileBase ImageFile, Parent d, string pass, string passConf)
        {
            if (ModelState.IsValid)
            {
                if (pass == passConf)
                {
                    if (ImageFile != null)
                    {
                        string fileName = Path.GetFileNameWithoutExtension(ImageFile.FileName);
                        string fileExtension = Path.GetExtension(ImageFile.FileName);
                        fileName = fileName + DateTime.Now.ToString("yymmssfff") + fileExtension;
                        d.ImagePath = "~/image/teacherPhoto/" + fileName;
                        fileName = Path.Combine(Server.MapPath("~/image/teacherPhoto/"), fileName);
                        ImageFile.SaveAs(fileName);
                    }
                    var db = new DnevnikEntities();
                    db.Parents.Attach(d);
                    d.Password = Crypto.HashPassword(pass);
                    db.Entry(d).Property(p => p.Password).IsModified = true;
                    db.Entry(d).Property(p => p.LastName).IsModified = true;
                    db.Entry(d).Property(p => p.FirstName).IsModified = true;
                    db.Entry(d).Property(p => p.DoB).IsModified = true;
                    db.Entry(d).Property(p => p.Address).IsModified = true;
                    db.Entry(d).Property(p => p.Phone).IsModified = true;
                    db.Entry(d).Property(p => p.Email).IsModified = true;
                    db.Entry(d).Property(p => p.ImagePath).IsModified = true;
                    db.SaveChanges();

                    return RedirectToAction("Index", new { id = d.Id });
                }
                else
                {
                    ViewBag.Pass = "Пароли не совпадают";
                    return View();
                }
            }
            return View();

        }

        public JsonResult getMyChildren(string id)
        {
            int PID = Int32.Parse(id);
            if (Request.IsAjaxRequest())
            {


                DnevnikEntities db = new DnevnikEntities();
                var children = db.Pupils.Where(p => p.Parent == PID).Select(p => new { p.Id, p.LastName, p.FirstName, p.Class }).ToList();
                return new JsonResult { Data = children, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

               
            }

            return new JsonResult
            {
                Data = "error",
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult getClassName(string id)
        {
            int PID = Int32.Parse(id);
            if (Request.IsAjaxRequest())
            {


                DnevnikEntities db = new DnevnikEntities();
                var classes = db.Classes.Where(p => p.Id == PID).Select(p => new { p.Id, p.Level, p.Name }).FirstOrDefault();
                return new JsonResult { Data = classes, JsonRequestBehavior = JsonRequestBehavior.AllowGet };


            }

            return new JsonResult
            {
                Data = "error",
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult getDiaries(string id, string ddate)
        {
            DateTime d = DateTime.Parse(ddate);
            int SID = Int32.Parse(id);
            if (Request.IsAjaxRequest())
            {


                DnevnikEntities db = new DnevnikEntities();
                var diaries = db.Diaries.Where(p => p.Pupil == SID && p.DateOfIssue == d ).Select(p => new { p.DiaryId, p.Attandance, p.DateOfIssue, p.Details, p.Homework, p.Mark, p.Subject }).ToList();
                return new JsonResult { Data = diaries, JsonRequestBehavior = JsonRequestBehavior.AllowGet };


            }

            return new JsonResult
            {
                Data = "error",
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult getSubjectName(string id)
        {
            
            int SID = Int32.Parse(id);
            if (Request.IsAjaxRequest())
            {


                DnevnikEntities db = new DnevnikEntities();
                var subjects = db.Subjects.Where(p => p.Id == SID ).Select(p => new { p.SubjectName }).FirstOrDefault();
                return new JsonResult { Data = subjects, JsonRequestBehavior = JsonRequestBehavior.AllowGet };


            }

            return new JsonResult
            {
                Data = "error",
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

    }
}