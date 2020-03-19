using prjNewPet.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace prjNewPet.Controllers
{
    public class ProductController : Controller
    {
        // GET: Product
        dbNewPetEntities db = new dbNewPetEntities();

        public ActionResult ProductList()
        {
            SelectList selectListcategories = new SelectList(this.GetCategories(), "fCategoryID", "fName");
            ViewBag.selectListcategories = selectListcategories;

            SelectList selectListsuppliers = new SelectList(this.GetSuppliers(), "fSupplierID", "fName");
            ViewBag.selectListsuppliers = selectListsuppliers;

            var products = from p in db.tProducts
                           select p;

            

            return View("ProductList","_Layout_Backmanagement", products);
        }

        private IEnumerable<tSupplier> GetSuppliers()
        {
            using (dbNewPetEntities db = new dbNewPetEntities())//釋放記憶體
            {
                var query = db.tSuppliers.OrderBy(m => m.fSupplierID);

                return query.ToList();
            }
        }

        private IEnumerable<tCategory> GetCategories()
        {
            using (dbNewPetEntities db = new dbNewPetEntities())//釋放記憶體
            {
                var query = db.tCategories.OrderBy(m => m.fCategoryID);

                return query.ToList();
            }
        }

        [HttpPost]
        public ActionResult ProductList(string cateId, string suppId)
        {
            List<tProduct> list = null;

           

            if (cateId != "" && suppId != "")
            {
                int cID = Convert.ToInt32(cateId);
                int sID = Convert.ToInt32(suppId);

                var q = from p in db.tProducts
                        where p.fCategoryID == cID && p.fSupplierID == sID
                        select p;

                list = q.ToList();

            }
            else if(cateId != "" && suppId == "")
            {
                int cID = Convert.ToInt32(cateId);
                var q = from p in db.tProducts
                        where p.fCategoryID == cID
                        select p;

                list = q.ToList();
            }
            else if (cateId == "" && suppId != "")
            {
                int sID = Convert.ToInt32(suppId);
                var q = from p in db.tProducts
                        where p.fSupplierID == sID
                        select p;

                list = q.ToList();
            }

            else
            {
                list = db.tProducts.ToList();
            }

            SelectList selectListcategories = new SelectList(this.GetCategories(), "fCategoryID", "fName");
            ViewBag.selectListcategories = selectListcategories;

            SelectList selectListsuppliers = new SelectList(this.GetSuppliers(), "fSupplierID", "fName");
            ViewBag.selectListsuppliers = selectListsuppliers;

            return View("ProductList", "_Layout_Backmanagement",list);
        }




        private IEnumerable<tCategory> GetCategory()
        {
            using (dbNewPetEntities db = new dbNewPetEntities())//釋放記憶體
            {
                var query = db.tCategories.OrderBy(m => m.fCategoryID);

                return query.ToList();
            }
        }

        private IEnumerable<tSupplier> GetSupplier()
        {
            using (dbNewPetEntities db = new dbNewPetEntities())//釋放記憶體
            {
                var query = db.tSuppliers.OrderBy(m => m.fSupplierID);

                return query.ToList();
            }
        }



        public ActionResult Edit(int fProductID)
        {
            var products = (from p in db.tProducts
                           where p.fProductID == fProductID
                            select p).FirstOrDefault();

            SelectList selectList = new SelectList(this.GetCategory(), "fCategoryID", "fName");
            ViewBag.SelectList = selectList;


            SelectList selectListsupp = new SelectList(this.GetSupplier(), "fSupplierID", "fName");
            ViewBag.SelectListsupp = selectListsupp;

            if (products== null)
            {
                return RedirectToAction("ProductList");
            }
            else
            {
                return View("Edit", "_Layout_Backmanagement",products);
            }

        }

        [HttpPost]
        public ActionResult Edit(tProduct prod, HttpPostedFileBase image)
        {
            if (image != null)
            {
                string fileName = Path.GetFileName(image.FileName);
                var path = Path.Combine(Server.MapPath("~/images/ProductImages/商品圖片/"), fileName);

                image.SaveAs(path);

                var q = (from p in db.tProducts
                         where p.fProductID == prod.fProductID
                         select p).FirstOrDefault();

                q.fName = prod.fName;
                q.fPic = "/images/ProductImages/商品圖片/"+fileName;
                q.fCategoryID = prod.fCategoryID;
                q.fUnitPrice = prod.fUnitPrice;
                q.fUnitOnOrder = prod.fUnitOnOrder;
                q.fUnitInStock = prod.fUnitInStock;
                q.fOnShelfDate = prod.fOnShelfDate;
                q.fSupplierID = prod.fSupplierID;
                q.fSafeStock = prod.fSafeStock;
                q.fDescription = prod.fDescription;

                db.SaveChanges();
            }
            else
            {
                var q = (from p in db.tProducts
                         where p.fProductID == prod.fProductID
                         select p).FirstOrDefault();

                q.fName = prod.fName;
                
                q.fCategoryID = prod.fCategoryID;
                q.fUnitPrice = prod.fUnitPrice;
                q.fUnitOnOrder = prod.fUnitOnOrder;
                q.fUnitInStock = prod.fUnitInStock;
                q.fOnShelfDate = prod.fOnShelfDate;
                q.fSupplierID = prod.fSupplierID;
                q.fSafeStock = prod.fSafeStock;
                q.fDescription = prod.fDescription;

                db.SaveChanges();
            }
        
            return RedirectToAction("ProductList");
        }

        public ActionResult New()
        {
            SelectList selectList = new SelectList(this.GetCategory(), "fCategoryID", "fName");
            ViewBag.SelectList = selectList;


            SelectList selectListsupp = new SelectList(this.GetSupplier(), "fSupplierID", "fName");
            ViewBag.SelectListsupp = selectListsupp;

            return View("New", "_Layout_Backmanagement");
        }

        [HttpPost]
        public ActionResult New(tProduct prod, HttpPostedFileBase image)
        {

            if (image != null)
            {
                string fileName = Path.GetFileName(image.FileName);
                var path = Path.Combine(Server.MapPath("~/images/ProductImages/商品圖片/"), fileName);

                image.SaveAs(path);

                prod.fPic = "/images/ProductImages/商品圖片/"+fileName;

                db.tProducts.Add(prod);

                db.SaveChanges();
            }
            else
            {
                db.tProducts.Add(prod);

                db.SaveChanges();
            }    

            return RedirectToAction("ProductList");
        }

        public ActionResult Delete(int fProductID)
        {
            var ordersdetail = (from od in db.tOrder_Detail
                                where od.fProductID == fProductID
                                select od).FirstOrDefault();

            if(ordersdetail!=null)
            {
                db.tOrder_Detail.Remove(ordersdetail);
            }


            var products = (from p in db.tProducts
                            where p.fProductID == fProductID
                            select p).FirstOrDefault();

            db.tProducts.Remove(products);

            db.SaveChanges();

            return RedirectToAction("ProductList");
        }


    



    }
}