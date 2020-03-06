using Budget_System.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Budget_System.Controllers
{
    public class AccountController : Controller
    {
        private MISEntities db = new MISEntities();
        // GET: Account
        public ActionResult Login(string type)
        {
            //if (type == null || string.IsNullOrEmpty(type))
            //    return View();
            //else
            //{
            // 登出，清除session
            FormsAuthentication.SignOut();
            System.Web.HttpContext.Current.Session["account"] = null;
            return View();
            //return RedirectToAction("Login");
            //}
        }

        [HttpPost]
        public ActionResult Login(FormCollection post)
        {
            string account = post["account"];
            string password = post["password"];

            //驗證帳號密碼
            if (new AccountModel().Check_USER(account, password))
            {
                // Session預設有效時間是20分鐘 
                Session["account"] = account;
                Session["Deptno"] = new AccountModel().Dept(account).Deptno;
                return RedirectToAction("Index", "Home");

                //FormsAuthentication.SetAuthCookie(account, false);//將用戶名放入Cookie中
                //System.Web.HttpContext.Current.Session["account"] = account; //將用戶名放入session中

                // 會重複要求再登一次 
                //Response.Redirect("~/Account/Test");
                //return new EmptyResult();
            }
            else
            {
                ViewBag.Msg = "登入失敗...";
                return View();
            }
        }

        public ActionResult ACreate()
        {
            var DeptList = db.Dept.ToList();
            ViewBag.Dept = DeptList;

            return View(new AccountModel());
        }

        [HttpPost]
        public ActionResult ACreate(AccountModel data)
        {
            if (string.IsNullOrWhiteSpace(data.password1) || data.password1 != data.password2)
            {
                ViewBag.Msg = "密碼輸入錯誤";
                return View(data);
            }
            else
            {
                if (CreateUSER(data))
                {
                    Response.Redirect("~/Account/Login");
                    return new EmptyResult();
                }
                else
                {
                    ViewBag.Msg = "註冊失敗...";
                    return View(data);
                }
            }
        }

        public bool CreateUSER(AccountModel data)
        {
            try
            {
                // save
                Person tuser = new Person();
                tuser.Penno = data.Penno;
                tuser.Deptno = data.Deptno;
                tuser.Pname = data.Pname;
                tuser.password = data.password1;

                new AccountModel().Create_USER(tuser);
                return true;
            }
            catch
            {
                return false;
            }
        }

    }
}