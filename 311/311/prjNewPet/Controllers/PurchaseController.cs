using prjNewPet.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace prjNewPet.Controllers
{
    public class PurchaseController : Controller
    {
        // GET: Purchase
        dbNewPetEntities db = new dbNewPetEntities();
        public ActionResult List()
        {
            return View("List", "_Layout_Backmanagement");
        }



        [HttpPost]
        public JsonResult GetProductlist()
        {
            List<TPro> items = new List<TPro>();


            var model = this.GetProduct();
            if (model.Count() > 0)
            {
                foreach (var item in model)
                {
                    TPro pro = new TPro
                    {
                        fProductID = item.fProductID,

                        fName = item.fName,
                        fPic = item.fPic,
                        fCategoryName = item.tCategory.fName,
                        fUnitPrice = Math.Truncate(Convert.ToDouble(item.fUnitPrice)*0.8),
                        fUnitInStock = item.fUnitInStock,
                        fSupplierName = item.tSupplier.fName,
                        

                    };



                    items.Add(pro);


                }

            }


           

            return this.Json(items);
        }

        private IEnumerable<tProduct> GetProduct()
        {
           

            List<tProduct> productlist = null;

            var query = from f in db.tProducts
                        select f;

            productlist = query.ToList();

            return productlist;
        }


        public ActionResult _Purchasecart()
        {
            

            var q = (from sc in db.tPurchaseShoppingCarts                 
                     select sc).ToList();

            return PartialView(q);
        }

        [HttpPost]
        public JsonResult AddToCart(int fPId)
        {



            var q = (from sc in db.tPurchaseShoppingCarts
                     where sc.fProductID == fPId 
                     select sc).FirstOrDefault();

            //若等於null，表示產品不在購物車清單內，將產品丟入購物車
            if (q == null)
            {
                var q1 = (from p in db.tProducts
                          where p.fProductID == fPId
                          select p).FirstOrDefault();


                tPurchaseShoppingCart tpsc = new tPurchaseShoppingCart();
                tpsc.fProductID = q1.fProductID;
                tpsc.fSupplierID = q1.fSupplierID;
                tpsc.fUnitPrice = Math.Truncate( Convert.ToDecimal(Convert.ToDouble(q1.fUnitPrice) * 0.8));
                tpsc.fQuantity = 1;
                tpsc.fPic = q1.fPic;
                                            

                db.tPurchaseShoppingCarts.Add(tpsc);
            }
            else
            {
                q.fQuantity += 1; //若產品為購物車狀態，將產品數量+1
            }

            db.SaveChanges();

            return this.Json("done");
        }

        [HttpPost]
        public JsonResult DeleteCart(int fPId)
        {
            

            var q = (from sc in db.tPurchaseShoppingCarts
                     where sc.fProductID == fPId 
                     select sc).FirstOrDefault();

            if(q!=null)
            {
                db.tPurchaseShoppingCarts.Remove(q);
                db.SaveChanges();

            }

          

            return this.Json("done");
           
        }


        [HttpPost]
        public JsonResult UpdateCart(int fPId)
        {


            var q = (from sc in db.tPurchaseShoppingCarts
                     where sc.fProductID == fPId
                     select sc).FirstOrDefault();
            if(q!=null)
            {
                if (q.fQuantity > 1)
                {
                    q.fQuantity -= 1;
                }
                else
                {
                    q.fQuantity = 1;
                }

            }

             
            db.SaveChanges();

            return this.Json("done");

        }

        [HttpPost]
        public JsonResult addPurchase()
        {

            

            tPurchase tpu = new tPurchase();
            tpu.fPurchaseDate = DateTime.Now;

            db.tPurchases.Add(tpu);

            //-------------------------------------------------

            var q = from od in db.tPurchaseShoppingCarts                    
                     select od;

            foreach (var x in q)
            {
                tPurchase_Detials tOD = new tPurchase_Detials();

                tOD.fPurchaseID = tpu.fPurchaseID;
                tOD.fProductID = x.fProductID;
                tOD.fName = x.tProduct.fName;
                tOD.fUnitPrice = x.fUnitPrice;
                tOD.fQuantity = x.fQuantity;
                tOD.fPic = x.fPic;
                tOD.fSupplierID = x.fSupplierID;

                db.tPurchase_Detials.Add(tOD);
                db.tPurchaseShoppingCarts.Remove(x);


                var productstock = from st in db.tProducts
                                   where st.fProductID == x.fProductID
                                   select st;

                foreach (var item in productstock)
                {

                    item.fUnitInStock += x.fQuantity;

                }


            }
            //-------------------------------------------------

            db.SaveChanges();

            CSendMail csm = new CSendMail();

            Task.Run(() => (csm.sendPurchaseDetail("leoncheng19@gmail.com", tpu.fPurchaseID)));



            return this.Json("done");
        }


    }
}