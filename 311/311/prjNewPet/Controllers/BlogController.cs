using PagedList;
using prjNewPet.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace prjNewPet.Controllers
{
    public class BlogController : Controller
    {
        // GET: Blog

        private int pageSize = 7;

        public ActionResult Total(int? disid, int page = 1)
        {
            dbNewPetEntities db = new dbNewPetEntities();

            List<tDiscussion> dis = null;

            int currentpage = page < 1 ? 1 : page;

            if (disid != null)
            {
                ViewBag.disname = ":" + db.tDiscussionClasses.Where(m => m.fDiscussionClassID == disid)
               .FirstOrDefault().fName;

                ViewBag.identity = disid;//用來儲存id給pagelist用
            }

            if (disid == null)
            {
                var query = from d in db.tDiscussions
                            orderby d.fCreateDate descending
                            select d;

                dis = query.ToList();
            }
            else if (disid == 1)
            {
                var query1 = db.tDiscussions.Where(m => m.fDiscussionClassID == disid).OrderByDescending(k => k.fCreateDate).ToList();
                dis = query1.ToList();
            }
            else if (disid == 2)
            {
                var query2 = db.tDiscussions.Where(m => m.fDiscussionClassID == disid).OrderByDescending(k => k.fCreateDate).ToList();
                dis = query2.ToList();
            }
            else
            {
                var query3 = db.tDiscussions.Where(m => m.fDiscussionClassID == 3).OrderByDescending(k => k.fCreateDate).ToList();
                dis = query3.ToList();
            }

            var result = dis.ToPagedList(currentpage, pageSize);

            //CDiscussion vm = new CDiscussion()
            //{
            //    DiscussionClass = db.tDiscussionClasses.ToList(),
            //    Discussion = db.tDiscussions.Where(m => m.fDiscussionClassID == disid).OrderByDescending(k => k.fCreateDate).ToList()

            //};

            //vm.Discussion.ToPagedList(currentpage, pageSize);

            return View("Total", "_layout", result);
        }

        public ActionResult Single(int? articleid)
        {
            dbNewPetEntities db = new dbNewPetEntities();

            var query = (from dis in db.tDiscussions
                         where dis.fArticleID == articleid
                         select dis).FirstOrDefault();

            var likecount = from likecheck in db.tLikes.AsEnumerable()
                            where likecheck.fArticleID == articleid && likecheck.fMemberID == ((tMember)Session[CDictionary.SK_LOGINED_USER]).fMemberID
                            select likecheck;

            ViewBag.count = likecount.Count();

            ViewBag.usercomment = ((tMember)Session[CDictionary.SK_LOGINED_USER]).fNickName;

            return View("Single", "_layout", query);
        }

        //==============================================================
        [HttpPost]
        public JsonResult savecomment(int? articleids, string commentss)
        {
            dbNewPetEntities db = new dbNewPetEntities();
            if (commentss != null)
            {
                tComment comment = new tComment();

                comment.fMemberID = ((tMember)Session[CDictionary.SK_LOGINED_USER]).fMemberID;
                comment.fArticleID = articleids;
                comment.fCreateDate = DateTime.Now;
                comment.fContent = commentss;

                db.tComments.Add(comment);

                db.SaveChanges();
            }
            return Json("done");
        }

        //==============================================================
        //getcommentforall

        [HttpPost]
        public JsonResult getallcomment(int? rearticleid)
        {
            List<Tcom> items = new List<Tcom>();

            var model = this.Getallcom(rearticleid);
            if (model.Count() > 0)
            {
                foreach (var item in model)
                {
                    Tcom rep = new Tcom
                    {
                        fIcon = item.tMember.fIcon,
                        fMemberNickname = item.tMember.fNickName,
                        fContent = item.fContent,
                        fCreateDate = item.fCreateDate
                    };

                    items.Add(rep);
                }
            }
            var itemset = items.Select(p => new { fIcon = p.fIcon, fMemberNickname = p.fMemberNickname, fContent = p.fContent, fCreateDate = string.Format("{0:yyyy/MM/dd HH:mm:ss}", p.fCreateDate) });
            return this.Json(itemset);
        }

        private IEnumerable<tComment> Getallcom(int? rearticleid)
        {
            dbNewPetEntities db = new dbNewPetEntities();

            List<tComment> commentlist = null;

            var query = from f in db.tComments
                        where f.fArticleID == rearticleid
                        select f;

            commentlist = query.ToList();

            return commentlist;
        }

        //==============================================================
        //getcommentforone
        [HttpPost]
        public JsonResult getcomment(int? rearticleid)
        {
            List<Tcom> items = new List<Tcom>();
            dbNewPetEntities db = new dbNewPetEntities();
            var query = (from f in db.tComments.AsEnumerable()
                         where f.fArticleID == rearticleid && f.fMemberID == ((tMember)Session[CDictionary.SK_LOGINED_USER]).fMemberID
                         orderby f.fCreateDate descending
                         select f).FirstOrDefault();

            Tcom rep = new Tcom
            {
                fIcon = query.tMember.fIcon,
                fMemberNickname = query.tMember.fNickName,
                fContent = query.fContent,
                fCreateDate = query.fCreateDate
            };

            items.Add(rep);

            var itemset = items.Select(p => new { fIcon = p.fIcon, fMemberNickname = p.fMemberNickname, fContent = p.fContent, fCreateDate = string.Format("{0:yyyy/MM/dd HH:mm:ss}", p.fCreateDate) });
            return this.Json(itemset);
        }

        //==============================================================
        [HttpPost]
        public ActionResult Report(tReport report, int? articleids)
        {
            dbNewPetEntities db = new dbNewPetEntities();
            report.fMemberID = ((tMember)Session[CDictionary.SK_LOGINED_USER]).fMemberID;
            report.fArticleID = (int)articleids;
            report.fReportComment = report.fReportComment;

            var query = (from dis in db.tDiscussions
                         where dis.fArticleID == articleids
                         select dis).FirstOrDefault();

            query.fReportcheck = true;

            db.tReports.Add(report);

            db.SaveChanges();

            System.Threading.Thread.Sleep(3000);

            return RedirectToAction("Single", new { articleid = articleids });
        }

        public ActionResult Lockpage()
        {
            return View("Lockpage", "_Layout_Lockpage");
        }

        //==================================================

        [HttpPost]
        public JsonResult checklike(int? articleid)
        {
            dbNewPetEntities db = new dbNewPetEntities();

            var likecount = from likecheck in db.tLikes.AsEnumerable()
                            where likecheck.fArticleID == articleid && likecheck.fMemberID == ((tMember)Session[CDictionary.SK_LOGINED_USER]).fMemberID
                            select likecheck;

            int count = likecount.Count();

            return Json(count);
        }

        //===================================================

        [HttpPost]
        public JsonResult savelike(int? articleid, int? likec)
        {
            dbNewPetEntities db = new dbNewPetEntities();

            var likecount = from likecheck in db.tLikes.AsEnumerable()
                            where likecheck.fArticleID == articleid && likecheck.fMemberID == ((tMember)Session[CDictionary.SK_LOGINED_USER]).fMemberID
                            select likecheck;

            int count = likecount.Count();

            if (count == 0)
            {
                tLike like = new tLike();
                like.fMemberID = ((tMember)Session[CDictionary.SK_LOGINED_USER]).fMemberID;
                like.fArticleID = articleid;

                var query = (from dis in db.tDiscussions
                             where dis.fArticleID == articleid
                             select dis).FirstOrDefault();

                query.fLike = likec;

                db.tLikes.Add(like);
                db.SaveChanges();
            }

            return Json("done");
        }

        //================================================
        [HttpPost]
        public JsonResult showlike(int? articleid)
        {
            List<Tdis> items = new List<Tdis>();

            var model = this.Getlikes(articleid);
            if (model.Count() > 0)
            {
                foreach (var item in model)
                {
                    Tdis dis = new Tdis
                    {
                        fLike = item.fLike
                    };

                    items.Add(dis);
                }
            }

            return Json(items);
        }

        private IEnumerable<tDiscussion> Getlikes(int? articleid)
        {
            dbNewPetEntities db = new dbNewPetEntities();

            List<tDiscussion> likelist = null;

            var query = from f in db.tDiscussions
                        where f.fArticleID == articleid
                        select f;

            likelist = query.ToList();

            return likelist;
        }

        //================================================

        [HttpPost]
        public JsonResult minuslike(int? articleid, int? likec)
        {
            dbNewPetEntities db = new dbNewPetEntities();

            var querylike = (from like in db.tLikes.AsEnumerable()
                             where like.fMemberID == ((tMember)Session[CDictionary.SK_LOGINED_USER]).fMemberID && like.fArticleID == articleid
                             select like).FirstOrDefault();

            var querydis = (from dis in db.tDiscussions
                            where dis.fArticleID == articleid
                            select dis).FirstOrDefault();

            querydis.fLike = likec;

            if (querylike != null)
            {
                db.tLikes.Remove(querylike);
                db.SaveChanges();
            }

            return Json("done");
        }
    }
}