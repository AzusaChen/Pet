using prjNewPet.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace prjNewPet.Controllers
{
    public class SupplierController : Controller
    {

        dbNewPetEntities db = new dbNewPetEntities();

        // GET: Supplier
        public ActionResult SupplierList()
        {
            //SelectList selectListsuppliers = new SelectList(this.GetSuppliers(), "fSupplierID", "fName");
            //ViewBag.selectListsuppliers = selectListsuppliers;

            SelectList selectListsuppliers = new SelectList(this.GetSuppliers(), "fCityID", "fName");
            ViewBag.selectListsuppliers = selectListsuppliers;

            var suppliers = from s in db.tSuppliers
                           select s;
            return View("SupplierList", "_Layout_Backmanagement", suppliers);
        }

        private IEnumerable<tCity> GetSuppliers()
        {
            using (dbNewPetEntities db = new dbNewPetEntities())//釋放記憶體
            {
                var query = db.tCities.OrderBy(m => m.fCityID);

                return query.ToList();
            }
        }

        [HttpPost]
        public ActionResult SupplierList(string cityId)
        {
            List<tSupplier> list = null;

            if (cityId != "")
            {
                int cID = Convert.ToInt32(cityId);

                var q = from s in db.tSuppliers
                        where s.fCityID == cID
                        select s;

                list = q.ToList();
            }
            else
            {
                list = db.tSuppliers.ToList();
            }

            //SelectList selectListsuppliers = new SelectList(this.GetSuppliers(), "fSupplierID", "fName");
            //ViewBag.selectListsuppliers = selectListsuppliers;

            SelectList selectListsuppliers = new SelectList(this.GetSuppliers(), "fCityID", "fName");
            ViewBag.selectListsuppliers = selectListsuppliers;

            return View("SupplierList", "_Layout_Backmanagement", list);
        }





        private IEnumerable<tCity> GetCity()
        {
            using (dbNewPetEntities db = new dbNewPetEntities())//釋放記憶體
            {
                var query = db.tCities.OrderBy(m => m.fCityID);

                return query.ToList();
            }
        }

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
        
        public ActionResult New()
        {
            SelectList selectListcity = new SelectList(this.GetCity(), "fCityID", "fName");
            ViewBag.SelectListcity = selectListcity;

            return View("New", "_Layout_Backmanagement");
        }

        [HttpPost]
        public ActionResult New(tSupplier supp)
        {

                db.tSuppliers.Add(supp);

                db.SaveChanges();
            

            return RedirectToAction("SupplierList");
        }

        public ActionResult Delete(int fSupplierID)
        {
           
            var suppliers= (from s in db.tSuppliers
                            where s.fSupplierID == fSupplierID
                            select s).FirstOrDefault();

            db.tSuppliers.Remove(suppliers);


            db.SaveChanges();

            return RedirectToAction("SupplierList");
        }

        public ActionResult Edit(int fSupplierID)
        {
            SelectList selectListcity = new SelectList(this.GetCity(), "fCityID", "fName");
            ViewBag.SelectListcity = selectListcity;


            var suppliers = (from s in db.tSuppliers
                            where s.fSupplierID == fSupplierID
                            select s).FirstOrDefault();

           

            if (suppliers == null)
            {
                return RedirectToAction("ProductList");
            }
            else
            {
                return View("Edit", "_Layout_Backmanagement", suppliers);
            }

        }

        [HttpPost]
        public ActionResult Edit(tSupplier supplier)
        {

            var q = (from s in db.tSuppliers
                     where s.fSupplierID == supplier.fSupplierID
                     select s).FirstOrDefault();

            if (supplier.tCity == null)
            {
                q.fCityID = q.fCityID;
            }
            else if (supplier.fName == null)
            {
                q.fName = q.fName;
            }
            else if (supplier.fContactName == null)
            {
                q.fContactName = q.fContactName;
            }
            else if (supplier.fAddress == null)
            {
                q.fAddress = q.fAddress;
            }
            else if (supplier.fPhone == null)
            {
                q.fPhone = q.fPhone;
            }

            if (supplier.fRegionID == 0)
            {
                q.tRegion.fRegionID = q.tRegion.fRegionID;
            }
            else
            {
                q.fRegionID = supplier.fRegionID;
            }

            q.fName = supplier.fName;
            q.fContactName = supplier.fContactName;
            q.fAddress = supplier.fAddress;
            q.fCityID = supplier.fCityID;
            //q.fRegionID = supplier.fRegionID;
            q.fPhone = supplier.fPhone;

            db.SaveChanges();



            return RedirectToAction("SupplierList");
        }



    }
}