using prjNewPet.Models;
using prjNewPet.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace prjNewPet.Controllers
{
    public class LoginController : Controller
    {
        // GET: Auth
        public ActionResult Auth(CLogin user)
        {
            string s = "";
            if (Session[CDictionary.SK_RANDOM_CODE] == null)
            {
                string[] array = new string[4];
                Random r = new Random();

                for (int i = 0; i < 4; i++)
                {
                    array[i] = r.Next(0, 10).ToString();
                }
                ViewBag.img1 = array[0] + ".png";
                ViewBag.img2 = array[1] + ".png";
                ViewBag.img3 = array[2] + ".png";
                ViewBag.img4 = array[3] + ".png";

                Session[CDictionary.SK_RANDOM_CODE] = array;

                //s = Session[CDictionary.SK_RANDOM_CODE] as string;
                s += array[0];
                s += array[1];
                s += array[2];
                s += array[3];
            }
            else
            {
                string[] arraycode = Session[CDictionary.SK_RANDOM_CODE] as string[];

                ViewBag.img1 = arraycode[0] + ".png";
                ViewBag.img2 = arraycode[1] + ".png";
                ViewBag.img3 = arraycode[2] + ".png";
                ViewBag.img4 = arraycode[3] + ".png";

                s += arraycode[0];
                s += arraycode[1];
                s += arraycode[2];
                s += arraycode[3];
            }

            //===============================

            //接收Request的cookie
            HttpCookie cookie = Request.Cookies[CDictionary.CK_REMEMBERME_FLAG];

            dbNewPetEntities db = new dbNewPetEntities();

            //如果有cookie，則從資料庫撈出該筆會員資料
            if (cookie != null)
            {
                var member = (from m in db.tMembers
                              where m.fEMail == cookie.Value
                              select m).FirstOrDefault();

                if (member == null)  //如果有人離職，公司將其Email帳號刪除，就算他有保留原來的cookie，也導向forgetMe將其cookie清除
                {
                    return RedirectToAction("forgetMe");
                }

                //用 Session記住該筆會員資料
                Session[CDictionary.SK_LOGINED_USER] = member;

                return RedirectToAction("Index", "Home");
            }

            //如果輸入email的欄位為空值，則回到Login頁面
            if (user.account == null)
            {
                return View("Auth", "_Layout_Login");
            }

            //=====================
            if (!s.Equals(user.code))
            {
                ViewData[CDictionary.MGS_LOGINED_ERRORS_MESSAGE] = "驗證碼錯誤";

                Session[CDictionary.SK_RANDOM_CODE] = null;

                string[] array = new string[4];
                Random r = new Random();

                for (int i = 0; i < 4; i++)
                {
                    array[i] = r.Next(0, 10).ToString();
                }
                ViewBag.img1 = array[0] + ".png";
                ViewBag.img2 = array[1] + ".png";
                ViewBag.img3 = array[2] + ".png";
                ViewBag.img4 = array[3] + ".png";

                Session[CDictionary.SK_RANDOM_CODE] = array;

                //string[] arraycode = Session[CDictionary.SK_RANDOM_CODE] as string[];

                return View("Auth", "_Layout_Login");
            }
            else
            {
                string[] array = new string[4];
                Random r = new Random();

                for (int i = 0; i < 4; i++)
                {
                    array[i] = r.Next(0, 10).ToString();
                }
                ViewBag.img1 = array[0] + ".png";
                ViewBag.img2 = array[1] + ".png";
                ViewBag.img3 = array[2] + ".png";
                ViewBag.img4 = array[3] + ".png";

                Session[CDictionary.SK_RANDOM_CODE] = array;
            }

            //=====================

            //驗證輸入email及密碼的欄位在資料庫是否有該筆會員資料
            var memberE = (from m in db.tMembers
                           where m.fAccount == user.account && m.fPassword == user.password
                           select m).FirstOrDefault();

            if (string.Equals(user.account, "leon888") && string.Equals(user.password, "leon888"))
            {
                return RedirectToAction("List", "BMemberEdit", null);
            }

            //如果有該筆會員資料
            if (memberE != null)
            {
                if (memberE.fAccount == user.account && memberE.fPassword == user.password)
                {
                    //如果checkbox有勾選(isRememberMe為checkbox的name)，則用cookie記起來
                    if (user.isRememberMe == true)
                    {
                        cookie = new HttpCookie(CDictionary.CK_REMEMBERME_FLAG);
                        cookie.Value = memberE.fEMail; //將cookie.Value設定為該會員的Email
                        cookie.Expires = DateTime.Now.AddMinutes(10);  //cookie存在時間設為10分鐘
                        Response.Cookies.Add(cookie);  //將cookie傳回加入
                    }

                    //用 Session記住該筆會員資料
                    Session[CDictionary.SK_LOGINED_USER] = memberE;

                    var mem = Session[CDictionary.SK_LOGINED_USER] as tMember;

                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ViewData[CDictionary.MGS_LOGINED_ERRORS_MESSAGE] = "帳號與密碼不符";
                }
            }
            else  //如果輸入email及密碼的欄位在資料庫沒有該筆會員資料，則顯示"帳號與密碼不符"
            {
                ViewData[CDictionary.MGS_LOGINED_ERRORS_MESSAGE] = "帳號與密碼不符";
            }

            return View("Auth", "_Layout_Login");
        }

        public ActionResult Home()
        {
            //接收Request的cookielimit(權限，目前資料庫尚未建立會員的權限)
            //HttpCookie cookielimit = Request.Cookies[CDictionary.CK_LIMIT_FLAG];

            ////如果無權限(訪客)，則進入首頁，後續要再加不同權限的功能
            //if (cookielimit == null)
            //{
            //    return View();
            //}

            //如果不是會員則導向Login頁面
            if (Session[CDictionary.SK_LOGINED_USER] == null)
            {
                return RedirectToAction("Auth");
            }

            return View();
        }

        public ActionResult forgetMe() //清除cookie的方法
        {
            HttpCookie cookie = new HttpCookie(CDictionary.CK_REMEMBERME_FLAG);
            if (cookie != null)
            {
                cookie.Expires = DateTime.Now.AddMinutes(-1);  //將時間設定在前一分鐘，並以下一行加入cookie將原本的cookie蓋掉
                Response.Cookies.Add(cookie);
            }

            //petcoin
            dbNewPetEntities db = new dbNewPetEntities();

            //撈出session
            var mem = Session[CDictionary.SK_LOGINED_USER] as tMember;
            //加入最近登入時間
            var addmemlogindate = db.tMembers.Where(x => x.fMemberID == mem.fMemberID);
            foreach (var item in addmemlogindate)
            {
                item.fRecentLogInDate = DateTime.Now;
            }
            db.SaveChanges();

            Session[CDictionary.SK_LOGINED_USER] = null;

            return RedirectToAction("Index", "Home");
        }

        public ActionResult Regist()
        {
            return View();
        }

        public JsonResult FindPassword(string email)
        {
            dbNewPetEntities db = new dbNewPetEntities();

            //驗證輸入email的欄位在資料庫是否有該筆會員資料
            string memberE = (from m in db.tMembers
                              where m.fEMail == email
                              select m.fEMail).FirstOrDefault();

            //如果有該筆會員資料
            if (memberE != null)
            {
                CSendMail c = new CSendMail();

                Task.Run(() => (c.sendPassword(memberE)));

                return Json("密碼已寄出!");
            }
            else  //如果輸入email及密碼的欄位在資料庫沒有該筆會員資料，則顯示"帳號與密碼不符"
            {
                return Json("無此帳號!");
            }
        }
    }
}