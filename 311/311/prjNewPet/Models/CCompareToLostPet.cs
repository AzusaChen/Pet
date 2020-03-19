using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace prjNewPet.Models
{
    public class CCompareToLostPet
    {
        private dbNewPetEntities db = new dbNewPetEntities();

        public List<string> CompareToLost(int PetClassID)
        {
            List<string> lostPetMemEmail;

            var lp = from l in db.tLostPets
                     where l.tPetMember.fPetClassID == PetClassID
                     select l.tPetMember.tMember.fEMail;
            lostPetMemEmail = lp.ToList();

            return lostPetMemEmail;
        }

        public async Task CLSorting(tFoundPet fp)
        {
            if (fp.fChipID != null)
            {
                PCompare(fp);

            }
            else
            {
                IEnumerable<string> CLV = from c in db.tLostPets
                                          where c.tPetMember.fPetClassID == fp.fPetClassID
                                          where c.tPetMember.fBreedID == fp.fBreedID
                                          select c.fCompareLevel;

                foreach (var cl in CLV)
                {
                    switch (cl)
                    {
                        case "部分相關":
                            LCompare(fp);
                            break;
                        case "高度相關":
                            HCompare(fp);
                            break;
                    }
                }
            }

        }

        //部分相關的比對
        public List<string> LCompare(tFoundPet fp)
        {
            List<string> lostPetMemEmail;

            var lp = from l in db.tLostPets
                     where l.tPetMember.fPetClassID == fp.fPetClassID
                     where l.tPetMember.fBreedID == fp.fBreedID
                     where l.tPetMember.fSkin.Contains(fp.fSkin)
                     where l.fCompareLevel == "部分相關"
                     select l.tPetMember.tMember.fEMail;

            fp.fRemark += "L";

            lostPetMemEmail = lp.ToList();

            CSendMail sm = new CSendMail();

            if (lostPetMemEmail.Count > 0)
            {
                Task.Run(() => (sm.sendGmail(lostPetMemEmail, fp)));
            }

            return lostPetMemEmail;
        }

        //高度相關的比對
        public List<string> HCompare(tFoundPet fp)
        {
            List<string> lostPetMemEmail;

            var lp = from l in db.tLostPets
                     where l.tPetMember.fPetClassID == fp.fPetClassID
                     where l.tPetMember.fBreedID == fp.fBreedID
                     where l.tPetMember.fSkin.Contains(fp.fSkin)
                     where l.tPetMember.fCollar == fp.fCollar
                     where l.tPetMember.fCityID == fp.fCityID
                     where l.tPetMember.fRegionID == fp.fRegionID
                     where l.tPetMember.fGender == fp.fGender
                     where l.fCompareLevel == "高度相關"
                     select l.tPetMember.tMember.fEMail;

            fp.fRemark += "H";

            lostPetMemEmail = lp.ToList();

            CSendMail sm = new CSendMail();

            if (lostPetMemEmail.Count > 0)
            {
                Task.Run(() => (sm.sendGmail(lostPetMemEmail, fp)));
            }

            return lostPetMemEmail;
        }


        //完全相關的比對
        public List<string> PCompare(tFoundPet fp)
        {
            List<string> lostPetMemEmail;

            var lp = from l in db.tLostPets
                     where l.tPetMember.fChipID == fp.fChipID
                     select l.tPetMember.tMember.fEMail;

            fp.fRemark = "Perfect" + fp.fRemark;

            lostPetMemEmail = lp.ToList();

            CSendMail sm = new CSendMail();

            if (lostPetMemEmail.Count > 0)
            {
                Task.Run(() => (sm.sendGmail(lostPetMemEmail, fp)));
            }

            return lostPetMemEmail;
        }
    }
}