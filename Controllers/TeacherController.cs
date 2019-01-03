using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace eDnevnik.Controllers
{
    public class TeacherController : Controller
    {
        // GET: Teacher
        public ActionResult Index(int id)
        {
            DnevnikEntities _db = new DnevnikEntities();
            string directorName = HttpContext.User.Identity.Name;
            int directorId = _db.Teachers.Where(p => (p.Email.Equals(directorName)) || (p.UserName.Equals(directorName))).FirstOrDefault().Id;
            if (id == directorId)
            {
                ViewBag.Id = directorId;
                Teacher d = _db.Teachers.Find(id);
                return View(d);
            }



            return View();
        }

        public ActionResult EditFirstTime(int id)
        {
            DnevnikEntities _db = new DnevnikEntities();
            string directorName = HttpContext.User.Identity.Name;
            int directorId = _db.Teachers.Where(p => (p.Email.Equals(directorName)) || (p.UserName.Equals(directorName))).FirstOrDefault().Id;
            if (id == directorId)
            {
                Teacher d = _db.Teachers.Find(id);
                return View(d);
            }



            return View();
        }

        [HttpPost]
        public ActionResult EditFirstTime(HttpPostedFileBase ImageFile, Teacher d, string pass, string passConf)
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
                    db.Teachers.Attach(d);
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

        public JsonResult getMySubjects(string id)
        {
            int teacherId = Int32.Parse(id);
            if (Request.IsAjaxRequest())
            {


                DnevnikEntities db = new DnevnikEntities();
                    var subjects = (from s in db.Subjects
                                    join sa in db.SubjectTeacherAllocations on s.Id equals sa.Subject
                                    where sa.Teacher == teacherId
                                    select new { s.Id, s.Level, s.SubjectName }).ToList();
                    return new JsonResult { Data = subjects, JsonRequestBehavior = JsonRequestBehavior.AllowGet };


            }

            return new JsonResult
            {
                Data = "error",
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult getMyClasses(string level)
        {
            int l = Int32.Parse(level);
            if (Request.IsAjaxRequest())
            {


                DnevnikEntities db = new DnevnikEntities();
                var classes = db.Classes.Where(c => c.Level == l).OrderBy(c => c.Level).Select(c => new { c.Id, c.Level, c.Name });
                return new JsonResult { Data = classes, JsonRequestBehavior = JsonRequestBehavior.AllowGet };


            }

            return new JsonResult
            {
                Data = "error",
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult getMyStudents(string classId)
        {
            int id = Int32.Parse(classId);
            if (Request.IsAjaxRequest())
            {


                DnevnikEntities db = new DnevnikEntities();
                var group = db.Pupils.Where(c => c.Class == id).OrderBy(c => c.LastName).Select(c => new { c.Id, c.LastName, c.FirstName });
                return new JsonResult { Data = group, JsonRequestBehavior = JsonRequestBehavior.AllowGet };


            }

            return new JsonResult
            {
                Data = "error",
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult addDiary(string tId, string dDate, string sId, string hTS, string dTS, string mark, string subject)
        {
            int teacher = Int32.Parse(tId);
            int student = int.Parse(sId);
            decimal Mark = 0;
            string attendance = " ";
            int Subject = int.Parse(subject);
            if(mark == "-")
            {
                attendance = "-";
            }
            else
            {
                Mark = decimal.Parse(mark);
                attendance = "+";
            }
            DateTime? DDate = DateTime.Parse(dDate);
            if (Request.IsAjaxRequest())
            {
                DnevnikEntities db = new DnevnikEntities();
                var diary = ifExists(DDate, student, Subject);
                if (diary == null)
                {
                    Diary tt = new Diary();
                    tt.Attandance = attendance;
                    tt.DateOfIssue = DDate;
                    tt.Details = dTS;
                    tt.Homework = hTS;
                    tt.Mark = Mark;
                    tt.Pupil = student;
                    tt.Subject = Subject;
                    db.Diaries.Add(tt);
                    db.SaveChanges();
                    return new JsonResult { Data = "success", JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                }
                else { return new JsonResult { Data = "exists", JsonRequestBehavior = JsonRequestBehavior.AllowGet }; }
                


            }

            return new JsonResult
            {
                Data = "error",
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        private Diary ifExists( DateTime? date, int sID, int subj)
        {
            DnevnikEntities db = new DnevnikEntities();
            return db.Diaries.FirstOrDefault(t => t.Subject == subj && t.DateOfIssue == date && t.Pupil == sID);
        }
    }
}