using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using eDnevnik.Models;

namespace eDnevnik.Provider
{
    public class CustomRoleProvider : RoleProvider
    {
        public override string[] GetRolesForUser(string email)
        {
            string[] role = new string[] { };
            using (DnevnikEntities _db = new DnevnikEntities())
            {
                try
                {
                    // Получаем пользователя
                    Admin admin = (from a in _db.Admins
                                 where a.Email == email || a.UserName == email
                                 select a).FirstOrDefault();
                    if (admin != null)
                    {
                        // получаем роль
                        Role adminRole = _db.Roles.Find(admin.RoleId);

                        if (adminRole != null)
                        {
                            role = new string[] { adminRole.RoleName };
                        }
                    }
                }
                catch
                {
                    role = new string[] { };
                }
            }

            using (DnevnikEntities _db = new DnevnikEntities())
            {
                try
                {
                    // Получаем пользователя
                    Director director = (from d in _db.Directors
                                   where d.Email == email || d.UserName == email
                                   select d).FirstOrDefault();
                    if (director != null)
                    {
                        // получаем роль
                        Role directorRole = _db.Roles.Find(director.RoleId);

                        if (directorRole != null)
                        {
                            role = new string[] { directorRole.RoleName };
                        }
                    }
                }
                catch
                {
                    role = new string[] { };
                }
            }

            using (DnevnikEntities _db = new DnevnikEntities())
            {
                try
                {
                    // Получаем пользователя
                    Teacher teacher = (from t in _db.Teachers
                                   where t.Email == email || t.UserName == email
                                       select t).FirstOrDefault();
                    if (teacher != null)
                    {
                        // получаем роль
                        Role teacherRole = _db.Roles.Find(teacher.RoleId);

                        if (teacherRole != null)
                        {
                            role = new string[] { teacherRole.RoleName };
                        }
                    }
                }
                catch
                {
                    role = new string[] { };
                }
            }

            using (DnevnikEntities _db = new DnevnikEntities())
            {
                try
                {
                    // Получаем пользователя
                    Parent parent  = (from p in _db.Parents
                                       where p.Email == email || p.UserName == email
                                       select p).FirstOrDefault();
                    if (parent != null)
                    {
                        // получаем роль
                        Role parentRole = _db.Roles.Find(parent.RoleId);

                        if (parentRole != null)
                        {
                            role = new string[] { parentRole.RoleName };
                        }
                    }
                }
                catch
                {
                    role = new string[] { };
                }
            }

            using (DnevnikEntities _db = new DnevnikEntities())
            {
                try
                {
                    // Получаем пользователя
                    Pupil pupil = (from p in _db.Pupils
                                       where p.Username == email
                                       select p).FirstOrDefault();
                    if (pupil != null)
                    {
                        // получаем роль
                        Role pupRole = _db.Roles.Find(pupil.RoleId);

                        if (pupRole != null)
                        {
                            role = new string[] { pupRole.RoleName };
                        }
                    }
                }
                catch
                {
                    role = new string[] { };
                }
            }



            return role;
        }
        public override void CreateRole(string roleName)
        {
            //Role newRole = new Role() { Name = roleName };
            //UserContext db = new UserContext();
            //db.Roles.Add(newRole);
            //db.SaveChanges();
        }
        public override bool IsUserInRole(string username, string roleName)
        {
            bool outputResult = false;
            //Находим пользователя
            using (DnevnikEntities _db = new DnevnikEntities())
            {
                try
                {
                    // Получаем пользователя
                    Admin admin = (from a in _db.Admins
                                 where a.Email == username || a.UserName == username
                                   select a).FirstOrDefault();
                    Director director = (from d in _db.Directors
                                         where d.Email == username || d.UserName == username
                                         select d).FirstOrDefault();
                    Teacher teacher = (from t in _db.Teachers
                                   where t.Email == username || t.UserName == username
                                       select t).FirstOrDefault();
                    Parent parent = (from p in _db.Parents
                                     where p.Email == username || p.UserName == username
                                     select p).FirstOrDefault();
                    Pupil pup = (from p in _db.Pupils
                                   where p.Username == username
                                 select p).FirstOrDefault();
                    if (admin != null)
                    {
                        // получаем роль
                        Role adminRole = _db.Roles.Find(admin.RoleId);

                        //сравниваем
                        if (adminRole != null && adminRole.RoleName == roleName)
                        {
                            outputResult = true;
                        }
                    }

                    if (director != null)
                    {
                        // получаем роль
                        Role directorRole = _db.Roles.Find(director.RoleId);

                        //сравниваем
                        if (directorRole != null && directorRole.RoleName == roleName)
                        {
                            outputResult = true;
                        }
                    }

                    if (teacher != null)
                    {
                        // получаем роль
                        Role teacherRole = _db.Roles.Find(teacher.RoleId);

                        //сравниваем
                        if (teacherRole != null && teacherRole.RoleName == roleName)
                        {
                            outputResult = true;
                        }
                    }

                    if (parent != null)
                    {
                        // получаем роль
                        Role parentRole = _db.Roles.Find(parent.RoleId);

                        //сравниваем
                        if (parentRole != null && parentRole.RoleName == roleName)
                        {
                            outputResult = true;
                        }
                    }

                    if (pup != null)
                    {
                        // получаем роль
                        Role pupRole = _db.Roles.Find(pup.RoleId);

                        //сравниваем
                        if (pupRole != null && pupRole.RoleName == roleName)
                        {
                            outputResult = true;
                        }
                    }

                }
                catch
                {
                    outputResult = false;
                }
            }
            return outputResult;
        }
        public override void AddUsersToRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }

        public override string ApplicationName
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
        {
            throw new NotImplementedException();
        }

        public override string[] FindUsersInRole(string roleName, string usernameToMatch)
        {
            throw new NotImplementedException();
        }

        public override string[] GetAllRoles()
        {
            throw new NotImplementedException();
        }

        public override string[] GetUsersInRole(string roleName)
        {
            throw new NotImplementedException();
        }

        public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }

        public override bool RoleExists(string roleName)
        {
            throw new NotImplementedException();
        }
    }
}