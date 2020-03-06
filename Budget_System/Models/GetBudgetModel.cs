using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;

namespace Budget_System.Models
{
    public class GetBudgetModel
    {
        private DataSet Ds = new DataSet();
        private SqlDataAdapter Ad = null;
        private MISEntities db = new MISEntities();


        public class Bugdaselect
        {
            public string Bno { get; set; }
            public string Month { get; set; }
            public string Total { get; set; }
            public string Uid { get; set; }
            public string Dept { get; set; }
            public string Sctrl { get; set; }
            public string Lev { get; set; }
        }

        public class Member
        {
            public string Bno { get; set; }
            public string Dept { get; set; }
            public string Bugda { get; set; }
            public string Bugna { get; set; }
            public int Total { get; set; }
            public string Puid { get; set; }
            public string Uid { get; set; }
        }

        //public List<Budget_t> getList()
        //{
        //    var list = db.Budget_t.Where(s => s.Sctrl == "N").ToList();
        //    return list;
        //}
        public List<Member> getList2(string Uid)
        {
            List<Member> List2 = new List<Member>();

            List2 = (from a in db.Budget_Bugda
                     join b in db.Bugda on a.Bugda equals b.Bugda1
                     where a.Puid.ToString() == Uid
                     select new Member
                     {
                         Bugda = a.Bugda,
                         Bugna = b.Bugna,
                         Total = a.Total,
                         Puid = a.Puid.ToString(),
                         Uid = a.Uid.ToString()
                     }).ToList();



            //List2 = db.Budget_Bugda.
            //    Where(o => o.Puid.ToString() == Uid).
            //    Select(c => new Member { Bugda = c.Bugda, Total = c.Total, Uid = c.Puid.ToString() }).OrderBy(s => s.Bugda).ToList();

            

            if (List2.Count == 0)
            {
                var quary = List2.Where(a => 1 == 0).ToList();
                return quary;
            }
            else
            {
                var quary1 = List2.OrderBy(o => o.Bugda).ToList();
                return quary1;
            }
        }

        public DataSet 查詢()
        {
            // get ConnectionString
            SqlDataSource comm = new SqlDataSource();
            comm.ConnectionString = System.Web.Configuration.WebConfigurationManager.ConnectionStrings["MISEntities"].ConnectionString;

            string aa = comm.ConnectionString.ToString();
            string comm1 = aa.Substring(aa.IndexOf("data source"), aa.Length - aa.IndexOf("data source") - 1);

            Ad = new SqlDataAdapter("預算查詢", comm1);
            Ad.SelectCommand.CommandType = CommandType.StoredProcedure;
            Ad.SelectCommand.CommandTimeout = 600;
            Ad.SelectCommand.Parameters.Clear();
            Ds.Clear();
            Ad.Fill(Ds);

            return Ds;
        }

        public string Bno()
        {
            var data = from o in db.Budget_t
                       orderby o.Bno descending
                       select o.Bno;

            string Bno = "";
            string date = DateTime.Now.ToString("yyyyMMdd");

            if (data.Count() > 1)
            {
                Bno = data.First();
                if ("20" + Bno.Substring(0, 6) == date)
                    Bno = (Convert.ToInt32(Bno) + 1).ToString();
                else
                    Bno = date.Substring(2, 6) + "001";
            }
            else
                Bno = date.Substring(0, 6) + "001";

            return Bno;
        }

        public int Dept(string Dept)
        {
            var data = (from o in db.Dept
                        where o.DeptNo == Dept
                        select o).Count();

            return data;
        }


        public int Check(string Dept, string Month)
        {
            var data = (from o in db.Budget_t
                        where o.Dept == Dept && o.Month == Month
                        select o).Count();

            return data;
        }

        public class Bugdalist
        {
            public string Dept { get; set; }
            public string Month { get; set; }
            public string Bugda { get; set; }
            public string Bugna { get; set; }
            public int Total { get; set; }
        }

        public List<Bugdalist> getCrlist(string BGDEP, string BGYM)
        {
            var data = db.Bugda.Select(o =>
                       new Bugdalist { Dept = BGDEP, Month = BGYM, Bugda = o.Bugda1, Bugna = o.Bugna, Total = 0 }).
                       OrderBy(o => o.Bugda).ToList();

            return data;
        }

        public class Bugdalist2
        {
            public string Dept { get; set; }
            public string Month { get; set; }
            public string Bugda { get; set; }
            public string Bugna { get; set; }
            public string Puid { get; set; }
            public string Uid { get; set; }
            public int Total { get; set; }
        }

        public List<Bugdalist2> getCrlist2(string Bno)
        {
            var data = db.Budget_t.Where(s => s.Bno == Bno).Join(db.Budget_Bugda,
                       t => t.Uid,
                       p => p.Puid,
                       (da1, da2) => new
                       {
                           da1.Dept,
                           da1.Month,
                           da2.Bugda,
                           da2.Total,
                           da2.Puid,
                           da2.Uid
                       }).Join(db.Bugda,
                       t => t.Bugda,
                       p => p.Bugda1,
                       (da1, da2) => new Bugdalist2
                       {
                           Dept = da1.Dept,
                           Month = da1.Month,
                           Bugda = da1.Bugda,
                           Bugna = da2.Bugna,
                           Total = da1.Total,
                           Puid = da1.Puid.ToString(),
                           Uid = da1.Uid.ToString()
                       }).OrderBy(o => o.Bugda).ToList();

            return data;
        }

        public class Itemslist
        {
            public string Bugda { get; set; }
            public string Items { get; set; }
            public string Remark { get; set; }
            public decimal Price { get; set; }
            public int Amount { get; set; }
            public int? Total { get; set; }
            public string Puid { get; set; }
            public string Uid { get; set; }
            public string sctrl { get; set; }
        }

        public List<Itemslist> getItems(string Puid)
        {
            // get Items
            //var data = db.Budget_Items.Where(o => o.Puid.ToString() == Puid).OrderBy(s => s.Items).ToList();


            var data = db.Budget_Items.Where(o => o.Puid.ToString() == Puid && o.sctrl != "X").Join(db.Budget_Bugda,
                       t => t.Puid,
                       p => p.Uid,
                       (da1, da2) => new Itemslist
                       {
                           Bugda = da2.Bugda,
                           Items = da1.Items,
                           Remark = da1.Remark,
                           Price = da1.Price,
                           Amount = da1.Amount,
                           Total = da1.Total,
                           Puid = da1.Puid.ToString(),
                           Uid = da1.Uid.ToString(),
                           sctrl = da1.sctrl.ToString()
                       }).OrderBy(o => o.Items).ToList();
            return data;
        }

        public void DeleteAllItems(Guid Puid)
        {
            //Budget_Items items = db.Budget_Items.Find(Puid);

            //Budget_Items items = db.Budget_Items.Where(o => o.Puid == Puid);
            //db.Budget_Items.Remove(items);
            //db.SaveChanges();
        }

        public void ItemeCreate(Budget_Items items)
        {
            db.Budget_Items.Add(items);
            db.SaveChanges();

            string Puid = items.Puid.ToString();


        }

        public void ItemeUpdate(Budget_Items items)
        {
            db.Entry(items).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
        }

        public void UpdateBudget(string Puid)
        {
            //var data = db.Budget_Bugda.Where(o => o.Uid == new Guid(Puid))



        }


        //public void ItemeDelete(Budget_Items items2)
        //{
        //    Budget_Items items = db.Budget_Items.Find(items2.Uid);
        //    db.Budget_Items.Remove(items);
        //    db.SaveChanges();
        //}
        public void Create(Budget_t BudgetNo)
        {
            db.Budget_t.Add(BudgetNo);
            db.SaveChanges();
        }

        public void Create(Budget_Bugda Budget2)
        {
            db.Budget_Bugda.Add(Budget2);
            db.SaveChanges();
        }

        public void Update(Comm_Account account)
        {
            db.Entry(account).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
        }

        public void StepUpdate(Lev_Step Step1)
        {
            db.Entry(Step1).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
        }

        public class Mlist02
        {
            public string 單號 { get; set; }
            public decimal LEV { get; set; }
            public string sctrl { get; set; }
            public string 第一關 { get; set; }
            public string 第二關 { get; set; }
            public string 第三關 { get; set; }
        }

        public List<Mlist02> CheckAccount(string account, string Bno)
        {
            // 最後一關，財務主管
            string thee = (from x in db.Dept where x.DeptNo == "1300" select x.Supervisor).ToList()[0].ToString();

            var result = (from a in db.Budget_t
                          join b in db.Lev_Step on a.Bno equals b.Bno
                          //join c in db.Person on b.DEPT equals c.DEPTNO
                          where a.Bno == Bno && b.type == "部門預算"
                          join c in db.Dept on a.Dept equals c.DeptNo
                          select new Mlist02
                          {
                              單號 = a.Bno,
                              LEV = b.lev,
                              sctrl = b.sctrl,
                              第一關 = c.Agent,
                              第二關 = c.Supervisor + ";" + c.Agent + ";",
                              第三關 = thee
                          }).ToList();


            //var result = (from a in db.Budget_t
            //              join b in db.Dept on a.Dept equals b.DeptNo
            //              //join c in db.Person on b.DEPT equals c.DEPTNO
            //              where a.Bno == Bno
            //              select new Mlist02
            //              {
            //                  單號 = a.Bno,
            //                  LEV = a.Lev,
            //                  第一關 = b.Agent,
            //                  第二關 = b.Supervisor + ";" + b.Agent + ";",
            //                  第三關 = thee
            //              }).ToList();

            return result;
        }

        public class Mlist03
        {
            public string 單號 { get; set; }
            public string LEV { get; set; }
            public string 簽核人 { get; set; }
        }

        //public List<Mlist03> Signlist(string Bno)
        //{
        //    var one = (from a in db.Budget_t
        //               join b in db.Dept on a.Dept equals b.DeptNo
        //               where a.Bno == Bno
        //               select b.Agent).ToList()[0].ToString();

        //    //string dept = (from a in db.Person                           
        //    //               where a.Penno == one
        //    //               select a.Deptno).ToList()[0].ToString();

        //    string two = (from x in db.Dept where x.Agent == one select x.Supervisor + "," + x.Agent).ToList()[0].ToString();

        //    //string one = (from x in db.Budget_t where x.Bno == Bno select "123").ToList()[0].ToString();
        //    //string two = (from x in db.Dept where x.Dept == DEPT select x.DEPTSUPR + "," + x.BACKNO).ToList()[0].ToString();
        //    string thee = (from x in db.Dept where x.DeptNo == "1300" select x.Supervisor).ToList()[0].ToString();


        //    List<Mlist03> Check2 = new List<Mlist03>();
        //    Check2.Add(new Mlist03() { 單號 = Bno, LEV = 0, 簽核人 = one });
        //    Check2.Add(new Mlist03() { 單號 = Bno, LEV = 1, 簽核人 = two });
        //    Check2.Add(new Mlist03() { 單號 = Bno, LEV = 2, 簽核人 = thee });

        //    return Check2;
        //}


        //public class Mlist04
        //{
        //    public string Bno { get; set; }
        //    public string Dept { get; set; }
        //    public string lev { get; set; }
        //    public string sctrl { get; set; }
        //    public string sioff { get; set; }
        //    public string auth { get; set; }
        //}

    }
}