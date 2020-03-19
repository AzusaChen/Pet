using prjNewPet.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace prjNewPet.Controllers
{
    public class BackCenterBlogController : Controller
    {
        // GET: BackCenterBlog
        public ActionResult BlogList()
        {
            return View("BlogList", "_Layout_Backmanagement");
        }

        [HttpPost]
        public JsonResult Regions()
        {
            List<Tdis> items = new List<Tdis>();

            
            var model = this.GetOrders();
            if (model.Count() > 0)
            {
                foreach (var item in model)
                {
                    Tdis dis = new Tdis
                    {
                        fArticleID = item.fArticleID,

                        fMemberID = item.tMember.fName,
                        fDiscussionClassID = item.tDiscussionClass.fName,
                        fTitle = item.fTitle,
                        fCreateDate = item.fCreateDate,
                        fReportcheck = item.fReportcheck,
                        fLock=item.fLock

                    };

                    

                    items.Add(dis);

                    
                }

            }


            var itemset = items.Select(p => new { fArticleID=p.fArticleID, fMemberID=p.fMemberID, fDiscussionClassID=p.fDiscussionClassID, fTitle=p.fTitle, fCreateDate = string.Format("{0:yyyy/MM/dd HH:mm:ss}", p.fCreateDate), fReportcheck=p.fReportcheck, fLock=p.fLock });

            return this.Json(itemset);
        }

        private IEnumerable<tDiscussion> GetOrders()
        {
            dbNewPetEntities db = new dbNewPetEntities();

            List<tDiscussion> discussionlist = null;

            var query = from f in db.tDiscussions             
                        select f;

            discussionlist = query.ToList();

            return discussionlist;
        }

        //public ActionResult Transfertosingle(int? articleid)
        //{
        //    return RedirectToAction("Single", "Blog", new { articleid = articleid });
        //}

        //======================================================================
        //getreportcheck details

        [HttpPost]
        public JsonResult getreport(int? rearticleid)
        {
            List<Trep> items = new List<Trep>();

            var model = this.GetRep(rearticleid);
            if (model.Count() > 0)
            {
                foreach (var item in model)
                {
                    Trep rep = new Trep
                    {
                        fReportID = item.fReportID,
                        fArticleID = item.tDiscussion.fTitle,

                        fMemberID = item.tMember.fEMail,
                        fReportComment = item.fReportComment
                    };

                    items.Add(rep);
                }
            }

            return this.Json(items);
        }

        private IEnumerable<tReport> GetRep(int? rearticleid)
        {
            dbNewPetEntities db = new dbNewPetEntities();

            List<tReport> reportlist = null;

            var query = from f in db.tReports
                        where f.fArticleID == rearticleid
                        select f;

            reportlist = query.ToList();

            return reportlist;
        }

        //======================================================================
        //Lock

        [HttpPost]
        public JsonResult getLock(int? lockid)
        {
            dbNewPetEntities db = new dbNewPetEntities();

            var query = (from f in db.tDiscussions
                        where f.fArticleID == lockid
                        select f).FirstOrDefault();

            query.fLock = true;

            db.SaveChanges();


            return Json("done");
        }





    }
}