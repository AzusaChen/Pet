using prjNewPet.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace prjNewPet.Controllers
{
    public class SentPetController : Controller
    {
        
        dbNewPetEntities db = new dbNewPetEntities();
        // GET: PetBreed
        public ActionResult SentPetList()
        {

            return View("SentPetList", "_Layout_Backmanagement", db.tPetMembers.Where(m=>m.fStatus=="SENT"));
        }

        public ActionResult SentPetCreate()
        {
            SelectList selectList = new SelectList(db.tPetClasses, "fPetClassID", "fName");
            ViewBag.SelectList = selectList;

            return View("SentPetCreate", "_Layout_Backmanagement");


        }





        [HttpPost]
        public JsonResult PetBreeds(string PetClassID)
        {
            List<KeyValuePair<string, string>> items = new List<KeyValuePair<string, string>>();

            if (!string.IsNullOrWhiteSpace(PetClassID))
            {
                var PetBreeds = this.GetOrders(PetClassID);
                if (PetBreeds.Count() > 0)
                {
                    foreach (var breed in PetBreeds)
                    {
                        items.Add(new KeyValuePair<string, string>
                            (
                            breed.fBreedID.ToString(),
                            breed.fName
                            ));
                    }
                }
            }
            return this.Json(items);
        }


        private IEnumerable<tBreed> GetOrders(string PetClassID)
        {
            using (dbNewPetEntities db = new dbNewPetEntities())
            {
                var query = db.tBreeds.Where(x => x.fPetClassID.ToString() == PetClassID);
                return query.ToList();
            }
        }



        public ActionResult SentPetEdit(int SentID)
        {
            
            SelectList selectList = new SelectList(db.tPetClasses, "fPetClassID", "fName");
            ViewBag.SelectList = selectList;

            return View("SentPetEdit", "_Layout_Backmanagement", db.tPetMembers.Where(m => m.fStatus == "SENT").FirstOrDefault());


        }



        

        
       
    }
}