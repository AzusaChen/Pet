using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace prjNewPet.Models
{
    public static class DataService
    {
       

        public static List<DataPoint> GetProductAvgAxis()
        {
            dbNewPetEntities dbContext = new dbNewPetEntities();

            List<DataPoint> list = new List<DataPoint>();

            var model = from p in dbContext.tProducts.AsEnumerable()
                    where p.tCategory != null
                    group p by p.tCategory.fName into g
                    select new
                    {
                        CategoryName = g.Key,
                        Avg = g.Average(p => p.fUnitPrice),
                        //Min = g.Min(p => p.UnitPrice),
                        //Max = g.Max(p => p.UnitPrice),
                    };

            foreach (var item in model)
            {

             
                list.Add(new DataPoint(item.Avg,item.CategoryName));

            }

            

            return list;


        }



        public static List<DataPoint> GetMemberregisterAvgAxis()
        {
            dbNewPetEntities dbContext = new dbNewPetEntities();

            List<DataPoint> list = new List<DataPoint>();

            var model = from p in dbContext.tMembers.AsEnumerable()
                        group p by p.fRegisterDate.Value.Year into g
                        select new
                        {
                            年份 = g.Key,
                            會員人數 = g.Count()
                    
                        };

            foreach (var item in model)
            {


                list.Add(new DataPoint(item.年份, item.會員人數));

            }



            return list;


        }



        public static List<DataPoint> GetProductPriceAvgAxis()
        {
            dbNewPetEntities dbContext = new dbNewPetEntities();

            List<DataPoint> list = new List<DataPoint>();

            var model = from p in dbContext.tProducts.AsEnumerable()
                     
                        select new
                        {
                            產品名稱 = p.fName,
                            產品價錢 = p.fUnitPrice

                        };

            foreach (var item in model)
            {


                list.Add(new DataPoint(item.產品價錢, item.產品名稱));

            }



            return list;


        }



        //會員寵物類別分布圖
        public static List<DataPoint> GetChartPetClass()
        {
            dbNewPetEntities db = new dbNewPetEntities();

            List<DataPoint> list = new List<DataPoint>();

            var q = from ptc in db.tPetMembers.AsEnumerable()
                    group ptc by ptc.tPetClass.fName into g
                    select new
                    {
                        寵物類別 = g.Key,
                        數量 = g.Count(),

                    };

            foreach (var item in q)
            {

                list.Add(new DataPoint(item.數量, item.寵物類別));
            }

            return list;
        }


        //會員養狗品種分布圖
        public static List<DataPoint> GetChartDogBreed()
        {
            dbNewPetEntities db = new dbNewPetEntities();

            List<DataPoint> list = new List<DataPoint>();

            var q = from ptb in db.tPetMembers.AsEnumerable()
                    where ptb.fPetClassID == 1//狗
                    group ptb by ptb.tBreed.fName into g
                    select new
                    {
                        寵物品種 = g.Key,
                        數量 = g.Count(),

                    };

            foreach (var item in q)
            {
                list.Add(new DataPoint(item.數量, item.寵物品種));
            }

            return list;
        }
        //會員養貓品種分布圖
        public static List<DataPoint> GetChartCatBreed()
        {
            dbNewPetEntities db = new dbNewPetEntities();

            List<DataPoint> list = new List<DataPoint>();

            var q = from ptb in db.tPetMembers.AsEnumerable()
                    where ptb.fPetClassID == 2//貓
                    group ptb by ptb.tBreed.fName into g
                    select new
                    {
                        寵物品種 = g.Key,
                        數量 = g.Count(),

                    };

            foreach (var item in q)
            {
                list.Add(new DataPoint(item.數量, item.寵物品種));
            }

            return list;
        }

        //top5PetCoin會員
        public static List<DataPoint> GetChartTop5PetCoin()
        {
            dbNewPetEntities db = new dbNewPetEntities();

            List<DataPoint> list = new List<DataPoint>();

            var q = (from mem in db.tMembers.AsEnumerable()
                     orderby mem.fPetCoin descending
                     select new { 會員暱稱 = mem.fNickName, PetCoin數 = Convert.ToInt32(mem.fPetCoin) }).Take(5);


            foreach (var item in q)
            {
                list.Add(new DataPoint(item.PetCoin數, item.會員暱稱));
            }

            return list;
        }

        //top5Products
        public static List<DataPoint> GetChartTop5Products()
        {
            dbNewPetEntities db = new dbNewPetEntities();

            List<DataPoint> list = new List<DataPoint>();
            var count = (from x in db.tProducts.AsEnumerable()
                         select x.fProductID).Count();


            var q = (from mem in db.tOrder_Detail.AsEnumerable()
                    group mem by  mem.tProduct.fName into g 
                   
                    select new
                    {
                       產品名稱 = g.Key,
                       訂單數量 = g.Sum(p=>p.fQuantity),
                    }).OrderBy(p=>p.訂單數量);

            foreach (var item in q)
            {
                list.Add(new DataPoint(item.訂單數量, item.產品名稱.ToString()));
            }

            return list;
        }












    }
}