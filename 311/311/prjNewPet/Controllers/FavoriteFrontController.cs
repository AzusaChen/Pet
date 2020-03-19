using prjNewPet.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace prjNewPet.Controllers
{
    public class FavoriteFrontController : Controller
    {
        // GET: FavoriteFront
        public ActionResult Favlist(int? articleid)
        {
            dbNewPetEntities db = new dbNewPetEntities();

            tFavorite fav = new tFavorite();

            var fid = Session[CDictionary.SK_LOGINED_USER] as tMember;

            int countarticle = db.tFavorites.Where(m => m.fArticleID == articleid && m.fMemberID == fid.fMemberID).Count();

            if (articleid != null && countarticle < 1)
            {
                fav.fArticleID = articleid;
                fav.fMemberID = ((tMember)Session[CDictionary.SK_LOGINED_USER]).fMemberID;
                db.tFavorites.Add(fav);
                db.SaveChanges();
            }

            return RedirectToAction("show");
        }

        [HttpPost]
        public JsonResult Regions()
        {
            List<Tfav> items = new List<Tfav>();

            var model = this.GetOrders();
            if (model.Count() > 0)
            {
                foreach (var item in model)
                {
                    Tfav fav = new Tfav
                    {
                        fArticleID = item.fArticleID,
                        fArticleTitle = item.tDiscussion.fTitle,
                        fMemberID = item.tDiscussion.tMember.fName,
                        fFavoriteID = item.fFavoriteID,
                        discussionchange = item.tDiscussion.tDiscussionClass.fName
                    };

                    items.Add(fav);
                }
            }

            return this.Json(items);
        }

        private IEnumerable<tFavorite> GetOrders()
        {
            dbNewPetEntities db = new dbNewPetEntities();

            List<tFavorite> favoritelist = null;

            if (Session[CDictionary.SK_LOGINED_USER] != null)
            {
                var fid = Session[CDictionary.SK_LOGINED_USER] as tMember;

                var query = from f in db.tFavorites
                            where f.fMemberID == fid.fMemberID
                            select f;

                favoritelist = query.ToList();
            }
            return favoritelist;
        }

        public ActionResult show()
        {
            dbNewPetEntities db = new dbNewPetEntities();

            List<tFavorite> favoritelist = null;

            if (Session[CDictionary.SK_LOGINED_USER] != null)
            {
                var fid = Session[CDictionary.SK_LOGINED_USER] as tMember;

                var query = from f in db.tFavorites
                            where f.fMemberID == fid.fMemberID
                            select f;

                favoritelist = query.ToList();
            }

            return View("show", "_layout_Membercenter", favoritelist);
        }

        public ActionResult FavoriteDelete(int? articleid)
        {
            dbNewPetEntities db = new dbNewPetEntities();

            var fid = Session[CDictionary.SK_LOGINED_USER] as tMember;

            var query = (from f in db.tFavorites
                         where f.fMemberID == fid.fMemberID && f.fArticleID == articleid
                         select f).FirstOrDefault();

            db.tFavorites.Remove(query);
            db.SaveChanges();

            return RedirectToAction("show");
        }

        public ActionResult Transfertosingle(int? articleid)
        {
            return RedirectToAction("Single", "Blog", new { articleid = articleid });
        }
    }
}