using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Budget_System.Models
{
    public class AccountModel
    {
        private MISEntities db = new MISEntities();

        public string Penno { get; set; }
        public string Deptno { get; set; }
        public string Pname { get; set; }
        public string password1 { get; set; }
        public string password2 { get; set; }

        public void Create_USER(Person tuser)
        {
            db.Person.Add(tuser);
            db.SaveChanges();
        }

        public bool Check_USER(string account, string password)
        {
            var data = db.Person.Where(o => o.Penno == account && o.password == password).Select(s => s).ToList();
            if (data.Count > 0)
                return true;
            else
                return false;
        }
        public Person Dept(string account)
        {
            var data = db.Person.Where(o => o.Penno == account).SingleOrDefault();

            return data;
        }
        //public string Find_id()
        //{
        //    // 失敗
        //    //var id = db.TUSER.Max(o => o != null ? o.id : 0);
        //    // 如果null就給0，else Max
        //    var Penno = db.Person.Select(s => Convert.ToInt32(s.Penno)).DefaultIfEmpty(0).Max();
        //    //var id = db.Person.Select(s => s.Penno).DefaultIfEmpty(0).Max();

        //    return Penno.ToString();
        //}
    }
}