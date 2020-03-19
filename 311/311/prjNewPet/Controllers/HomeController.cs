using log4net;
using prjNewPet.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace prjNewPet.Controllers
{
    public class HomeController : Controller
    {
        public static ILog logger = LogManager.GetLogger("Web");

        public ActionResult Index()
        {
            dbNewPetEntities db = new dbNewPetEntities();
            var mem = Session[CDictionary.SK_LOGINED_USER] as tMember;
            try
            {
                //int y = 10;
                //int z = 0;
                //var qq = y / z;

                //petcoin
                if (mem != null)
                {
                    //比較上次登入和現在時間是否超過一天
                    double EnoughloginDay = new TimeSpan(DateTime.Now.Ticks - mem.fRecentLogInDate.Value.Ticks).TotalDays;
                    if (EnoughloginDay > 1)
                    {
                        var SKMEM = Session[CDictionary.SK_LOGINED_USER] as tMember;



                        //petcoin
                        ViewBag.GET50 = 1;
                        var add50petcoin = db.tMembers.Where(x => x.fMemberID == mem.fMemberID).AsEnumerable();
                        foreach (var item in add50petcoin)
                        {
                            item.fPetCoin += 50;
                            SKMEM.fPetCoin = item.fPetCoin;//存回session讓商城有辦法抓到
                            //立即設定最近登入時間以防頁面跳轉回來會再拿到
                            item.fRecentLogInDate = DateTime.Now;
                        }
                        //也要將SESSION裡的最近登入日期改掉
                        SKMEM.fRecentLogInDate = DateTime.Now;
                        db.SaveChanges();
                    }
                    else
                    {
                        ViewBag.GET50 = 2;
                    }

                }
            }
            catch (Exception ex)
            {

                logger.Error(ex.ToString());
            }





            return View();
        }


    }
}