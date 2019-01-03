using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Security;
using eDnevnik.Models;
 

namespace eDnevnik.Provider
{
    public class CustomMembershipProvider : MembershipProvider
    {
        public override bool ValidateUser(string username, string password)
        {
            bool isValid = false;

            using (DnevnikEntities _db = new DnevnikEntities())
            {
                try
                {
                    Admin admin = (from a in _db.Admins
                                 where a.Email == username || a.UserName == username
                                 select a).FirstOrDefault();
                    SettingsContext context = new SettingsContext();
                    string adminName = (string)context["UserName"];
                    int aID = _db.Admins.Where(a => a.Email.Equals(adminName) || a.UserName.Equals(adminName)).FirstOrDefault().AdminID;

                    if (admin != null && Crypto.VerifyHashedPassword(admin.Password, password))
                    {
                        isValid = true;
                    }

                    Director director = (from d in _db.Directors
                                         where d.Email == username || d.UserName == username
                                         select d).FirstOrDefault();
                    if (director != null && Crypto.VerifyHashedPassword(director.Password, password))
                    {
                        isValid = true;
                    }

                    Parent parent = (from p in _db.Parents
                                     where p.Email == username || p.UserName == username
                                     select p).FirstOrDefault();
                    if (parent != null && Crypto.VerifyHashedPassword(parent.Password, password))
                    {
                        isValid = true;
                    }
                    Teacher teacher = (from t in _db.Teachers
                                   where t.Email == username || t.UserName == username
                                       select t).FirstOrDefault();

                    if (teacher != null && Crypto.VerifyHashedPassword(teacher.Password, password))
                    {
                        isValid = true;
                    }

                    Pupil pupil = (from p in _db.Pupils
                                   where  p.Username == username
                                   select p).FirstOrDefault();

                    string pupName = (string)context["UserName"];
                    int pID = _db.Pupils.Where(p => p.Username.Equals(pupName)).FirstOrDefault().Id;

                    if (pupil != null && Crypto.VerifyHashedPassword(pupil.Password, password))
                    {
                        isValid = true;
                    }

                }
                catch
                {
                    isValid = false;
                }
            }
            return isValid;
        }

        public MembershipUser CreateUser(string email, string password)
        {
            MembershipUser membershipUser = GetUser(email, false);

            if (membershipUser == null)
            {
                try
                {
                    using (DnevnikEntities _db = new DnevnikEntities())
                    {
                        Admin admin = new Admin();
                        admin.Email = email;
                        admin.Password = Crypto.HashPassword(password);
                        //admin.CreationDate = DateTime.Now;

                        if (_db.Roles.Find(1) != null)
                        {
                            admin.RoleId = 1;
                        }

                        if (_db.Roles.Find(2) != null)
                        {
                            admin.RoleId = 2;
                        }

                        _db.Admins.Add(admin);
                        _db.SaveChanges();
                        membershipUser = GetUser(email, false);
                        return membershipUser;

#pragma warning disable CS0162 // Unreachable code detected
                        Director director = new Director();
#pragma warning restore CS0162 // Unreachable code detected
                        director.Email = email;
                        director.Password = Crypto.HashPassword(password);
                        //admin.CreationDate = DateTime.Now;

                        if (_db.Roles.Find(3) != null)
                        {
                            director.RoleId = 3;
                        }

                        _db.Directors.Add(director);
                        _db.SaveChanges();
                        membershipUser = GetUser(email, false);
                        return membershipUser;



#pragma warning disable CS0162 // Unreachable code detected
                        Parent parent = new Parent();
#pragma warning restore CS0162 // Unreachable code detected
                        parent.Email = email;
                        parent.Password = Crypto.HashPassword(password);

                        if(_db.Roles.Find(5) != null)
                        {
                            parent.RoleId = 5;
                        }

                        _db.Parents.Add(parent);
                        _db.SaveChanges();
                        membershipUser = GetUser(email, false);
                        return membershipUser;

#pragma warning disable CS0162 // Unreachable code detected
                        Teacher teacher = new Teacher();
#pragma warning restore CS0162 // Unreachable code detected
                        teacher.Email = email;
                        teacher.Password = Crypto.HashPassword(password);
                        //admin.CreationDate = DateTime.Now;

                        if (_db.Roles.Find(4) != null)
                        {
                            teacher.RoleId = 4;
                        }

                        _db.Teachers.Add(teacher);
                        _db.SaveChanges();
                        membershipUser = GetUser(email, false);
                        return membershipUser;

                        Pupil pupil = new Pupil();
                        pupil.Username = email;
                        pupil.Password = Crypto.HashPassword(password);
                        //admin.CreationDate = DateTime.Now;

                        if (_db.Roles.Find(6) != null)
                        {
                            pupil.RoleId = 6;
                        }

                        _db.Pupils.Add(pupil);
                        _db.SaveChanges();
                        membershipUser = GetUser(email, false);
                        return membershipUser;
                    }
                }
                catch
                {
                    return null;
                }
            }
            return null;
        }

        //public override MembershipUser GetUser(string email, bool userIsOnline)
        //{
        //    try
        //    {
        //        using (UserContext _db = new UserContext())
        //        {
        //            var users = from u in _db.Users
        //                        where u.Email == email
        //                        select u;
        //            if (users.Count() > 0)
        //            {
        //                User user = users.First();
        //                MembershipUser memberUser = new MembershipUser("MyMembershipProvider", user.Email, null, null, null, null,
        //                    false, false, user.CreationDate, DateTime.MinValue, DateTime.MinValue, DateTime.MinValue, DateTime.MinValue);
        //                return memberUser;
        //            }
        //        }
        //    }
        //    catch
        //    {
        //        return null;
        //    }
        //    return null;
        //}

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

        public override bool ChangePassword(string username, string oldPassword, string newPassword)
        {
            throw new NotImplementedException();
        }

        public override bool ChangePasswordQuestionAndAnswer(string username, string password, string newPasswordQuestion, string newPasswordAnswer)
        {
            throw new NotImplementedException();
        }

        public override MembershipUser CreateUser(string username, string password, string email, string passwordQuestion, string passwordAnswer, bool isApproved, object providerUserKey, out MembershipCreateStatus status)
        {
            throw new NotImplementedException();
        }

        public override bool DeleteUser(string username, bool deleteAllRelatedData)
        {
            throw new NotImplementedException();
        }

        public override bool EnablePasswordReset
        {
            get { throw new NotImplementedException(); }
        }
        public override bool EnablePasswordRetrieval
        {
            get { throw new NotImplementedException(); }
        }
        public override MembershipUserCollection FindUsersByEmail(string emailToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            throw new NotImplementedException();
        }
        public override MembershipUserCollection FindUsersByName(string usernameToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            throw new NotImplementedException();
        }
        public override MembershipUserCollection GetAllUsers(int pageIndex, int pageSize, out int totalRecords)
        {
            throw new NotImplementedException();
        }
        public override int GetNumberOfUsersOnline()
        {
            throw new NotImplementedException();
        }
        public override string GetPassword(string username, string answer)
        {
            throw new NotImplementedException();
        }
        public override MembershipUser GetUser(object providerUserKey, bool userIsOnline)
        {
            throw new NotImplementedException();
        }
        public override string GetUserNameByEmail(string email)
        {
            throw new NotImplementedException();
        }
        public override int MaxInvalidPasswordAttempts
        {
            get { throw new NotImplementedException(); }
        }
        public override int MinRequiredNonAlphanumericCharacters
        {
            get { throw new NotImplementedException(); }
        }
        public override int MinRequiredPasswordLength
        {
            get { throw new NotImplementedException(); }
        }
        public override int PasswordAttemptWindow
        {
            get { throw new NotImplementedException(); }
        }
        public override MembershipPasswordFormat PasswordFormat
        {
            get { throw new NotImplementedException(); }
        }
        public override string PasswordStrengthRegularExpression
        {
            get { throw new NotImplementedException(); }
        }
        public override bool RequiresQuestionAndAnswer
        {
            get { throw new NotImplementedException(); }
        }
        public override bool RequiresUniqueEmail
        {
            get { throw new NotImplementedException(); }
        }
        public override string ResetPassword(string username, string answer)
        {
            throw new NotImplementedException();
        }
        public override bool UnlockUser(string userName)
        {
            throw new NotImplementedException();
        }
        public override void UpdateUser(MembershipUser user)
        {
            throw new NotImplementedException();
        }

        public override MembershipUser GetUser(string username, bool userIsOnline)
        {
            throw new NotImplementedException();
        }
    }
}