using prjNewPet.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace prjNewPet.Controllers
{
    public class ActivityController : Controller
    {
        dbNewPetEntities db = new dbNewPetEntities();

        // GET: Activity
        public ActionResult Index()
        {
           

            return View(db.tActivities);
        }


        public ActionResult ActivityList()
        {
           

            return View("ActivityList", "_Layout_Backmanagement", db.tActivities);
        }

        public ActionResult ActivityEdit(int ActID)
        {
           

            var act = (from a in db.tActivities
                       where a.fActivityID == ActID
                      select a).FirstOrDefault();

            return View("ActivityEdit", "_Layout_Backmanagement", act);
        }


        [HttpPost]
        public ActionResult ActivityEdit(tActivity activity, HttpPostedFileBase Image)
        {
           
            var act = (from a in db.tActivities
                       where a.fActivityID == activity.fActivityID
                       select a).FirstOrDefault();

            if (Image != null)
            {
                string fileName = Path.GetFileName(Image.FileName);
                string filePath = Path.Combine(Server.MapPath("~/images"), fileName);
                Image.SaveAs(filePath);

                act.fDMPic = fileName;
            }
            else
            {
                act.fDMPic = act.fDMPic;
            }
            


            act.fName = activity.fName;
            act.fEffectiveDate = activity.fEffectiveDate;
            act.fClosedDate = activity.fClosedDate;
            act.fContext = activity.fContext;


            db.SaveChanges();

            return RedirectToAction("ActivityList");   
        }

        public ActionResult ActivityCreate()
        {
            
            return View("ActivityCreate", "_Layout_Backmanagement");
        }

        [HttpPost]
        public ActionResult ActivityCreate(tActivity activity, HttpPostedFileBase Image)
        {
            
           
            string fileName = Path.GetFileName(Image.FileName);
            string filePath = Path.Combine(Server.MapPath("~/images"), fileName);
            Image.SaveAs(filePath);

            activity.fDMPic =  fileName;

            db.tActivities.Add(activity);
            db.SaveChanges();
            return RedirectToAction("ActivityList");
        }

        public ActionResult ActivityDelete(int ActID)
        {
           

            var act = (from a in db.tActivities
                       where a.fActivityID == ActID
                       select a).FirstOrDefault();

            db.tActivities.Remove(act);
            db.SaveChanges();

            return RedirectToAction("ActivityList");
        }
    }
}