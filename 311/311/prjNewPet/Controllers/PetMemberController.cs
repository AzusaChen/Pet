using prjNewPet.Models;
using prjNewPet.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace temp.Controllers
{
    public class PetMemberController : Controller
    {
        dbNewPetEntities db = new dbNewPetEntities();
        CGetXName getxname = new CGetXName();


        //================================================主頁面與部分檢視============================================================

        // GET: PetMember
        public ActionResult PetMemlist()
        {
            SelectList citylist = new SelectList(getxname.getCity(), "fCityID", "fName");
            ViewBag.CITLIST = citylist;

            var mem = Session[CDictionary.SK_LOGINED_USER] as tMember;


            if (mem != null)
            {
            var petHave = from p in db.tPetMembers
                          where p.fMemberID == mem.fMemberID
                          where p.fStatus == "HAVE"
                          select p;


                return View("PetMemlist", "_layout_Membercenter", petHave);
            }
            else
            {

                return RedirectToAction("Auth", "Login");
            }


           
        }

        //走失的部分檢視
        public ActionResult PetMemLost()
        {
            var mem = Session[CDictionary.SK_LOGINED_USER] as tMember;

            var LostPet = from p in db.tPetMembers
                          where p.fMemberID == mem.fMemberID
                          where p.fStatus == "LOST"
                          select p;

            return PartialView("_PetMemLost", LostPet);
        }

        //送養的部分檢視
        public ActionResult PetMemSent()
        {
            var mem = Session[CDictionary.SK_LOGINED_USER] as tMember;

            var SentPet = from p in db.tPetMembers
                          where p.fMemberID == mem.fMemberID
                          where p.fStatus == "SENT"
                          select p;

            return PartialView("_PetMemSent", SentPet);
        }

        //尋獲的部分檢視
        public ActionResult FoundPetList(int PetID)
        {
            List<tFoundPet> samebreedFoundPet = new List<tFoundPet>();

            if (PetID != 0)
            {
           int petBreedID = (db.tPetMembers.Where(p => p.fPetID == PetID)).FirstOrDefault().fBreedID;
                        
            samebreedFoundPet = db.tFoundPets.Where(f=>f.fBreedID==petBreedID).ToList();
            }

 

            return PartialView("_FoundPetList", samebreedFoundPet);
        }

        //認養訊息的部分檢視
        public ActionResult PetMemAdopt()
        {
            var mem = Session[CDictionary.SK_LOGINED_USER] as tMember;

            var adopt = db.tAdoptMessages.Where(r => r.tPetMember.fMemberID==mem.fMemberID);

            return PartialView("_PetMemAdopt", adopt);
        }

        //================================================改寵物狀態==============================================================

        //改狀態成擁有
        public void ToHave(int HID)
        {
            var q = (from p in db.tPetMembers
                     where p.fPetID == HID
                     select p).FirstOrDefault();

            q.fStatus = "HAVE";

            //刪除所有送養申請
            var adoptmessage = from m in db.tAdoptMessages
                               where m.fPetID == q.fPetID
                               select m;



            if (adoptmessage != null)
            {

                foreach (var a in adoptmessage)
                {
                    db.tAdoptMessages.Remove(a);
                }

            }


            db.SaveChanges();

        }


        //改狀態成走失
        public void ToLost(int LID)
        {
            var q = (from p in db.tPetMembers
                    where p.fPetID == LID
                    select p).FirstOrDefault();

            q.fStatus = "LOST";

            db.SaveChanges();


        }

        //改狀態成送養
        public void ToSent(int SID)
        {
            var q = (from p in db.tPetMembers
                     where p.fPetID == SID
                     select p).FirstOrDefault();

            q.fStatus = "SENT";

            db.SaveChanges();

        }


        //==========================================增刪修=====================================================================================


        //新增寵物會員
        public void addNewPetMember(tPetMember petMember)
        {
            var mem = Session[CDictionary.SK_LOGINED_USER] as tMember;
            petMember.fMemberID = mem.fMemberID;

            db.tPetMembers.Add(petMember);
            db.SaveChanges();

        }

        //修改寵物會員
        public void editPetMember(tPetMember petMember)
        {
            var mem = Session[CDictionary.SK_LOGINED_USER] as tMember;

            var EditPet = (from p in db.tPetMembers
                           where p.fPetID == petMember.fPetID
                           select p).FirstOrDefault();



            if (petMember.fPetName == "")
            {
                EditPet.fPetName = EditPet.fPetName;
            }
            else
            {
                EditPet.fPetName = petMember.fPetName;
            }


            EditPet.fMemberID = mem.fMemberID;



            if (petMember.fPetClassID == 0)
            {
                EditPet.fPetClassID = EditPet.fPetClassID;
            }
            else
            {
                EditPet.fPetClassID = petMember.fPetClassID;
            }




            if (petMember.fBreedID == 0)
            {
                EditPet.fBreedID = EditPet.fBreedID;
            }
            else
            {
                EditPet.fBreedID = petMember.fBreedID;
            }




            if (petMember.fCityID == 0)
            {
                EditPet.fCityID = EditPet.fCityID;
            }
            else
            {
                EditPet.fCityID = petMember.fCityID;
            }





            if (petMember.fRegionID == 0)
            {
                EditPet.fRegionID = EditPet.fRegionID;
            }
            else
            {
                EditPet.fRegionID = petMember.fRegionID;
            }





            if (petMember.fSkin == "")
            {
                EditPet.fSkin = EditPet.fSkin;
            }
            else
            {
                EditPet.fSkin = petMember.fSkin;
            }




            if (petMember.fPetPic == null)
            {
                EditPet.fPetPic = EditPet.fPetPic;
            }
            else
            {
                EditPet.fPetPic = petMember.fPetPic;
            }




            if (petMember.fChipID == "")
            {
                EditPet.fChipID = EditPet.fChipID;
            }
            else
            {
                EditPet.fChipID = petMember.fChipID;
            }





            if (petMember.fCollar == "")
            {
                EditPet.fCollar = EditPet.fCollar;
            }
            else
            {
                EditPet.fCollar = petMember.fCollar;
            }




            if (petMember.fGender == "")
            {
                EditPet.fGender = EditPet.fGender;
            }
            else
            {
                EditPet.fGender = petMember.fGender;
            }




            EditPet.fStatus = EditPet.fStatus;

            db.SaveChanges();


        }


        //傳回寵物詳細資料
        public JsonResult petMemberDetial(int petMemberID)
        {

            var pet = (from p in db.tPetMembers
                       where p.fPetID == petMemberID
                       select p).FirstOrDefault();

            tPetMember petDetila = new tPetMember
            {
                fPetName = pet.fPetName,
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

            return Json(petDetila);

        }

        //刪除寵物會員
        public void deletePetMember(int petID)
        {
            var removedPet = db.tPetMembers.Where(p => p.fPetID == petID).FirstOrDefault();


            db.tPetMembers.Remove(removedPet);
            db.SaveChanges();

        }




        //新增走失寵物資料
        public void addNewLost(tLostPet lostPet)
        {
            lostPet.fSignInDate = DateTime.Now;

            db.tLostPets.Add(lostPet);
            db.SaveChanges();


        }

        //修改走失寵物資料
        public void editLost(tLostPet lostPet)
        {
            var EditPet = (from p in db.tLostPets
                           where p.fPetID == lostPet.fPetID
                           select p).FirstOrDefault();

            EditPet.fLostPlace = lostPet.fLostPlace;
            EditPet.fLostTime = lostPet.fLostTime;
            EditPet.fRemark = lostPet.fRemark;
            EditPet.fReward = lostPet.fReward;
            EditPet.fCompareLevel = lostPet.fCompareLevel;

            EditPet.fSignInDate = EditPet.fSignInDate;

            db.SaveChanges();


        }



        //移除走失資料
        public void removeLost(int petID)
        {

            string petState = isLostExit(petID);

            if (petState == "LOST")
            {

                var q = (db.tLostPets.Where(l => l.fPetID == petID)).FirstOrDefault();

                db.tLostPets.Remove(q);
                db.SaveChanges();
            }

        }
        //====================================================領養申請========================================================================

        //領養申請
        public JsonResult PetAdoptApply(int PetID, string content)
        {
            var mem = Session[CDictionary.SK_LOGINED_USER] as tMember;

            //是否登入
            if(mem != null)
            {

                var hadadopted = (from h in db.tAdoptMessages
                                 where h.fMemberID == mem.fMemberID
                                 select h).FirstOrDefault();


                //是否申請超過一筆以上
                if (hadadopted != null)
                {
                    return Json(4);

                }
                else
                {
                    var sentPet = (from s in db.tPetMembers
                                   where s.fPetID == PetID
                                   select s).FirstOrDefault();

                    if (sentPet.fMemberID != mem.fMemberID)
                    {
                        tAdoptMessage am = new tAdoptMessage()
                        {
                            fPetID = PetID,
                            fMemberID = mem.fMemberID,
                            fContent = content
                        };

                        db.tAdoptMessages.Add(am);
                        db.SaveChanges();

                        return Json(1);
                    }

                    return Json(3);

                }


            }
            else
            {
                return Json(2);
            }

        }

        //收回申請
        public void PetAdoptRetract(int PetID)
        {
            var mem = Session[CDictionary.SK_LOGINED_USER] as tMember;



                var retract = (from r in db.tAdoptMessages
                               where r.fMemberID==mem.fMemberID
                               select r).FirstOrDefault();


                db.tAdoptMessages.Remove(retract);
                db.SaveChanges();

        }

        //申請詳細資料
        public JsonResult ApplierDetial(int ApplyID)
        {
            
             var applier = (from s in db.tMembers
                            where s.fMemberID == ApplyID
                            select s).FirstOrDefault();


            ApplierData ad = new ApplierData()
            {
                ApplierPic=applier.fIcon,
                ApplierName = applier.fName,
                ApplierPhone = applier.fPhone,
                ApplierIncome = applier.fEnconomicStatus,
                ApplierCity = applier.tCity.fName,
                ApplierRegion = applier.tRegion.fName


            };


            return Json(ad);
        }

        //申請者寵物資料
        public JsonResult ApplierPetDetial(int ApplyID)
        {

            var pets = from p in db.tPetMembers
                       where p.fMemberID == ApplyID
                       select p;

            List<ApplierPetData> apds = new List<ApplierPetData>();
            
            foreach(var pet in pets)
            {
                ApplierPetData apd = new ApplierPetData()
                {
                    PetName = pet.fPetName,
                    PetBreed = pet.tBreed.fName,
                    PetPic = pet.fPetPic
                };

                apds.Add(apd);
            }
            


            return Json(apds);
        }

        //更改飼主ID，並刪除其他它申請資料
        public void AllowAdopt(int applierID)
        {
            var q = (from p in db.tAdoptMessages
                    where p.fMemberID == applierID
                    select p.tPetMember).FirstOrDefault();
            

            var app = (from a in db.tMembers
                      where a.fMemberID == applierID
                      select a).FirstOrDefault();


            q.fMemberID = applierID;
            q.fCityID = app.fCityID;
            q.fRegionID = app.fRegionID;
            q.fStatus = "HAVE";

            var adoptmessage = from m in db.tAdoptMessages
                               where m.fPetID == q.fPetID
                               select m;

            foreach (var a in adoptmessage)
            {
                db.tAdoptMessages.Remove(a);
            }

            db.SaveChanges();

        }




        //======================================================其他功能======================================================================

        //傳送所有走失資料
        public JsonResult LostExited(string lost)
        {
            var mem = Session[CDictionary.SK_LOGINED_USER] as tMember;

            List<int> exitLpID =new List<int>();

            var q = from lp in db.tPetMembers
                    where lp.fMemberID == mem.fMemberID
                    where lp.fStatus == lost
                    select lp.fPetID;

            foreach(var id in q)
            {
                exitLpID.Add(id);
            }


            return Json(exitLpID);

        }


        //檢查寵物是否登記走失
        public string isLostExit(int petID)
        {
            string isLpExit = db.tPetMembers.Where(p => p.fPetID == petID).FirstOrDefault().fStatus;

            return isLpExit;
       
        }


        //傳回走失詳細資料
        public JsonResult ReturnLostDetail(int petID)
        {
            var lostDetail = db.tLostPets.Where(p => p.fPetID == petID).FirstOrDefault();

            lostPetDetial lp = new lostPetDetial()
            {
                LostTime = lostDetail.fLostTime.ToShortDateString(),
                CompareLevel = lostDetail.fCompareLevel,
                LostPlace = lostDetail.fLostPlace,
                Remark = lostDetail.fRemark,
                Reward = lostDetail.fReward.ToString(),
            };

            return Json(lp);

        }



        //傳回寵物名
        public string ReturnPetName(int petID)
        {
            string petName = db.tPetMembers.Where(p => p.fPetID == petID).FirstOrDefault().fPetName;

            return petName;

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


    }
}

//走失寵物資料類別
class lostPetDetial
{
    public string LostTime { get; set; }
    public string CompareLevel { get; set; }
    public string LostPlace { get; set; }
    public string Remark { get; set; }
    public string Reward { get; set; }

}


class ApplierData
{
    public string ApplierName { get; set; }
    public string ApplierPhone { get; set; }
    public string ApplierIncome { get; set; }
    public string ApplierCity { get; set; }
    public string ApplierRegion { get; set; }
    public string ApplierPic { get; set; }
}

class ApplierPetData
{
    public string PetName { get; set; }
    public string PetPic { get; set; }
    public string PetBreed { get; set; }
}

