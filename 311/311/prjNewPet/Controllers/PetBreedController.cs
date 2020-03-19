using prjNewPet.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace prjNewPet.Controllers
{
    public class PetBreedController : Controller
    {
        dbNewPetEntities db = new dbNewPetEntities();
        // GET: PetBreed
        public ActionResult BreedList()
        {

            return View("BreedList", "_Layout_Backmanagement", db.tBreeds.OrderBy(b=>b.fBreedID));
        }

        public ActionResult BreedCreate()
        {
            SelectList selectList = new SelectList(db.tPetClasses, "fPetClassID", "fName");
            ViewBag.SelectList = selectList;

            return View("BreedCreate", "_Layout_Backmanagement");


        }

        [HttpPost]
        public ActionResult BreedCreate(tBreed breed, HttpPostedFileBase Image)
        {
            string fileName = Path.GetFileName(Image.FileName);
            string filePath = Path.Combine(Server.MapPath("~/images/PetBreedImg"), fileName);
            Image.SaveAs(filePath);

            breed.fPic = fileName;

            db.tBreeds.Add(breed);
            db.SaveChanges();

            return RedirectToAction("BreedList");
        }


        public ActionResult BreedEdit(int BreedID)
        {
            

            SelectList selectList = new SelectList(db.tPetClasses, "fPetClassID", "fName");
            ViewBag.SelectList = selectList;

            return View("BreedEdit", "_Layout_Backmanagement",(db.tBreeds.Where(b=>b.fBreedID==BreedID)).FirstOrDefault());


        }

        [HttpPost]
        public ActionResult BreedEdit(tBreed breed, HttpPostedFileBase Image)
        {
            var pb = (from b in db.tBreeds
                     where b.fBreedID == breed.fBreedID
                     select b).FirstOrDefault();

            string fileName = Path.GetFileName(Image.FileName);
            string filePath = Path.Combine(Server.MapPath("~/images/PetBreedImg"), fileName);
            Image.SaveAs(filePath);



            pb.fName = breed.fName;
            pb.fPic = fileName;
            pb.fPetClassID = breed.fPetClassID;

            db.SaveChanges();

            return RedirectToAction("BreedList");
        }


        public ActionResult BreedDelete(int BreedID)
        {

            var pb = (from b in db.tBreeds
                     where b.fBreedID == BreedID
                     select b).FirstOrDefault();

            db.tBreeds.Remove(pb);
            db.SaveChanges();

            return RedirectToAction("BreedList");


        }
    }
}