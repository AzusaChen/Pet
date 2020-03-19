using Newtonsoft.Json;
using prjNewPet.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace prjNewPet.Controllers
{
    public class MainChartController : Controller
    {
        // GET: MainChart
        JsonSerializerSettings _jsonSetting = new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore };


        public ActionResult AllChart()
        {
            dbNewPetEntities db = new dbNewPetEntities();



            ViewBag.DataPoints = JsonConvert.SerializeObject(DataService.GetProductAvgAxis(), _jsonSetting);

            ViewBag.DataPointsMemberre = JsonConvert.SerializeObject(DataService.GetMemberregisterAvgAxis(), _jsonSetting);

            ViewBag.DataPointsProductprice = JsonConvert.SerializeObject(DataService.GetProductPriceAvgAxis(), _jsonSetting);

            ViewBag.PETCLASS = JsonConvert.SerializeObject(DataService.GetChartPetClass(), _jsonSetting);

            ViewBag.DOGBREED = JsonConvert.SerializeObject(DataService.GetChartDogBreed(), _jsonSetting);

            ViewBag.CATBREED = JsonConvert.SerializeObject(DataService.GetChartCatBreed(), _jsonSetting);

            ViewBag.TOP5PTCOIN = JsonConvert.SerializeObject(DataService.GetChartTop5PetCoin(), _jsonSetting);

            ViewBag.TOP5PRODUCTS = JsonConvert.SerializeObject(DataService.GetChartTop5Products(), _jsonSetting);


            return View("AllChart", "_Layout_Backmanagement");
        }
    }
}