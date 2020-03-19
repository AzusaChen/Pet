using prjNewPet.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;
using Newtonsoft.Json;
using System.Threading.Tasks;
using PagedList;
using PagedList.Mvc;

namespace prjNewPet.Controllers
{
    public class CShoppingController : Controller
    {
        dbNewPetEntities db = new dbNewPetEntities();
        // GET: CShopping
        private int pageSize = 9;

        public ActionResult ShoppingIndex(int page=1)
        {
            int currentPage = page < 1 ? 1 : page; 

            var q = from p in db.tProducts
                    orderby p.fProductID
                    select p;
            var result = q.ToPagedList(currentPage,pageSize);

            return View(result);
        }

        public ActionResult ProductDetail(int pId)
        {
            var q = from p in db.tProducts
                    where p.fProductID == pId
                    select p;

            return View(q);
        }

        public ActionResult similarProduct(int cId)
        {
            var q = from p in db.tProducts
                    where p.fCategoryID==cId
                    select p;

            return PartialView(q.Take(4));
        }

        [HttpPost]
        public ActionResult Change(int pro,int cate)
        {
            var q = from p in db.tProducts
                    where p.fCategoryID == cate && p.fSupplierID == pro
                    orderby p.fProductID
                    select p;

            return PartialView(q);
        }

        [HttpPost]
        public ActionResult Change1(int cate)
        {
            var q = from p in db.tProducts
                    where p.fCategoryID == cate
                    orderby p.fProductID
                    select p;

            return PartialView(q);
        }

        [HttpPost]
        public JsonResult AddToCart(int fPId)
        {

            int no_login = 1;
            int user = 2;

            var fUserId = Session[CDictionary.SK_LOGINED_USER] as tMember;

            if (fUserId == null)
            {
                return this.Json(no_login);
                
            }

            var q = (from sc in db.tShoppingCarts
                     where sc.fProductID == fPId && sc.fMemberID == fUserId.fMemberID
                     select sc).FirstOrDefault();

            //若等於null，表示產品不在購物車清單內，將產品丟入購物車
            if (q == null)
            {
                var q1 = (from p in db.tProducts
                          where p.fProductID == fPId
                          select p).FirstOrDefault();

                tShoppingCart tsc = new tShoppingCart();
                tsc.fMemberID = fUserId.fMemberID;
                tsc.fPic = q1.fPic;
                tsc.fProductID = q1.fProductID;
                tsc.fQuantity = 1;
                tsc.fUnitPrice = q1.fUnitPrice;

                db.tShoppingCarts.Add(tsc);
            }
            else
            {
                q.fQuantity += 1; //若產品為購物車狀態，將產品數量+1
            }

            db.SaveChanges();

            return this.Json(user);
        }


        public ActionResult ShoppingCart()
        {
            var fUserId = Session[CDictionary.SK_LOGINED_USER] as tMember;

            if (fUserId == null)
            {
                return Content("<script>alert('請先登入會員！'); window.location.href='../Login/Auth';</script>");
            }

            var q = (from sc in db.tShoppingCarts
                     where sc.fMemberID == fUserId.fMemberID
                     select sc).ToList();

            return View(q);
        }

        
        public ActionResult Minicart()
        {
            var fUserId = Session[CDictionary.SK_LOGINED_USER] as tMember;

            if (fUserId == null)
            {
                return Content("");
            }

            var q = (from sc in db.tShoppingCarts
                     where sc.fMemberID == fUserId.fMemberID
                     select sc).ToList();

            return PartialView(q);
        }

        [HttpPost]
        public JsonResult DeleteCart(int fPId)
        {
            bool result = true;
            var fUserId = Session[CDictionary.SK_LOGINED_USER] as tMember;

            var q = (from sc in db.tShoppingCarts
                     where sc.fProductID == fPId && fUserId.fMemberID==sc.fMemberID
                     select sc).FirstOrDefault();

            db.tShoppingCarts.Remove(q);
            db.SaveChanges();

            return this.Json(result);
            //return RedirectToAction("ShoppingCart");
        }

        [HttpPost]
        public JsonResult UpdateCart(int fPId, int fQty, int fAmount)
        {
            bool result = true;
            var fUserId = Session[CDictionary.SK_LOGINED_USER] as tMember;

            var q = (from sc in db.tShoppingCarts
                     where sc.fProductID == fPId && fUserId.fMemberID == sc.fMemberID
                     select sc).FirstOrDefault();

            if (fQty + fAmount >= 1)
            {
                q.fQuantity = fQty + fAmount;
            }

            db.SaveChanges();

            return this.Json(result);
        }

        public ActionResult OrderInfo()
        {
            var fUserId = Session[CDictionary.SK_LOGINED_USER] as tMember;

            if (fUserId == null)
            {
                return Content("<script>alert('請先登入會員！'); window.location.href='../Login/Auth';</script>");
            }

            var q = (from sc in db.tShoppingCarts
                     where sc.fMemberID == fUserId.fMemberID
                     select sc).ToList();

            return View(q);
        }

       
        public ActionResult ReceiptInfo()
        {
            var fUserId = Session[CDictionary.SK_LOGINED_USER] as tMember;

            if (fUserId == null)
            {
                return Content("<script>alert('請先登入會員！'); window.location.href='../Login/Auth';</script>");
            }

            var q = (from o in db.tOrders
                     where o.fMemberID == fUserId.fMemberID
                     select o).FirstOrDefault();

            SelectList citylist = new SelectList(getxname.getCity(), "fCityID", "fName");
            ViewBag.CityName = citylist;

            return PartialView(q);
        }


        CGetXName getxname = new CGetXName();
        public JsonResult Regions(string cityID)
        {
            List<KeyValuePair<string, string>> items = new List<KeyValuePair<string, string>>();
            if (!string.IsNullOrEmpty(cityID))
            {
                var regions = this.getxname.getRegion(cityID);
                if (regions.Count() > 0)
                {
                    foreach (var r in regions)
                    {
                        items.Add(new KeyValuePair<string, string>(
                            r.fRegionID.ToString(), r.fName
                            ));
                    }
                }
            }
            return this.Json(items);
        }

        
        public ActionResult payment()
        {
            return PartialView();
        }

        [HttpPost]
        public JsonResult addOrder(COrderViewModel covm)
        {

            var fUserId = Session[CDictionary.SK_LOGINED_USER] as tMember;

            tOrder to = new tOrder();
            to.fMemberID = fUserId.fMemberID;
            to.fOrderDate = DateTime.Now;
            to.fReceiverName = covm.fRn;
            to.fReceiverPhone = covm.fRp;
            to.fCityID = covm.fcId;
            to.fRegionID = covm.frId;
            to.fAddressDetail = covm.fAd;
            to.fReceiverMail = covm.fRm;
            to.fPayWayID = covm.fPwId;
            to.fStatus = "未出貨";

            db.tOrders.Add(to);

            //-------------------------------------------------

            var q = (from od in db.tShoppingCarts
                     where od.fMemberID == fUserId.fMemberID
                     select od);

            //petcoin區
            const decimal Discount = 1M;//商品九折?
            const decimal PetCoinPercent = 0.3M;//
            decimal TotalPrice = 0;

            //將購物車商品資料移到訂單明細，並清空會員的購物車
            foreach (var x in q)
            {
                tOrder_Detail tOD = new tOrder_Detail();

                tOD.fOrderID = to.fOrderID;
                tOD.fProductID = x.fProductID;
                tOD.fName = x.tProduct.fName;
                tOD.fUnitPrice = x.fUnitPrice;
                tOD.fQuantity = x.fQuantity;
                tOD.fPic = x.fPic;

                //--訂單成立即減少商品庫存-------//

                var q1 = from p in db.tProducts
                         where p.fProductID==x.fProductID
                         select p;

                foreach (var items in q1)
                {
                    if (items.fUnitInStock <= 0)
                    {
                        items.fUnitInStock = 0;
                    }
                    else
                    {
                        items.fUnitInStock -= x.fQuantity;
                    }
                       
                }

                db.tOrder_Detail.Add(tOD);
                db.tShoppingCarts.Remove(x);

                //------------------------------//

                //petcoin
                TotalPrice += tOD.fUnitPrice * tOD.fQuantity * Discount;
            }
            //-------------------------------------------------
            
            //petcoin消費回饋
            var addmempetcoin = db.tMembers.Where(x => x.fMemberID == fUserId.fMemberID);
            foreach (var item in addmempetcoin)
            {
                if (covm.fPC == true)//判斷是否有使用PetCoin折抵，如果有就將會員的PetCoin/100取餘數。
                {
                    decimal d = 100;
                    item.fPetCoin = item.fPetCoin % d;
                    item.fPetCoin += TotalPrice * PetCoinPercent;
                    fUserId.fPetCoin = item.fPetCoin;//將最新PETCOIN數存回SESSION裡
                }
                else
                {
                    item.fPetCoin += TotalPrice * PetCoinPercent;
                    fUserId.fPetCoin = item.fPetCoin;//將最新PETCOIN數存回SESSION裡
                }

            }

            
            //-------------------------------------------------

            db.SaveChanges();

            CSendMail csm = new CSendMail();
            
            Task.Run(() => (csm.sendOrderDetail(covm.fRm, to.fOrderID)));

            bool data = true;

            return this.Json(data);
        }
    }
}