using prjNewPet.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace prjNewPet.Models
{
    public class CBulletinHome
    {
        public List<tLostPet> multiSearchForlost(int classID, string cityName, string regionName)
        {

            dbNewPetEntities db = new dbNewPetEntities();
            List<tLostPet> lp = null;

            if (classID == 0 && cityName == "" && regionName == "")
            {
                lp = db.tLostPets.ToList();
            }
            else if (classID == 0 && regionName == "")
            {
                var list = from l in db.tLostPets
                           where l.tPetMember.tCity.fName.Contains(cityName.ToString())
                           select l;
                lp = list.ToList();
            }
            else if (classID == 0 && cityName == "")
            {
                var list = from l in db.tLostPets
                           where l.tPetMember.tRegion.fName.Contains(regionName.ToString())
                           select l;
                lp = list.ToList();
            }
            else if (cityName == "" && regionName == "")
            {
                var list = from l in db.tLostPets
                           where l.tPetMember.fPetClassID == classID
                           select l;

                lp = list.ToList();
            }
            else if (classID == 0)
            {
                var list = from l in db.tLostPets
                           where l.tPetMember.tCity.fName.Contains(cityName.ToString())
                           where l.tPetMember.tRegion.fName.Contains(regionName.ToString())
                           select l;
                lp = list.ToList();
            }
            else if (cityName == "")
            {
                var list = from l in db.tLostPets
                           where l.tPetMember.fPetClassID == classID
                           where l.tPetMember.tRegion.fName.Contains(regionName.ToString())
                           select l;
                lp = list.ToList();
            }
            else if (regionName == "")
            {
                var list = from l in db.tLostPets
                           where l.tPetMember.fPetClassID == classID
                           where l.tPetMember.tCity.fName.Contains(cityName.ToString())
                           select l;
                lp = list.ToList();
            }
            else
            {
                var list = from l in db.tLostPets
                           where l.tPetMember.fPetClassID == classID
                           where l.tPetMember.tCity.fName.Contains(cityName.ToString())
                           where l.tPetMember.tRegion.fName.Contains(regionName.ToString())
                           select l;
                lp = list.ToList();
            }

            return lp;
        }


        public List<tFoundPet> multiSearchForFound(int classID, string cityName, string regionName)
        {

            dbNewPetEntities db = new dbNewPetEntities();
            List<tFoundPet> fp = null;

            if (classID == 0 && cityName == "" && regionName == "")
            {
                fp = db.tFoundPets.ToList();
            }
            else if (classID == 0 && regionName == "")
            {
                var list = from l in db.tFoundPets
                           where l.tCity.fName.Contains(cityName.ToString())
                           select l;
                fp = list.ToList();
            }
            else if (classID == 0 && cityName == "")
            {
                var list = from l in db.tFoundPets
                           where l.tRegion.fName.Contains(regionName.ToString())
                           select l;
                fp = list.ToList();
            }
            else if (cityName == "" && regionName == "")
            {
                var list = from l in db.tFoundPets
                           where l.fPetClassID == classID
                           select l;

                fp = list.ToList();
            }
            else if (classID == 0)
            {
                var list = from l in db.tFoundPets
                           where l.tCity.fName.Contains(cityName.ToString())
                           where l.tRegion.fName.Contains(regionName.ToString())
                           select l;
                fp = list.ToList();
            }
            else if (cityName == "")
            {
                var list = from l in db.tFoundPets
                           where l.fPetClassID == classID
                           where l.tRegion.fName.Contains(regionName.ToString())
                           select l;
                fp = list.ToList();
            }
            else if (regionName == "")
            {
                var list = from l in db.tFoundPets
                           where l.fPetClassID == classID
                           where l.tRegion.fName.Contains(cityName.ToString())
                           select l;
                fp = list.ToList();
            }
            else
            {
                var list = from l in db.tFoundPets
                           where l.fPetClassID == classID
                           where l.tCity.fName.Contains(cityName.ToString())
                           where l.tRegion.fName.Contains(regionName.ToString())
                           select l;
                fp = list.ToList();
            }

            return fp;
        }
    }
}