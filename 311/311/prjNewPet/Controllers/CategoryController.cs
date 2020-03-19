using prjNewPet.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Linq.Dynamic;
using System.Data.Entity;

namespace prjNewPet.Controllers
{
    public class CategoryController : Controller
    {
        dbNewPetEntities db = new dbNewPetEntities();

        // GET: Category
        public ActionResult CategoryList()
        {
            return View("CategoryList", "_Layout_Backmanagement");
        }

        // POST: Categories
        [HttpPost]
        public JsonResult List(int jtStartIndex, int jtPageSize, string jtSorting)
        {
            string[] OrderByCondition = jtSorting.Split(new char[] { ' ' });

            string SortDirection = OrderByCondition[1].Equals("ASC") ? "Ascending" : "Descending";

            string Ordering = $"{OrderByCondition[0]} {SortDirection}";

            var query = db.tCategories.Select(c => new CategoryViewModel
            {
                fCategoryID = c.fCategoryID,
                fName = c.fName,
                fDescription = c.fDescription,
                
            });

            var RecordSet = query.OrderBy(Ordering).Skip(jtStartIndex).Take(jtPageSize);

            var result = Json(new { Result = "OK", Records = RecordSet, TotalRecordCount = query.Count() });

            result.MaxJsonLength = int.MaxValue;

            return result;
        }


        // POST: Categories/Create
        [HttpPost]
        public JsonResult Create([Bind(Include = "fCategoryID,fName,fDescription")] tCategory categories)
        {
            if (ModelState.IsValid)
            {
                db.tCategories.Add(categories);
                db.SaveChanges();
                return Json(new { Result = "OK", Record = categories });
            }
            else
            {
                return Json(new { Result = "Error", Message = "新增記錄失敗!" });
            }


        }


        // POST: Categories/Edit/5
        [HttpPost]
        public ActionResult Edit([Bind(Include = "fCategoryID,fName,fDescription")] tCategory categories)
        {
            if (ModelState.IsValid)
            {
                db.Entry(categories).State = EntityState.Modified;
                db.SaveChanges();
                return Json(new { Result = "OK", Record = categories });
            }
            else
            {
                return Json(new { Result = "Error", Message = "修改記錄失敗!" });
            }
        }


        // POST: Categories/Delete/5
        [HttpPost]
        public ActionResult Delete(int fCategoryID)
        {
            tCategory categories = db.tCategories.Find(fCategoryID);

            if (categories == null)
            {
                return Json(new { Result = "Error", Message = "找不到欲刪除紀錄!" });
            }
            else
            {
                db.tCategories.Remove(categories);
                db.SaveChanges();
                return Json(new { Result = "OK", Record = categories });
            }

        }




    }
}