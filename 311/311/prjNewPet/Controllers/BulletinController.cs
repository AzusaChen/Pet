
using prjNewPet.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace prjMyPet.Controllers
{
    public class BulletinController : Controller
    {
        dbNewPetEntities db = new dbNewPetEntities();

        public ActionResult BulletinForLostPet()
        {
            List<tLostPet> lp = null;

            var list = from l in db.tLostPets
                       select l;

            lp = list.ToList();

            return View(lp);
        }

        [HttpPost]
        public ActionResult BulletinForLostPet(int classID, string cityName, string regionName)
        {
            CBulletinHome cb = new CBulletinHome();

            List<tLostPet> lp = cb.multiSearchForlost(classID, cityName, regionName);

            return View(lp);
        }


        public ActionResult BulletinForFoundPet()
        {
            List<tFoundPet> fp = null;

            var list = from l in db.tFoundPets
                       select l;

            fp = list.ToList();

            return View(fp);
        }

        [HttpPost]
        public ActionResult BulletinForFoundPet(int classID, string cityName, string regionName)
        {
            CBulletinHome cb = new CBulletinHome();

            List<tFoundPet> fp = cb.multiSearchForFound(classID, cityName, regionName);

            return View(fp);
        }







        public ActionResult LostDetail(int lostID)
        {

            var list = (from l in db.tLostPets
                        where l.tPetMember.fPetID == lostID
                        select l).FirstOrDefault();


            return View("LostDetail", "_Layout_PetDetial", list);


        }

        public ActionResult FoundDetail(int foundID)
        {

            var list = (from l in db.tFoundPets
                        where l.fFoundPetID == foundID
                        select l).FirstOrDefault();


            return View("FoundDetail", "_Layout_PetDetial", list);


        }

        public ActionResult FoundDetailForMem(int foundID)
        {

            var list = (from l in db.tFoundPets
                        where l.fFoundPetID == foundID
                        select l).FirstOrDefault();


            return View("FoundDetailForMem", "_Layout_PetDetial", list);


        }



        public ActionResult BulletinForSentPet()
        {
            List<tPetMember> SentPet = db.tPetMembers.Where(p => p.fStatus == "SENT").ToList();


            return View(SentPet);
        }
    }
}