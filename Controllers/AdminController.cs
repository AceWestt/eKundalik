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
   [Authorize(Roles ="Admin")]
    public class AdminController : Controller
    {
        DnevnikEntities db = new DnevnikEntities();
        // GET: Admin
        [Authorize]
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult getAllManagers()
        {
            if (Request.IsAjaxRequest())
            {
                var managers = db.Directors.OrderBy(m => m.LastName).Select(m => new { m.LastName, m.FirstName, m.Phone, m.Email, m.Id}).ToList();

                return new JsonResult { Data = managers, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }

            return new JsonResult
            {
                Data = "error",
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult getAllSubjects()
        {
            if (Request.IsAjaxRequest())
            {
                var managers = db.Subjects.OrderBy(m => m.Level).Select(m => new { m.SubjectName, m.Level, m.Id}).ToList();

                return new JsonResult { Data = managers, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }

            return new JsonResult
            {
                Data = "error",
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }



        public JsonResult managerDetails(string id)
        {
            int X = Int32.Parse(id);
           
            if (Request.IsAjaxRequest())
            {
                var manager = (from d in db.Directors
                               where d.Id == X
                               select new { d.ImagePath, d.Phone, d.Email, d.Address }).FirstOrDefault();

                return new JsonResult { Data = manager, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }

            return new JsonResult
            {
                Data = "error",
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult managerEditDetails(string id)
        {
            int X = Int32.Parse(id);

            if (Request.IsAjaxRequest())
            {
                var manager = (from d in db.Directors
                               where d.Id == X
                               select new { d.UserName, d.Password, d.LastName, d.FirstName, d.Phone, d.Email, d.Address }).FirstOrDefault();

                return new JsonResult { Data = manager, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }

            return new JsonResult
            {
                Data = "error",
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        [HttpPost]
        public JsonResult addManager(string UserName, string Password, string LastName,
            string FirstName, string Address, string Phone, string Email)
        {
            
           
                using (DnevnikEntities db = new DnevnikEntities())
                {
                    Director d = new Director();
                    d.UserName = UserName;
                    d.Password = Crypto.HashPassword(Password);
                    d.LastName = LastName;
                    d.FirstName = FirstName;
                    d.Address = Address;
                    d.Phone = Phone;
                    d.Email = Email;
                    db.Directors.Add(d);
                    db.SaveChanges();
                    ModelState.Clear();
                    


                    
                    return Json( d, JsonRequestBehavior.AllowGet );

                }
         
        }

        [HttpPost]
        public JsonResult addSubject(string SubjectName, string Level)
        {

            int l = int.Parse(Level);
            using (DnevnikEntities db = new DnevnikEntities())
            {
                Subject d = new Subject();
                d.SubjectName = SubjectName;
                d.Level = l;
                
                db.Subjects.Add(d);
                db.SaveChanges();
                ModelState.Clear();




                return Json(d, JsonRequestBehavior.AllowGet);

            }

        }

        [HttpPost]
        public JsonResult editManager(string id, string UserName, string Password, string LastName,
            string FirstName, string Address, string Phone, string Email)
        {

            int X = Int32.Parse(id);            
            using (DnevnikEntities db = new DnevnikEntities())
            {
                Director d = db.Directors.Find(X);
                d.UserName = UserName;
                d.Password = Crypto.HashPassword(Password); 
                d.LastName = LastName;
                d.FirstName = FirstName;
                d.Address = Address;
                d.Phone = Phone;
                d.Email = Email;
                d.RoleId = 2;
                db.Entry(d).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                ModelState.Clear();




                return Json("sucess", JsonRequestBehavior.AllowGet);

            }

        }

        [HttpPost]
        public JsonResult deleteManager(string id)
        {

            int X = Int32.Parse(id);
            using (DnevnikEntities db = new DnevnikEntities())
            {
                Director d = db.Directors.Find(X);
                db.Entry(d).State = System.Data.Entity.EntityState.Deleted;
                db.SaveChanges();
                ModelState.Clear();




                return Json(d, JsonRequestBehavior.AllowGet);

            }

        }

        public JsonResult getAllTeachers()
        {
            if (Request.IsAjaxRequest())
            {
                var managers = db.Teachers.OrderBy(m => m.LastName).Select(m => new { m.LastName, m.FirstName, m.Phone, m.Email, m.Id }).ToList();

                return new JsonResult { Data = managers, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }

            return new JsonResult
            {
                Data = "error",
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult teacherDetails(string id)
        {
            int X = Int32.Parse(id);

            if (Request.IsAjaxRequest())
            {
                var manager = (from d in db.Teachers
                               where d.Id == X
                               select new { d.ImagePath, d.Phone, d.Email, d.Address }).FirstOrDefault();

                return new JsonResult { Data = manager, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }

            return new JsonResult
            {
                Data = "error",
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        [HttpPost]

        public JsonResult teacherEditDetails(string id)
        {
            int X = Int32.Parse(id);

            if (Request.IsAjaxRequest())
            {
                var manager = (from d in db.Teachers
                               where d.Id == X
                               select new { d.UserName, d.Password, d.LastName, d.FirstName, d.Phone, d.Email, d.Address }).FirstOrDefault();

                return new JsonResult { Data = manager, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }

            return new JsonResult
            {
                Data = "error",
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        [HttpPost]

        public JsonResult subjectEditDetails(string id)
        {
            int X = Int32.Parse(id);

            if (Request.IsAjaxRequest())
            {
                var manager = (from d in db.Subjects
                               where d.Id == X
                               select new { d.SubjectName, d.Level }).FirstOrDefault();

                return new JsonResult { Data = manager, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }

            return new JsonResult
            {
                Data = "error",
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        [HttpPost]
        public JsonResult editTeacher(string id, string UserName, string Password, string LastName,
            string FirstName, string Address, string Phone, string Email)
        {

            int X = Int32.Parse(id);
            using (DnevnikEntities db = new DnevnikEntities())
            {
                Teacher d = db.Teachers.Find(X);
                d.UserName = UserName;
                d.Password = Crypto.HashPassword(Password);
                d.LastName = LastName;
                d.FirstName = FirstName;
                d.Address = Address;
                d.Phone = Phone;
                d.Email = Email;
                d.RoleId = 4;
                db.Entry(d).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                ModelState.Clear();




                return Json("", JsonRequestBehavior.AllowGet);

            }

        }

        [HttpPost]
        public JsonResult editSubject(string id, string SubjectName, string Level)
        {

            int X = Int32.Parse(id);
            int level = int.Parse(Level);
            using (DnevnikEntities db = new DnevnikEntities())
            {
                Subject d = new Subject();
                d = db.Subjects.Find(X);
                db.Subjects.Attach(d);
                d.SubjectName = SubjectName;
                d.Level = level;
                db.Entry(d).Property(p => p.Level).IsModified = true;
                db.Entry(d).Property(p => p.SubjectName).IsModified = true;
                db.SaveChanges();
                ModelState.Clear();




                return Json("", JsonRequestBehavior.AllowGet);

            }

        }

        [HttpPost]
        public JsonResult deleteSubject(string id)
        {

            int X = Int32.Parse(id);
            using (DnevnikEntities db = new DnevnikEntities())
            {
                Subject d = db.Subjects.Find(X);
                db.Entry(d).State = System.Data.Entity.EntityState.Deleted;
                db.SaveChanges();
                ModelState.Clear();




                return Json(d, JsonRequestBehavior.AllowGet);

            }

        }

        [HttpPost]
        public JsonResult deleteTeacher(string id)
        {

            int X = Int32.Parse(id);
            using (DnevnikEntities db = new DnevnikEntities())
            {
                Teacher d = db.Teachers.Find(X);
                db.Entry(d).State = System.Data.Entity.EntityState.Deleted;
                db.SaveChanges();
                ModelState.Clear();




                return Json(d, JsonRequestBehavior.AllowGet);

            }

        }

        [HttpPost]
        public JsonResult addTeacher(string UserName, string Password, string LastName,
            string FirstName, string Address, string Phone, string Email)
        {


            using (DnevnikEntities db = new DnevnikEntities())
            {
                Teacher d = new Teacher();
                d.UserName = UserName;
                d.Password = Crypto.HashPassword(Password);
                d.LastName = LastName;
                d.FirstName = FirstName;
                d.Address = Address;
                d.Phone = Phone;
                d.Email = Email;
                d.RoleId = 4; 
                db.Teachers.Add(d);
                db.SaveChanges();
                ModelState.Clear();




                return Json(d, JsonRequestBehavior.AllowGet);

            }

        }

        [HttpPost]
        public JsonResult addManyTeachers(string UserName, string Amount)
        {
            int x = Int32.Parse(Amount);
            string uName = "";

            using (DnevnikEntities db = new DnevnikEntities())
            {
                Teacher d = new Teacher();

                for (int i = 1; i <= x; i++ )
                {                    
                    uName = UserName + x.ToString();
                    d.UserName = uName;
                    d.Password = Crypto.HashPassword("12345");
                    d.RoleId = 4;
                    db.Teachers.Add(d);
                    db.SaveChanges();
                    ModelState.Clear();                    
                }

                return Json(d, JsonRequestBehavior.AllowGet);

            }

        }

        public JsonResult getAllParents()
        {
            if (Request.IsAjaxRequest())
            {
                var managers = db.Parents.OrderBy(m => m.LastName).Select(m => new { m.LastName, m.FirstName, m.Phone, m.Email, m.Id }).ToList();

                return new JsonResult { Data = managers, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }

            return new JsonResult
            {
                Data = "error",
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult parentDetails(string id)
        {
            int X = Int32.Parse(id);

            if (Request.IsAjaxRequest())
            {
                var manager = (from d in db.Parents
                               where d.Id == X
                               select new { d.ImagePath, d.Phone, d.Email, d.Address }).FirstOrDefault();

                return new JsonResult { Data = manager, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }

            return new JsonResult
            {
                Data = "error",
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult parentEditDetails(string id)
        {
            int X = Int32.Parse(id);

            if (Request.IsAjaxRequest())
            {
                var manager = (from d in db.Parents
                               where d.Id == X
                               select new { d.UserName, d.Password, d.LastName, d.FirstName, d.Phone, d.Email, d.Address }).FirstOrDefault();

                return new JsonResult { Data = manager, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }

            return new JsonResult
            {
                Data = "error",
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        [HttpPost]
        public JsonResult editParent(string id, string UserName, string Password, string LastName,
           string FirstName, string Address, string Phone, string Email)
        {

            int X = Int32.Parse(id);
            using (DnevnikEntities db = new DnevnikEntities())
            {
                Parent d = db.Parents.Find(X);
                d.UserName = UserName;
                d.Password = Crypto.HashPassword(Password);
                d.LastName = LastName;
                d.FirstName = FirstName;
                d.Address = Address;
                d.Phone = Phone;
                d.Email = Email;
                d.RoleId = 5;
                db.Entry(d).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                ModelState.Clear();




                return Json(d, JsonRequestBehavior.AllowGet);

            }

        }

        [HttpPost]
        public JsonResult deleteParent(string id)
        {

            int X = Int32.Parse(id);
            using (DnevnikEntities db = new DnevnikEntities())
            {
                Parent d = db.Parents.Find(X);
                db.Entry(d).State = System.Data.Entity.EntityState.Deleted;
                db.SaveChanges();
                ModelState.Clear();




                return Json(d, JsonRequestBehavior.AllowGet);

            }

        }

        [HttpPost]
        public JsonResult addParent(string UserName, string Password, string LastName,
            string FirstName, string Address, string Phone, string Email)
        {


            using (DnevnikEntities db = new DnevnikEntities())
            {
                Parent d = new Parent();
                d.UserName = UserName;
                d.Password = Crypto.HashPassword(Password);
                d.LastName = LastName;
                d.FirstName = FirstName;
                d.Address = Address;
                d.Phone = Phone;
                d.Email = Email;
                d.RoleId = 5;
                db.Parents.Add(d);
                db.SaveChanges();
                ModelState.Clear();




                return Json(d, JsonRequestBehavior.AllowGet);

            }

        }

        [HttpPost]
        public JsonResult addManyParents(string UserName, string Amount, string ClassId)
        {
            int x = Int32.Parse(Amount);
            string uName = "";
            int y = Int32.Parse(ClassId);

            using (DnevnikEntities db = new DnevnikEntities())
            {
                Parent d = new Parent();
                Pupil p = new Pupil();

                for (int i = 1; i <= x; i++)
                {
                    uName = UserName + i.ToString();
                    d.UserName = uName;
                    d.Password = Crypto.HashPassword("1111");
                    d.RoleId = 5;

                    p.Username = uName + "child";
                    p.Parent = d.Id;
                    p.Class = y;
                    p.RoleId = 6;
                    p.Password = Crypto.HashPassword("1111");

                    db.Pupils.Add(p);
                    db.Parents.Add(d);
                    db.SaveChanges();
                    ModelState.Clear();
                }

                return Json(d, JsonRequestBehavior.AllowGet);

            }

        }

        public JsonResult getAllClasses()
        {
            if (Request.IsAjaxRequest())
            {
                var managers = db.Classes.OrderBy(m => m.Level).Select(m => new { m.Name, m.Level, m.Teacher, m.Id}).ToList();             

                return new JsonResult { Data = managers, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }

            return new JsonResult
            {
                Data = "error",
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult classEditDetails(string id)
        {
            int X = Int32.Parse(id);

            if (Request.IsAjaxRequest())
            {
                var manager = (from d in db.Classes
                               where d.Id == X
                               select new { d.Name, d.Level, d.Teacher }).FirstOrDefault();

                return new JsonResult { Data = manager, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }

            return new JsonResult
            {
                Data = "error",
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        [HttpPost]
        public JsonResult editClass(string id, string Name, string Level, string Teacher)
        {

            int X = Int32.Parse(id);
            int teach = Int32.Parse(Teacher);
            int lvl = Int32.Parse(Level);
            using (DnevnikEntities db = new DnevnikEntities())
            {
                Class d = db.Classes.Find(X);
                d.Name = Name;
                d.Level = lvl;
                d.Teacher = teach;               
                db.Entry(d).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                ModelState.Clear();

                return Json(d, JsonRequestBehavior.AllowGet);

            }

        }

        [HttpPost]
        public JsonResult deleteClass(string id)
        {

            int X = Int32.Parse(id);
            using (DnevnikEntities db = new DnevnikEntities())
            {
                Class d = db.Classes.Find(X);
                db.Entry(d).State = System.Data.Entity.EntityState.Deleted;
                db.SaveChanges();
                ModelState.Clear();

                return Json(d, JsonRequestBehavior.AllowGet);

            }

        }

        [HttpPost]
        public JsonResult addClass(string Name, string Level, string Teacher)
        {
            int lvl = Int32.Parse(Level);
            int teach = Int32.Parse(Teacher);

            using (DnevnikEntities db = new DnevnikEntities())
            {
                Class d = new Class();
                d.Name = Name;
                d.Level = lvl;
                d.Teacher = teach;               
                db.Classes.Add(d);
                db.SaveChanges();
                ModelState.Clear();

                return Json(d, JsonRequestBehavior.AllowGet);

            }

        }

        [HttpPost]
        public JsonResult addManyClasses(string Name, string Amount)
        {
            int lvl = Int32.Parse(Amount);
            
           
            using (DnevnikEntities db = new DnevnikEntities())
            {
                Class d = new Class();

                for (int i = 1; i <= lvl; i++)
                {
                    d.Name = Name;
                    d.Level = i;
                    db.Classes.Add(d);
                    db.SaveChanges();
                    ModelState.Clear();
                }

                   

                return Json(d, JsonRequestBehavior.AllowGet);

            }

        }
    }
}