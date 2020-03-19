using prjNewPet.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace prjNewPet.Controllers
{
    public class MemberFronteditController : Controller
    {
        // GET: MemberFrontedit
        public ActionResult Edit(int? fid)
        {
            SelectList selectList = new SelectList(this.GetCity(), "fCityID", "fName");
            ViewBag.SelectList = selectList;

            dbNewPetEntities db = new dbNewPetEntities();

            var table = (from p in db.tMembers
                         where p.fMemberID == fid
                         select p).FirstOrDefault();

            return View("Edit", "_layout_Membercenter", table);
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

            SelectList selectList = new SelectList(this.GetCity(), "fCityID", "fName");
            ViewBag.SelectList = selectList;
            //重新選取一次

            db.SaveChanges();

            return View("Edit", "_layout_Membercenter", table);
        }
    }
}