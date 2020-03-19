using prjNewPet.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace prjNewPet.Controllers
{
    public class ContentFrontController : Controller
    {
        // GET: ContentFront
        public ActionResult Index()
        {
           

            return View("Index", "_layout_Membercenter");
        }


        [HttpPost]
        public JsonResult getalldis()
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
                       
                        fDiscussionClassID = item.tDiscussionClass.fName,
                        fTitle = item.fTitle,
                        fCreateDate = item.fCreateDate,
                       

                    };



                    items.Add(dis);


                }

            }


            var itemset = items.Select(p => new { fArticleID = p.fArticleID, fDiscussionClassID = p.fDiscussionClassID, fTitle = p.fTitle, fCreateDate = string.Format("{0:yyyy/MM/dd HH:mm:ss}", p.fCreateDate)});

            return this.Json(itemset);
        }

        private IEnumerable<tDiscussion> GetOrders()
        {
            dbNewPetEntities db = new dbNewPetEntities();

            List<tDiscussion> discussionlist = null;

            var fid = Session[CDictionary.SK_LOGINED_USER] as tMember;

            var query = from f in db.tDiscussions
                        where f.fMemberID == fid.fMemberID
                        select f;

            discussionlist = query.ToList();

            return discussionlist;
        }



        //public ActionResult Transfertosingle(int? articleid)
        //{
        //    return RedirectToAction("Single", "Blog", new { articleid = articleid });
        //}

        //============================================================================

        [HttpPost]
        public JsonResult gettime(DateTime? date1, DateTime? date2)
        {
            dbNewPetEntities db = new dbNewPetEntities();

            string datefirst = Convert.ToDateTime(date1).ToShortDateString();
            string datelast = Convert.ToDateTime(date2).ToShortDateString();

            DateTime datef = Convert.ToDateTime(datefirst);
            DateTime datel = Convert.ToDateTime(datelast);

            List<Tdis> items = new List<Tdis>();



            if (Session[CDictionary.SK_LOGINED_USER] != null && date1 != null && date2 != null)
            {
                var fid = Session[CDictionary.SK_LOGINED_USER] as tMember;

                var query = from d in db.tDiscussions
                            where (d.fMemberID == fid.fMemberID) && (DbFunctions.TruncateTime(d.fCreateDate) >= datef && DbFunctions.TruncateTime(d.fCreateDate) <= datel)
                            select d;
                //利用(DbFunctions.TruncateTime 運用在linq to Entity
                //轉譯Sql 的日期去掉後面的時間

                if(query.Count() > 0)
                {

                    foreach (var item in query)
                    {
                        Tdis dis = new Tdis
                        {
                            fArticleID = item.fArticleID,

                            fDiscussionClassID = item.tDiscussionClass.fName,
                            fTitle = item.fTitle,
                            fCreateDate = item.fCreateDate,

                        };

                        items.Add(dis);

                    }


                }

                
               
            }
            else
            {
                var fid = Session[CDictionary.SK_LOGINED_USER] as tMember;

                var query = from d in db.tDiscussions
                            where d.fMemberID == fid.fMemberID
                            select d;

                if (query.Count() > 0)
                {
                    foreach (var item in query)
                    {
                        Tdis dis = new Tdis
                        {
                            fArticleID = item.fArticleID,
                            fDiscussionClassID = item.tDiscussionClass.fName,
                            fTitle = item.fTitle,
                            fCreateDate = item.fCreateDate,

                        };

                        items.Add(dis);

                    }

                }




            }

            var itemset = items.Select(p => new { fArticleID = p.fArticleID, fDiscussionClassID = p.fDiscussionClassID, fTitle = p.fTitle, fCreateDate = string.Format("{0:yyyy/MM/dd HH:mm:ss}", p.fCreateDate) });

            return Json(itemset);
        }

        public ActionResult CreateAr()
        {
            SelectList selectList = new SelectList(this.GetDiscussionclass(), "fDiscussionClassID", "fName");
            ViewBag.SelectList = selectList;

            ViewBag.time = DateTime.Now;

            return View("CreateAr", "_layout_Membercenter");//index沒有值 這裡不需傳值
        }

        private IEnumerable<tDiscussionClass> GetDiscussionclass()
        {
            using (dbNewPetEntities db = new dbNewPetEntities())//釋放記憶體
            {
                var query = db.tDiscussionClasses.OrderBy(m => m.fDiscussionClassID);

                return query.ToList();
            }
        }

        [HttpPost]
        public ActionResult CreateAr(tDiscussion discussion)
        {
            discussion.fMemberID = ((tMember)Session[CDictionary.SK_LOGINED_USER]).fMemberID;

            discussion.fCreateDate = DateTime.Now;

            if (discussion.fTitle == null)
            {
                ViewBag.errormessgetitle = "<文章標題為必填項目>";

                SelectList selectList = new SelectList(this.GetDiscussionclass(), "fDiscussionClassID", "fName");
                ViewBag.SelectList = selectList;

                ViewBag.time = DateTime.Now;

                return View("CreateAr", "_layout_Membercenter");
            }
            else if (discussion.fDiscussionClassID == 0)
            {
                ViewBag.errormessgedisclass = "<請選擇分類看板>";

                SelectList selectList = new SelectList(this.GetDiscussionclass(), "fDiscussionClassID", "fName");
                ViewBag.SelectList = selectList;

                ViewBag.time = DateTime.Now;

                return View("CreateAr", "_layout_Membercenter");
            }
            else if (discussion.fContent == null)
            {
                ViewBag.errormessgeContent = "<內容為必填項目>";

                SelectList selectList = new SelectList(this.GetDiscussionclass(), "fDiscussionClassID", "fName");
                ViewBag.SelectList = selectList;

                ViewBag.time = DateTime.Now;

                return View("CreateAr", "_layout_Membercenter");
            }
            else
            {
                dbNewPetEntities db = new dbNewPetEntities();
                discussion.fLike = 0;
                discussion.fReportcheck = false;
                discussion.fLock = false;

                db.tDiscussions.Add(discussion);
                db.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        public ActionResult ContentEdit(int articleid)
        {
            dbNewPetEntities db = new dbNewPetEntities();

            tDiscussion dis = null;

            SelectList selectList = new SelectList(this.GetDiscussionclass(), "fDiscussionClassID", "fName");
            ViewBag.SelectList = selectList;

            ViewBag.time = DateTime.Now;

            if (Session[CDictionary.SK_LOGINED_USER] != null)
            {
                var fid = Session[CDictionary.SK_LOGINED_USER] as tMember;

                var query = (from d in db.tDiscussions
                             where d.fMemberID == fid.fMemberID && d.fArticleID == articleid
                             select d).FirstOrDefault();

                dis = query;
            }

            return View("ContentEdit", "_layout_Membercenter", dis);
        }

        [HttpPost]
        public ActionResult ContentEdit(tDiscussion discussion)
        {
            dbNewPetEntities db = new dbNewPetEntities();

            var fid = Session[CDictionary.SK_LOGINED_USER] as tMember;

            var query = (from d in db.tDiscussions
                         where d.fMemberID == fid.fMemberID && d.fArticleID == discussion.fArticleID
                         select d).FirstOrDefault();

            query.fArticleID = discussion.fArticleID;

            query.fTitle = discussion.fTitle;
            query.fContent = discussion.fContent;
            query.fCreateDate = DateTime.Now;

            if (discussion.fDiscussionClassID == 0)
            {
                query.fDiscussionClassID = query.fDiscussionClassID;
            }
            else
            {
                query.fDiscussionClassID = discussion.fDiscussionClassID;
            }

            db.SaveChanges();

            return RedirectToAction("Index");
        }

        [HttpPost]
        public JsonResult ContentDelete(int articleid)
        {
            dbNewPetEntities db = new dbNewPetEntities();

            var fid = Session[CDictionary.SK_LOGINED_USER] as tMember;

            //=======================================================
            var commentd = (from c in db.tComments
                            where c.fArticleID == articleid
                            select c).ToList();

            foreach (var item in commentd)
            {
                db.tComments.Remove(item);
                db.SaveChanges();
            }

            //=======================================================
            var favoritelist = (from c in db.tFavorites
                                where c.fArticleID == articleid
                                select c).ToList();

            foreach (var item in favoritelist)
            {
                db.tFavorites.Remove(item);
                db.SaveChanges();
            }

            //=======================================================
            var reportlist = (from c in db.tReports
                              where c.fArticleID == articleid
                              select c).ToList();

            foreach (var item in reportlist)
            {
                db.tReports.Remove(item);
                db.SaveChanges();
            }

            //=======================================================

            var likelist = (from c in db.tLikes
                            where c.fArticleID == articleid
                            select c).ToList();

            foreach (var item in likelist)
            {
                db.tLikes.Remove(item);
                db.SaveChanges();
            }

            //=======================================================
            var query = (from d in db.tDiscussions
                         where d.fMemberID == fid.fMemberID && d.fArticleID == articleid
                         select d).FirstOrDefault();

            db.tDiscussions.Remove(query);
            db.SaveChanges();

            return Json("done");
        }
    }
}