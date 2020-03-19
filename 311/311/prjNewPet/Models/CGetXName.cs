using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace prjNewPet.Models
{
    public class CGetXName
    {
        internal IEnumerable<tCity> getCity()
        {
            using (dbNewPetEntities db=new dbNewPetEntities())
            {
                var q = db.tCities.OrderBy(m => m.fCityID);
                return q.ToList(); ;
            }
        }

        internal IEnumerable<tRegion> getRegion(string cityid)
        {
            using (dbNewPetEntities db = new dbNewPetEntities())
            {
                var q = db.tRegions.Where(m => m.fCityID.ToString() == cityid);
                return q.ToList();
            }
        }

        internal IEnumerable<tPetClass> getPetClass()
        {
            using (dbNewPetEntities db = new dbNewPetEntities())
            {
                var q = db.tPetClasses;
                return q.ToList(); 
            }
        }

        internal IEnumerable<tBreed> getBreed(string petclassid)
        {
            using (dbNewPetEntities db = new dbNewPetEntities())
            {
                var q = db.tBreeds.Where(m => m.fPetClassID.ToString() == petclassid);
                return q.ToList();
            }
        }
    }
}