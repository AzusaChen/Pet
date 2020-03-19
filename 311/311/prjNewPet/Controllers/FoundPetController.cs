using prjNewPet.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace prjNewPet.Controllers
{
    public class FoundPetController : Controller
    {


        dbNewPetEntities db = new dbNewPetEntities();
        CGetXName getxname = new CGetXName();


        public ActionResult ListFound()
        {
            SelectList citylist = new SelectList(getxname.getCity(), "fCityID", "fName");
            ViewBag.CITLIST = citylist;

            var mem = Session[CDictionary.SK_LOGINED_USER] as tMember;

            var q = db.tFoundPets.Where(m => m.fMemberID == mem.fMemberID);

            return View("ListFound", "_layout_Membercenter", q);

        }


        //上傳圖片
        public JsonResult Upload()
        {
            for (int i = 0; i < Request.Files.Count; i++)
            {
                HttpPostedFileBase file = Request.Files[i];

                string fileName = file.FileName;


                System.IO.Stream fileContent = file.InputStream;


                file.SaveAs(Server.MapPath("~/Images/") + fileName);
            }
            return Json("Uploaded " + Request.Files.Count + " files");
        }


        // GET: FoundPet
        /// <summary>
        /// NewFound
        /// </summary>
        ///


        //新增尋獲資料
        public void NewFound(tFoundPet foundpet)
        {
            CCompareToLostPet ctlp = new CCompareToLostPet();

            var mem = Session[CDictionary.SK_LOGINED_USER] as tMember;
            foundpet.fMemberID = mem.fMemberID;
            foundpet.fSignInDate = DateTime.Now;

            db.tFoundPets.Add(foundpet);
            db.SaveChanges();


            Task.Run(() => (ctlp.CLSorting(foundpet)));

        }



        public void editFound(tFoundPet foundpet)
        {
            var mem = Session[CDictionary.SK_LOGINED_USER] as tMember;

            var EditPet = (from p in db.tFoundPets
                           where p.fFoundPetID == foundpet.fFoundPetID
                           select p).FirstOrDefault();

            EditPet.fMemberID = mem.fMemberID;


            if (foundpet.fStatus == "")
            {
                EditPet.fStatus = EditPet.fStatus;
            }
            else
            {
                EditPet.fStatus = foundpet.fStatus;
            }


            if (foundpet.fFoundTime == null)
            {
                EditPet.fFoundTime = EditPet.fFoundTime;
            }
            else
            {
                EditPet.fFoundTime = foundpet.fFoundTime;
            }



            if (foundpet.fPetClassID == 0)
            {
                EditPet.fPetClassID = EditPet.fPetClassID;
            }
            else
            {
                EditPet.fPetClassID = foundpet.fPetClassID;
            }




            if (foundpet.fBreedID == 0)
            {
                EditPet.fBreedID = EditPet.fBreedID;
            }
            else
            {
                EditPet.fBreedID = foundpet.fBreedID;
            }




            if (foundpet.fCityID == 0)
            {
                EditPet.fCityID = EditPet.fCityID;
            }
            else
            {
                EditPet.fCityID = foundpet.fCityID;
            }





            if (foundpet.fRegionID == 0)
            {
                EditPet.fRegionID = EditPet.fRegionID;
            }
            else
            {
                EditPet.fRegionID = foundpet.fRegionID;
            }





            if (foundpet.fSkin == "")
            {
                EditPet.fSkin = EditPet.fSkin;
            }
            else
            {
                EditPet.fSkin = foundpet.fSkin;
            }




            if (foundpet.fPetPic == null)
            {
                EditPet.fPetPic = EditPet.fPetPic;
            }
            else
            {
                EditPet.fPetPic = foundpet.fPetPic;
            }




            if (foundpet.fChipID == "")
            {
                EditPet.fChipID = EditPet.fChipID;
            }
            else
            {
                EditPet.fChipID = foundpet.fChipID;
            }





            if (foundpet.fCollar == "")
            {
                EditPet.fCollar = EditPet.fCollar;
            }
            else
            {
                EditPet.fCollar = foundpet.fCollar;
            }




            if (foundpet.fGender == "")
            {
                EditPet.fGender = EditPet.fGender;
            }
            else
            {
                EditPet.fGender = foundpet.fGender;
            }


            if (foundpet.fRemark == "")
            {
                EditPet.fRemark = EditPet.fRemark;
            }
            else
            {
                EditPet.fRemark = foundpet.fRemark;
            }



            db.SaveChanges();


        }


        //刪除寵物會員
        public void deleteFound(int foundID)
        {
            var removedPet = db.tFoundPets.Where(p => p.fFoundPetID == foundID).FirstOrDefault();


            db.tFoundPets.Remove(removedPet);
            db.SaveChanges();

        }


        public JsonResult foundDetial(int foundpetID)
        {

            var pet = (from p in db.tFoundPets
                       where p.fFoundPetID == foundpetID
                       select p).FirstOrDefault();

            FoundPet foundDetila = new FoundPet
            {
                fStatus = pet.fStatus,
                fRemark = pet.fRemark,
                fFoundTime = pet.fFoundTime.ToShortDateString(),
                fGender = pet.fGender,
                fPetClassID = pet.fPetClassID,
                fBreedID = pet.fBreedID,
                fCityID = pet.fCityID,
                fRegionID = pet.fRegionID,
                fCollar = pet.fCollar,
                fChipID = pet.fChipID,
                fSkin = pet.fSkin,
                fPetPic = pet.fPetPic,

            };

            return Json(foundDetila);

        }



        //物種連動
        public JsonResult Breeds(string petclassid)
        {
            List<KeyValuePair<string, string>> items = new List<KeyValuePair<string, string>>();

            if (!string.IsNullOrEmpty(petclassid))
            {
                var breeds = this.getxname.getBreed(petclassid);
                if (breeds.Count() > 0)
                {
                    foreach (var breed in breeds)
                    {
                        items.Add(new KeyValuePair<string, string>(
                            breed.fBreedID.ToString(), breed.fName
                            ));
                    }
                }
            }
            return this.Json(items);
        }


        //地區連動
        public JsonResult Regions(string cityid)
        {
            List<KeyValuePair<string, string>> items = new List<KeyValuePair<string, string>>();

            if (!string.IsNullOrEmpty(cityid))
            {
                var regions = this.getxname.getRegion(cityid);
                if (regions.Count() > 0)
                {
                    foreach (var region in regions)
                    {
                        items.Add(new KeyValuePair<string, string>(
                            region.fRegionID.ToString(), region.fName
                            ));
                    }
                }
            }
            return this.Json(items);
        }



        public JsonResult MemberLevel()
        {
            tMember mem = Session[CDictionary.SK_LOGINED_USER] as tMember;

            var found = from f in db.tFoundPets
                        where f.fMemberID == mem.fMemberID
                        select f;

            var discuss = from d in db.tDiscussions
                        where d.fMemberID == mem.fMemberID
                        select d;


            int point = (found.ToList().Count * 100) + (discuss.ToList().Count * 3);



            return this.Json(point);
        }



    }

}

class FoundPet
{
    public string fStatus { get; set; }
    public string fRemark { get; set; }
    public string fFoundTime { get; set; }
    public string fGender { get; set; }
    public int fPetClassID { get; set; }
    public int fBreedID { get; set; }
    public int fCityID { get; set; }
    public int fRegionID { get; set; }
    public string fCollar { get; set; }
    public string fChipID { get; set; }
    public string fSkin { get; set; }
    public string fPetPic { get; set; }

}