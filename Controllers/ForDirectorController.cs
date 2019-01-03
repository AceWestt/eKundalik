using eDnevnik.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace eDnevnik.Controllers
{
    public class ForDirectorController : Controller
    {
        DnevnikEntities db = new DnevnikEntities();
        // GET: ForDirector
        [Authorize]
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult MainPage()
        {
            return View();
        }

        public JsonResult getClass(string level, string letter)
        {
            int lvl = Int32.Parse(level);
            if (Request.IsAjaxRequest())
            {    
                
                var managers = (from m in db.Pupils
                               join c in db.Classes on m.Class equals c.Id
                               where c.Level == lvl && c.Name == letter
                               orderby c.Name
                               select new { m.Id, m.LastName, m.FirstName, m.Phone, m.Parent }).ToList();
                var count = managers.Count();

                var array = new { managers, count };

                return new JsonResult { Data = array, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }

            return new JsonResult
            {
                Data = "error",
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public ActionResult EditFirstTime(int id)
        {
            DnevnikEntities _db = new DnevnikEntities();
            string directorName = HttpContext.User.Identity.Name;
            int directorId = _db.Directors.Where(p => (p.Email.Equals(directorName)) || (p.UserName.Equals(directorName))).FirstOrDefault().Id;
            if (id == directorId)
            {
                Director d = _db.Directors.Find(id);
                return View(d);
            }


            
            return View();
        }

        [HttpPost]
        public ActionResult EditFirstTime(HttpPostedFileBase ImageFile, Director d, string pass, string passConf)
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
                        d.ImagePath = "~/image/directorPhoto/" + fileName;
                        fileName = Path.Combine(Server.MapPath("~/image/directorPhoto/"), fileName);
                        ImageFile.SaveAs(fileName);
                    }
                    var db = new DnevnikEntities();
                    db.Directors.Attach(d);
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

        public ActionResult TimeTableManagement()
        {
            return View();
        }

        public JsonResult getTeachers(string subjectId)
        {
            int id = Int32.Parse(subjectId);
            if (Request.IsAjaxRequest())
            {
                if (id == -1)
                {
                    var managers = db.Teachers.OrderBy(m => m.LastName).Select(m => new { m.LastName, m.FirstName, m.Phone, m.Email, m.Id }).ToList();

                    return new JsonResult { Data = managers, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                }
                else
                {
                    var managers = (from t in db.Teachers
                                    join tsa in db.SubjectTeacherAllocations on t.Id equals tsa.Teacher
                                    join s in db.Subjects on tsa.Subject equals s.Id
                                    where s.Id == id
                                    orderby t.LastName
                                    select new { t.LastName, t.FirstName, t.Id }).ToList();

                    return new JsonResult { Data = managers, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

                }
            }

            return new JsonResult
            {
                Data = "error",
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult getClasses(string level)
        {
            if (Request.IsAjaxRequest())
            {
                int? classlevel = int.Parse(level);
                if (classlevel == null || classlevel == 0)
                {
                    var classes = db.Classes.Select(c => new { c.Id, c.Level, c.Name }).ToList();
                    return new JsonResult { Data = classes, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                }
                else
                {
                    var classes = db.Classes.Where(c => c.Level == classlevel).Select(c => new { c.Id, c.Level, c.Name }).ToList();
                    return new JsonResult { Data = classes, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                }
            }
            return new JsonResult
            {
                Data = "error",
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };

        }

        public JsonResult getSubjects()
        {
            
            if (Request.IsAjaxRequest())
            {
               
                    var managers = db.Subjects.OrderBy(m => m.Level).Select(m => new { m.SubjectName, m.Level, m.Id }).ToList();

                    return new JsonResult { Data = managers, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
               
            }

            return new JsonResult
            {
                Data = "error",
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult getSubjectsByTeacher(string id, string level)
        {
            int teacherId = Int32.Parse(id);
            if (Request.IsAjaxRequest())
            {
                int? classlevel = int.Parse(level);
                
                if(classlevel == null || classlevel == 0)
                {
                    var managers = (from s in db.Subjects
                                    join sa in db.SubjectTeacherAllocations on s.Id equals sa.Subject                                    
                                    where sa.Teacher == teacherId
                                    select new { s.Id, s.Level, s.SubjectName }).ToList();
                    return new JsonResult { Data = managers, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

                }
                else
                {
                   var managers = SubjectList(teacherId).Where(s => s.Level == classlevel).Select(s => new { s.Id, s.Level, s.SubjectName});
                    return new JsonResult { Data = managers, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                };

                

            }

            return new JsonResult
            {
                Data = "error",
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult getTeacherTimeTable(string id, string subjOrd, string day)
        {
            int teacherId;
            int ord;
            int dayNum;
            teacherId = Int32.Parse(id);
            ord = Int32.Parse(subjOrd);
            dayNum = Int32.Parse(day);
            if (Request.IsAjaxRequest())
            {

                var timetable = ifExists(teacherId, dayNum, ord);
                if (timetable != null)
                {
                    var subject = db.Subjects.Where(s => s.Id == timetable.Subject).FirstOrDefault().SubjectName;
                    var classLevel = db.Classes.Where(c => c.Id == timetable.Class).FirstOrDefault().Level;
                    var classLetter = db.Classes.Where(c => c.Id == timetable.Class).FirstOrDefault().Name;
                    var data = new { subject, classLevel, classLetter };
                    return new JsonResult { Data = data, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                }
                else
                {
                    var subject = "--";
                    var classLevel = "-";
                    var classLetter = "-";
                    var data = new { subject, classLevel, classLetter };
                    return new JsonResult { Data = data, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

                }

            }

            return new JsonResult
            {
                Data = "error",
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult teacherTimeTableCreation(string x) {
            int teacherID = Int32.Parse(x);
            if (Request.IsAjaxRequest())
            {

                for (int i = 1; i <= 6; i++) {
                    var tmon = ifExists(teacherID, 1, i);
                    if (tmon == null)
                    {
                        TimeTable tt = new TimeTable();
                        tt.Teacher = teacherID;
                        tt.DayInt = 1;
                        tt.subjectOrder = i;
                        db.TimeTables.Add(tt);
                        db.SaveChanges();
                    }

                    var tue = ifExists(teacherID, 2, i);
                    if (tue == null)
                    {
                        TimeTable tt = new TimeTable();
                        tt.Teacher = teacherID;
                        tt.DayInt = 2;
                        tt.subjectOrder = i;
                        db.TimeTables.Add(tt);
                        db.SaveChanges();
                    }

                    var wed = ifExists(teacherID, 3, i);
                    if (wed == null)
                    {
                        TimeTable tt = new TimeTable();
                        tt.Teacher = teacherID;
                        tt.DayInt = 3;
                        tt.subjectOrder = i;
                        db.TimeTables.Add(tt);
                        db.SaveChanges();
                    }

                    var thu = ifExists(teacherID, 4, i);
                    if (thu == null)
                    {
                        TimeTable tt = new TimeTable();
                        tt.Teacher = teacherID;
                        tt.DayInt = 4;
                        tt.subjectOrder = i;
                        db.TimeTables.Add(tt);
                        db.SaveChanges();
                    }

                    var fri = ifExists(teacherID, 5, i);
                    if (fri == null)
                    {
                        TimeTable tt = new TimeTable();
                        tt.Teacher = teacherID;
                        tt.DayInt = 5;
                        tt.subjectOrder = i;
                        db.TimeTables.Add(tt);
                        db.SaveChanges();
                    }

                    var sat = ifExists(teacherID, 6, i);
                    if (sat == null)
                    {
                        TimeTable tt = new TimeTable();
                        tt.Teacher = teacherID;
                        tt.DayInt = 6;
                        tt.subjectOrder = i;
                        db.TimeTables.Add(tt);
                        db.SaveChanges();
                    }
                }               
                    
                   
                    return new JsonResult { Data = "", JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                

            };

            return new JsonResult
            {
                Data = "error",
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult editTeacherTimeTable(string id, string subjOrd, string subject, string classId)
        {
            if (Request.IsAjaxRequest())
            {
                int tId = int.Parse(id);
                int sId = int.Parse(subject);
                int cId = int.Parse(classId);
                int sOrd = int.Parse(subjOrd);
                TimeTable tb = new TimeTable();
                tb = db.TimeTables.Where(t => t.Teacher == tId && t.subjectOrder == sOrd).FirstOrDefault();
                if(sId == 0)
                {
                    tb.Class = cId;
                    db.Entry(tb).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    ModelState.Clear();
                }
                else if(cId == 0)
                {
                    tb.Subject = sId;
                    db.Entry(tb).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    ModelState.Clear();
                }
                else
                {
                    tb.Subject = sId;
                    tb.Class = cId;
                    db.Entry(tb).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    ModelState.Clear();
                }
                return new JsonResult { Data = "", JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }

            return new JsonResult
            {
                Data = "error",
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

         private TimeTable ifExists(int tId, int dInt, int sO)
        {
            return db.TimeTables.FirstOrDefault(t => t.Teacher == tId && t.DayInt == dInt && t.subjectOrder == sO);
        }

        private ICollection<Subject> SubjectList(int teacherId)
        {
            var subjects = (from s in db.Subjects
                            join sa in db.SubjectTeacherAllocations on s.Id equals sa.Subject
                            where sa.Teacher == teacherId
                            select s).ToList();
            return subjects;
        }

        
    }
}