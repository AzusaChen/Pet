using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using prjNewPet.Models;

namespace prjNewPet.Controllers
{
    public class MemberFrontController : Controller
    {
        // GET: MemberFront
        public ActionResult New()
        {
            SelectList selectList = new SelectList(this.GetCity(), "fCityID", "fName");
            ViewBag.SelectList = selectList;

            return View("New", "_Layout_MemberFront");
        }

        private IEnumerable<tCity> GetCity()
        {
            using (dbNewPetEntities db = new dbNewPetEntities())//釋放記憶體
            {
                var query = db.tCities.OrderBy(m => m.fCityID);

                return query.ToList();
            }
        }

        //後端產生 Orders JSON 資料的程式如下, return this.Json(items);後回到ajax

        [HttpPost]
        public JsonResult Regions(string cityID)
        {
            List<KeyValuePair<string, string>> items = new List<KeyValuePair<string, string>>();

            if (!string.IsNullOrWhiteSpace(cityID))
            {
                var regions = this.GetOrders(cityID);
                if (regions.Count() > 0)
                {
                    foreach (var region in regions)
                    {
                        items.Add(new KeyValuePair<string, string>(
                            region.fRegionID.ToString(),
                            region.fName
                            ));
                    }
                }
            }
            return this.Json(items);
        }

        private IEnumerable<tRegion> GetOrders(string cityID)
        {
            using (dbNewPetEntities db = new dbNewPetEntities())
            {
                var query = db.tRegions.Where(x => x.fCityID.ToString() == cityID);
                return query.ToList();
            }
        }

        [HttpPost]
        public ActionResult New(tMember member)
        {
            //if(member.photo!=null && member.fCityID!=0)
            
                string fileName = Path.GetFileName(member.photo.FileName);
                var pathPhoto = Path.Combine(Server.MapPath("~/Images"), fileName);
                member.photo.SaveAs(pathPhoto);

                member.fIcon = "/Images/" + fileName;

            //註冊會員得到petcoin
            member.fPetCoin = 200;
            member.fRegisterDate = DateTime.Now;
            dbNewPetEntities db = new dbNewPetEntities();


                db.tMembers.Add(member);
                db.SaveChanges();



            //ViewBag.need = "請填寫 城市,地區,照片 以及其他必要欄位";
            //return View("New", "_Layout_MemberFront");



            System.Threading.Thread.Sleep(3000);

            return RedirectToAction("Auth", "Login", null);
        }
    }
}