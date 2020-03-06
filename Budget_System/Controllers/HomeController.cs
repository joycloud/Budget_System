using Budget_System.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static Budget_System.Models.GetBudgetModel;

namespace Budget_System.Controllers
{
    public class HomeController : Controller
    {
        private DataSet Ds = new DataSet();
        private MISEntities db = new MISEntities();
        public ActionResult Index()
        {
            //List<Budget_t> gamers = new GetBudgetModel().getList();
            Session["account"] = "0004";
            // 依照部門顯示(資訊部、財務部顯示全部)            
            List<Budget_Step> gamers = db.Bugda_select(Session["account"].ToString()).ToList();


            ViewBag.t = gamers;
            return View();

            //DataSet result = new Models.GetBudget_t().查詢();

            //// 強轉table型態
            //EnumerableRowCollection<DataRow> table = result.Tables[0].AsEnumerable();
            ////EnumerableRowCollection<DataRow> table1 = result.Tables[1].AsEnumerable();
        }

        public JsonResult AjaxGetTeams(string Puid)
        {
            List<Member> gamers1 = new GetBudgetModel().getList2(Puid);
            return Json(gamers1, JsonRequestBehavior.AllowGet);
        }

        #region
        // GET: Gamer
        //[HttpGet]
        //public async Task<ActionResult> Index(string searchText, int? pageNumber, string sortBy)
        //{
        //    ViewBag.BnoSort = String.IsNullOrEmpty(sortBy) ? "Bno desc" : "";
        //    ViewBag.DeptSort = sortBy == "Dept" ? "Dept desc" : "Dept";

        //    //List<Budget_t> gamers1 = await gamers;
        //    //List <Budget_t> gamers = await GetBudget_t().getList();


        //    // List<Budget_t> gamers = await db.Budget_t.ToListAsync();
        //    // if (String.IsNullOrEmpty(searchText))
        //    // {
        //    //     gamers = await db.Budget_t
        //    //         .ToListAsync();
        //    // }
        //    //else
        //    // {
        //    //     gamers = await db.Budget_t
        //    //         .Where(x => x.Bno.Contains(searchText) || searchText == null)
        //    //         .ToListAsync();
        //    // }

        //    //IOrderedEnumerable<Budget_t> gamersOrderedEnumerable;
        //    //switch (sortBy)
        //    //{
        //    //    case "Bno":
        //    //        gamersOrderedEnumerable = gamers.OrderBy(x => x.Bno);
        //    //        break;
        //    //    case "Bno desc":
        //    //        gamersOrderedEnumerable = gamers.OrderByDescending(x => x.Bno);
        //    //        break;
        //    //    case "Dept":
        //    //        gamersOrderedEnumerable = gamers.OrderBy(x => x.Dept);
        //    //        break;
        //    //    case "Dept desc":
        //    //        gamersOrderedEnumerable = gamers.OrderByDescending(x => x.Dept);
        //    //        break;
        //    //    default:
        //    //        gamersOrderedEnumerable = gamers.OrderBy(x => x.Bno);
        //    //        break;
        //    //}

        //    ////IPagedList<Budget_t> gamerPagedList = gamers.ToPagedList(pageNumber ?? 1, 5);
        //    //IPagedList<Budget_t> gamerPagedList = gamersOrderedEnumerable.ToPagedList(pageNumber ?? 1, 5);
        //    //return View(gamerPagedList);

        //    //return View(gamers);

        //    //    //List<Budget_t> gamers = new GetBudget_t().getList();
        //    //    //gamers = await db.Budget_t
        //    //    //      .Where(x => x.Bno == searchText || searchText == null)
        //    //    //      .ToListAsync();


        //    //    //return View(gamers);



        //    //    DataSet result = new Models.GetBudget_t().查詢();
        //    //    DataTable Dt = new DataTable();

        //    //    if (!String.IsNullOrEmpty(searchText))
        //    //        Dt = result.Tables[0].Select("Bno='" + searchText + "'").CopyToDataTable();
        //    //    else
        //    //        Dt = result.Tables[0];

        //    //   // List<Budget_t> gamers = Dt.AsEnumerable().ToList();
        //    //    //強轉table型態
        //    //    EnumerableRowCollection<DataRow> table = Dt.AsEnumerable();
        //    //    //EnumerableRowCollection<DataRow> table1 = result.Tables[1].AsEnumerable();

        //    //    ViewBag.t = table;
        //    //    return View(ViewBag.t);
        //}
        #endregion
        // GET: Account/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Account/Create
        [HttpPost]
        public ActionResult Create(string Dept, string Month)
        {
            Budget_t BudgetNo = new Budget_t();
            Budget_Bugda Budget2 = new Budget_Bugda();

            if (String.IsNullOrEmpty(Dept.Trim()))
            {
                ModelState.AddModelError("Dept", $"Dept is empty.");
                return View();
            }
            // 判斷部門
            else if (new Models.GetBudgetModel().Dept(Dept) == 0)
            {
                ModelState.AddModelError("Dept", $"The {Dept} is wrong format.");
                return View();
            }

            // 判斷日期
            if (String.IsNullOrEmpty(Month.Trim()))
            {
                ModelState.AddModelError("Month", $"Month is empty.");
                return View();
            }
            else if (Month.Length != 4 ||
                    (Month.Substring(0, 2) != DateTime.Now.ToString("yyyy").Substring(2, 2) &&
                     Month.Substring(0, 2) != DateTime.Now.AddMonths(1).ToString("yyyy").Substring(2, 2)))
            {
                ModelState.AddModelError("Month", $"The {Month} is wrong format.");
                return View();
            }

            // 判斷這部門的這個月是否已建立
            if (new Models.GetBudgetModel().Check(Dept, Month) > 0)
            {
                ModelState.AddModelError("Dept", $"had created in{Month}.");
                return View();
            }

            string Bno = new GetBudgetModel().Bno();
            Guid uid = Guid.NewGuid();

            BudgetNo.Bno = Bno;
            BudgetNo.Month = Month;
            BudgetNo.Dept = Dept;
            BudgetNo.Uid = uid;
            BudgetNo.Total = 0;
            BudgetNo.Sctrl = "N";
            BudgetNo.Lev = 1;

            List<Bugdalist> gamers = new GetBudgetModel().getCrlist(Dept, Month);

            if (ModelState.IsValid)
            {
                // create 主list
                new GetBudgetModel().Create(BudgetNo);

                // create 明細list
                foreach (var item in gamers)
                {
                    Budget2.Bugda = item.Bugda;
                    Budget2.Total = item.Total;
                    Budget2.Puid = uid;
                    Budget2.Uid = Guid.NewGuid();

                    new GetBudgetModel().Create(Budget2);
                }

                // Insert Step
                db.Step_Insert(Bno, "預算系統", "部門預算", Session["account"].ToString());
            }
            return RedirectToAction("Subject", "Home", new { Bno });
        }

        // 科目================================================================
        public ActionResult Subject(string Bno)
        {
            List<Bugdalist2> gamers = new GetBudgetModel().getCrlist2(Bno);
            ViewBag.h = gamers;

            return View();
        }


        public ActionResult Items(string Puid, String Bugda)
        {
            List<Itemslist> gamers = new GetBudgetModel().getItems(Puid);
            ViewBag.h = gamers;
            ViewBag.k = Puid;
            ViewBag.Bugda = Bugda;
            //ViewBag.Guid = Guid.NewGuid();

            return View();
        }

        //public ActionResult DeleteItem(string Puid, string Uid)
        //{
        //    //if (Status == "create")
        //    //    return RedirectToAction("ItemsAbout", "Home", new { Puid, Uid, Status });
        //    //else if (Status == "edit")
        //    //    return RedirectToAction("ItemsAbout", "Home", new { Puid, Uid, Status });
        //    //else
        //    //{


        //    if (ModelState.IsValid)
        //    {
        //        Guid aa = new Guid(Uid);
        //        new GetBudgetModel().ItemeDelete(aa);
        //    }
        //    return RedirectToAction("Items", "Home", new { Puid });
        //}

        //public ActionResult ItemsAbout(string Puid, string Uid, string Status)
        //{
        //    if (Status == "create")
        //    {
        //        return View();
        //    }
        //    else if (Status == "edit")
        //    {
        //        Budget_Items gamers = new GetBudgetModel().getItemsAboutList(Uid);
        //        return View(gamers);
        //    }
        //    else
        //    {
        //        return View();
        //    }
        //}


        // 記得拆開寫Status

        [HttpPost]
        public ActionResult ItemsAbout(string Items, string Remark, double Price, int Amount, string Puid, string Uid, string Status)
        {
            Budget_Items items = new Budget_Items();
            items.Items = Items;
            items.Remark = Remark;
            items.Price = Convert.ToDecimal(Price);
            items.Amount = Amount;
            items.Total = Convert.ToInt32(Price * Amount);
            items.Puid = new Guid(Puid);

            if (Status == "create")
            {
                items.Uid = Guid.NewGuid();
                new GetBudgetModel().ItemeCreate(items);
                return RedirectToAction("Items", "Home", new { Puid });
            }
            else
            {
                items.Uid = new Guid(Uid);

                new GetBudgetModel().ItemeUpdate(items);
                return RedirectToAction("Items", "Home", new { Puid });
            }
        }

        public ActionResult Signoff(string Bno, string isPost = "0")
        {
            //string account = "0001";
            //string account = Session["account"].ToString(); 

            // 判斷帳號
            List<Mlist02> Check = new GetBudgetModel().CheckAccount(Session["account"].ToString(), Bno);

            //var gamers = db.Step_Signoff(Session["account"].ToString(), Bno);

            // 取現在關卡
            string LEV = "1.00";
            string 當關人 = "";


            if (Check[1].LEV == 2 && Check[1].sctrl == "Y")
            {
                LEV = Check[2].LEV.ToString();
                當關人 = Check[0].第三關.ToString();
            }
            else if (Check[0].LEV == 1 && Check[0].sctrl == "Y")
            {
                LEV = Check[1].LEV.ToString();
                當關人 = Check[0].第二關.ToString();
            }
            else if (Check[2].LEV == 3 && Check[2].sctrl == "Y")
            {
                LEV = "4.00";
                當關人 = "";
            }
            else
                當關人 = Check[0].第一關.ToString();


            string SCTRL = "";
            // 判斷account============================            
            if (當關人.Contains(";"))
            {
                // 當關有複數的人
                int i = 0;
                string[] sArray = 當關人.Split(';');
                foreach (string AA in sArray)
                {
                    if (!string.IsNullOrEmpty(AA))
                    {
                        if (AA == Session["account"].ToString())
                            i++;
                    }
                }
                if (i == 0)
                    SCTRL = "N";
            }
            else
            {
                if (當關人 != Session["account"].ToString())
                    SCTRL = "N";
            }

            List<Mlist03> Check1 = new List<Mlist03>();

            Check1.Add(new Mlist03() { 單號 = Bno, LEV = Check[0].LEV.ToString(), 簽核人 = Check[0].第一關.ToString() });
            Check1.Add(new Mlist03() { 單號 = Bno, LEV = Check[1].LEV.ToString(), 簽核人 = Check[0].第二關.ToString() });
            Check1.Add(new Mlist03() { 單號 = Bno, LEV = Check[2].LEV.ToString(), 簽核人 = Check[0].第三關.ToString() });


            ViewBag.h = SCTRL;
            ViewBag.t = Check1;
            ViewBag.LEV = LEV;
            ViewBag.單號 = Bno;
            ViewBag.isPost = isPost;
            return View();
        }
        [HttpPost]
        public ActionResult Signoff(FormCollection post)
        {
            string PP = post["PP"];
            string Bno = PP.Substring(0, 9);

            int i = PP.IndexOf(";") + 1;
            decimal Lev = Convert.ToDecimal(PP.Substring(i, PP.Length - i));
            Lev_Step Step1 = new Lev_Step();

            if (PP.Contains("簽核"))
            {
                List<Lev_Step> Step = db.Lev_Step.Where(o => o.Bno == Bno && o.lev == Lev && o.type == "部門預算").ToList();
                // 簽到下一關
                Step1.Bno = Bno;
                Step1.system = Step[0].system;
                Step1.type = Step[0].type;
                Step1.lev = Step[0].lev;
                Step1.sign = Step[0].sign;
                Step1.sctrl = "Y";
                Step1.cruser = Step[0].cruser;
                Step1.crdate = Step[0].crdate;
                Step1.eduser = Session["account"].ToString();
                Step1.eddate = DateTime.Now;
                new GetBudgetModel().StepUpdate(Step1);
            }
            else
            {
                List<Lev_Step> Step = db.Lev_Step.Where(o => o.Bno == Bno && o.lev == Lev - 1 && o.type == "部門預算").ToList();
                // 退回上一關
                Step1.Bno = Bno;
                Step1.system = Step[0].system;
                Step1.type = Step[0].type;
                Step1.lev = Step[0].lev;
                Step1.sign = Step[0].sign;
                Step1.sctrl = "N";
                Step1.cruser = Step[0].cruser;
                Step1.crdate = Step[0].crdate;
                Step1.eduser = Session["account"].ToString();
                Step1.eddate = DateTime.Now;
                new GetBudgetModel().StepUpdate(Step1);
            }

            return RedirectToAction("Signoff", new { Bno = Bno, isPost = "1" });
        }

        public JsonResult InsertCustomers(List<Itemslist> customers)
        {
            string Puid = customers[0].Puid;
            Budget_Items item = new Budget_Items();

            foreach (var oRow in customers)
            {
                // edit
                if (oRow.sctrl.ToString() == "X")
                {
                    // 舊資料update
                    if (!string.IsNullOrEmpty(oRow.Uid.ToString()))
                    {
                        item.Items = oRow.Items;
                        item.Remark = oRow.Remark;
                        item.Amount = oRow.Amount;
                        item.Price = oRow.Price;
                        item.Total = oRow.Total;
                        item.Puid = new Guid(Puid);
                        item.Uid = new Guid(oRow.Uid);
                        item.sctrl = oRow.sctrl;
                        item.eduser = Session["account"].ToString();
                        item.eddate = DateTime.Now;
                        new GetBudgetModel().ItemeUpdate(item);
                    }
                }
                else
                {
                    // 舊資料update
                    if (!string.IsNullOrEmpty(oRow.Uid))
                    {
                        if (ModelState.IsValid)
                        {
                            item.Items = oRow.Items;
                            item.Remark = oRow.Remark;
                            item.Amount = oRow.Amount;
                            item.Price = oRow.Price;
                            item.Total = oRow.Total;
                            item.Puid = new Guid(Puid);
                            item.Uid = new Guid(oRow.Uid);
                            item.sctrl = oRow.sctrl;
                            item.eduser = Session["account"].ToString();
                            item.eddate = DateTime.Now;
                            new GetBudgetModel().ItemeUpdate(item);
                        }
                    }
                    else
                    {
                        item.Items = oRow.Items;
                        item.Remark = oRow.Remark;
                        item.Amount = oRow.Amount;
                        item.Price = oRow.Price;
                        item.Total = oRow.Total;
                        item.Puid = new Guid(Puid);
                        item.Uid = Guid.NewGuid();
                        item.sctrl = "N";
                        item.cruser = Session["account"].ToString();
                        item.crdate = DateTime.Now;
                        new GetBudgetModel().ItemeCreate(item);
                    }
                }
                // update Total
                db.UpdateTotal(Puid);
            }
            return Json(1);
        }

        public ActionResult Items2(string Uid, String Bugda)
        {
            List<Itemslist> gamers = new GetBudgetModel().getItems(Uid);

            //List<Itemslist> gamers = new GetBudgetModel().getItems(Puid);
            ViewBag.h = gamers;
            //ViewBag.k = Puid;
            ViewBag.Bugda = Bugda;
            //ViewBag.Guid = Guid.NewGuid();

            return View();
        }
    }
}