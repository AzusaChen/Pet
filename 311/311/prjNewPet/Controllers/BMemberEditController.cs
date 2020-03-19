using PagedList;
using prjNewPet.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace prjNewPet.Controllers
{
    public class BMemberEditController : Controller
    {
        // GET: BMemberEdit
        private int pageSize = 5;

        public ActionResult List(int page = 1)
        {
            dbNewPetEntities db = new dbNewPetEntities();

            int currentpage = page < 1 ? 1 : page;

            var table = from p in db.tMembers
                        select p;

            var result = table.ToList().ToPagedList(currentpage, pageSize);

            //回傳list so view用IEnumerable

            return View("List", "_Layout_Backmanagement", result);
        }

        public ActionResult Edit(int? fid)
        {
            SelectList selectList = new SelectList(this.GetCity(), "fCityID", "fName");
            ViewBag.SelectList = selectList;

            dbNewPetEntities db = new dbNewPetEntities();

            var table = (from p in db.tMembers
                         where p.fMemberID == fid
                         select p).FirstOrDefault();

            //只回傳第一筆所以view用prjNewPet.Models.tMember

            return View("Edit", "_Layout_Backmanagement", table);
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
        public ActionResult Edit(tMember member)
        {
            if (member.photo != null)
            {
                string fileName = Path.GetFileName(member.photo.FileName);
                var pathPhoto = Path.Combine(Server.MapPath("~/Images"), fileName);
                member.photo.SaveAs(pathPhoto);

                member.fIcon = "/Images/" + fileName;
            }

            dbNewPetEntities db = new dbNewPetEntities();

            var table = (from p in db.tMembers
                         where p.fMemberID == member.fMemberID
                         select p).FirstOrDefault();

            table.fName = member.fName;
            table.fNickName = member.fNickName;
            table.fPassword = member.fPassword;

            //table.EnconomicStatus = member.EnconomicStatus;
            table.fPhone = member.fPhone;
            table.fEMail = member.fEMail;

            table.fCityID = member.fCityID;
            if (member.fRegionID == 0)
            {
                table.fRegionID = table.fRegionID;
            }
            else
            {
                table.fRegionID = member.fRegionID;
            }

            table.fAddressDetail = member.fAddressDetail;

            if (member.fIcon == null)
            {
                table.fIcon = table.fIcon;
            }
            else
            {
                table.fIcon = member.fIcon;
            }

            //SelectList selectList = new SelectList(this.GetCity(), "fCityID", "fName");
            //ViewBag.SelectList = selectList;

            db.SaveChanges();

            return RedirectToAction("List", "BMemberEdit", null);
        }

        public ActionResult Delete(int? fid)
        {
            dbNewPetEntities dbcontext = new dbNewPetEntities();

            var discussion = (from d in dbcontext.tDiscussions
                              where d.fMemberID == fid
                              select d).ToList();

            foreach (var item in discussion)
            {
                dbcontext.tDiscussions.Remove(item);

                dbcontext.SaveChanges();
            }

            //==============================================

            var ordertable = (from o in dbcontext.tOrders
                              where o.fMemberID == fid
                              select o).ToList();

            foreach (var item in ordertable)
            {
                var ordertable1 = (from o in dbcontext.tOrder_Detail
                                   where o.fOrderID == item.fOrderID
                                   select o).ToList();

                foreach (var item1 in ordertable1)
                {
                    dbcontext.tOrder_Detail.Remove(item1);

                    dbcontext.SaveChanges();
                }
            }

            var ordertables = (from o in dbcontext.tOrders
                               where o.fMemberID == fid
                               select o).ToList();

            foreach (var item in ordertables)
            {
                dbcontext.tOrders.Remove(item);

                dbcontext.SaveChanges();
            }

            ////============================================

            //var sentpet = (from s in dbcontext.tSentPets
            //               where s.fMemberID == fid
            //               select s).ToList();

            //foreach (var item in sentpet)
            //{
            //    dbcontext.tSentPets.Remove(item);

            //    dbcontext.SaveChanges();
            //}

            //===========================================

            var petapp = (from p in dbcontext.tPetMembers
                          where p.fMemberID == fid
                          select p).ToList();

            foreach (var item in petapp)
            {
                dbcontext.tPetMembers.Remove(item);

                dbcontext.SaveChanges();
            }

            //=========================================

            //var lostpet = (from l in dbcontext.tLostPets
            //               where l.fMemberID == fid
            //               select l).ToList();

            //foreach (var item in lostpet)
            //{
            //    dbcontext.tLostPets.Remove(item);

            //    dbcontext.SaveChanges();
            //}
            //=========================================
            var foundpet = (from f in dbcontext.tFoundPets
                            where f.fMemberID == fid
                            select f).ToList();

            foreach (var item in foundpet)
            {
                dbcontext.tFoundPets.Remove(item);

                dbcontext.SaveChanges();
            }

            //============================================

            var comment = (from c in dbcontext.tComments
                           where c.fMemberID == fid
                           select c).ToList();
            //用Tolist刪除所有這個資料,First只會刪除第一筆

            foreach (var item in comment)
            {
                dbcontext.tComments.Remove(item);

                dbcontext.SaveChanges();
            }

            //=============================================
            var favorite = (from c in dbcontext.tFavorites
                            where c.fMemberID == fid
                            select c).ToList();
            //用Tolist刪除所有這個資料,First只會刪除第一筆

            foreach (var item in favorite)
            {
                dbcontext.tFavorites.Remove(item);

                dbcontext.SaveChanges();
            }
            //==============================================
            var report = (from c in dbcontext.tReports
                          where c.fMemberID == fid
                          select c).ToList();
            //用Tolist刪除所有這個資料,First只會刪除第一筆

            foreach (var item in report)
            {
                dbcontext.tReports.Remove(item);

                dbcontext.SaveChanges();
            }

            //==============================================

            var like = (from c in dbcontext.tLikes
                        where c.fMemberID == fid
                        select c).ToList();
            //用Tolist刪除所有這個資料,First只會刪除第一筆

            foreach (var item in like)
            {
                dbcontext.tLikes.Remove(item);

                dbcontext.SaveChanges();
            }

            //==============================================
            var member = (from m in dbcontext.tMembers
                          where m.fMemberID == fid
                          select m).First();

            dbcontext.tMembers.Remove(member);

            dbcontext.SaveChanges();

            return RedirectToAction("List", "BMemberEdit", null);
        }
    }
}