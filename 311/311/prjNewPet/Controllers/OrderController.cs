using prjNewPet.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace prjNewPet.Controllers
{
    public class OrderController : Controller
    {
        // GET: Order
        dbNewPetEntities db = new dbNewPetEntities();

        public ActionResult OrderList()
        {
            var orders = from o in db.tOrders
                         select o;
    
            var orderdetail = from od in db.tOrder_Detail
                              join o in db.tOrders
                              on od.fOrderID equals o.fOrderID
                              select od;

            ViewBag.orderdetail = orderdetail;

            return View(orders);
           
        }

        [HttpPost]
        public ActionResult OrderList(string status)
        {
            List<tOrder> list = null;

            if (status == "已出貨" || status == "未出貨")
            {

                var q = from o in db.tOrders
                        where o.fStatus == status
                        select o;

                list = q.ToList();

            }
            else
            {
                list = db.tOrders.ToList();
            }


            var orders = from o in db.tOrders
                         select o;

            var orderdetail = from od in db.tOrder_Detail
                              join o in db.tOrders
                              on od.fOrderID equals o.fOrderID
                              select od;

            ViewBag.orderdetail = orderdetail;

            return View("OrderList", "_Layout_Backmanagement", list);

        }


        public ActionResult MemOrderList(int memId)
        {
            var orders = from o in db.tOrders
                         where o.fMemberID == memId                         
                         select o;

            return View("MemOrderList", "_layout_Membercenter", orders);

        }

        public ActionResult Details(int id)
        {
            var orderdetail = from od in db.tOrder_Detail
                         where od.fOrderID == id
                         select od;

            decimal totalPrice = 0;
            foreach(var n in orderdetail)
            {
                totalPrice += n.fQuantity * n.fUnitPrice;
            }
            ViewBag.totalPrice = totalPrice;

            return View("Details", "_layout_Membercenter", orderdetail);

        }

        [HttpPost]
        public ActionResult BackDetails(int id)
        {
            var orderdetail = from od in db.tOrder_Detail
                              where od.fOrderID == id
                              select od;

            decimal totalPrice = 0;
            foreach (var n in orderdetail)
            {
                totalPrice += n.fQuantity * n.fUnitPrice;
            }
            ViewBag.totalPrice = totalPrice;

            return PartialView(orderdetail);

        }

        private IEnumerable<tCity> GetCity()
        {
            using (dbNewPetEntities db = new dbNewPetEntities())//釋放記憶體
            {
                var query = db.tCities.OrderBy(m => m.fCityID);

                return query.ToList();
            }
        }

        //後端產生 Orders JSON 資料的程式如下, return this.Json(items);後回到ajax

        [HttpPost]
        public JsonResult Regions(string cityID)
        {
            List<KeyValuePair<string, string>> items = new List<KeyValuePair<string, string>>();

            if (!string.IsNullOrWhiteSpace(cityID))
            {
                var regions = this.GetOrders(cityID);
                if (regions.Count() > 0)
                {
                    foreach (var region in regions)
                    {
                        items.Add(new KeyValuePair<string, string>(
                            region.fRegionID.ToString(),
                            region.fName
                            ));
                    }
                }
            }
            return this.Json(items);
        }

        private IEnumerable<tRegion> GetOrders(string cityID)
        {
            using (dbNewPetEntities db = new dbNewPetEntities())
            {
                var query = db.tRegions.Where(x => x.fCityID.ToString() == cityID);
                return query.ToList();
            }
        }



        private IEnumerable<tMember> GetMember()
        {
            using (dbNewPetEntities db = new dbNewPetEntities())//釋放記憶體
            {
                var query = db.tMembers.OrderBy(m => m.fMemberID);

                return query.ToList();
            }
        }

        private IEnumerable<tShipVia> GetShipVia()
        {
            using (dbNewPetEntities db = new dbNewPetEntities())//釋放記憶體
            {
                var query = db.tShipVias.OrderBy(m => m.fShipViaID);

                return query.ToList();
            }
        }

        private IEnumerable<tPayWay> GetPayWay()
        {
            using (dbNewPetEntities db = new dbNewPetEntities())//釋放記憶體
            {
                var query = db.tPayWays.OrderBy(m => m.fPayWayID);

                return query.ToList();
            }
        }



        public ActionResult Edit(int fOrderID)
        {

            SelectList selectListmem = new SelectList(this.GetMember(), "fMemberID", "fName");
            ViewBag.SelectListmem = selectListmem;

            SelectList selectListcity = new SelectList(this.GetCity(), "fCityID", "fName");
            ViewBag.SelectListcity = selectListcity;

           

            SelectList selectListrShipVia = new SelectList(this.GetShipVia(), "fShipViaID", "fName");
            ViewBag.selectListrShipVia = selectListrShipVia;

            SelectList selectListrPayWay = new SelectList(this.GetPayWay(), "fPayWayID", "fName");
            ViewBag.selectListrPayWay = selectListrPayWay;



            var orders = (from o in db.tOrders
                            where o.fOrderID == fOrderID
                          select o).FirstOrDefault();

            if (orders == null)
            {
                return RedirectToAction("OrderList");
            }
            else
            {
                return View("Edit","_Layout_Backmanagement", orders);
            }

        }

        [HttpPost]
        public ActionResult Edit(tOrder order)
        {
            var orders = (from o in db.tOrders
                          where o.fOrderID == order.fOrderID
                          select o).FirstOrDefault();


            if (order.tCity == null)
            {
                orders.fCityID = orders.fCityID;
            }
            else if (order.tMember == null)
            {
                orders.tMember.fMemberID = orders.tMember.fMemberID;
            }
            else if (order.tPayWay == null)
            {
                orders.tPayWay.fPayWayID = orders.tPayWay.fPayWayID;
            }

            

            orders.fShipOutDate = order.fShipOutDate;
         
            orders.fCityID = order.fCityID;

            if (order.fRegionID == 0)
            {
                orders.fRegionID = orders.fRegionID;
            }
            else
            {
                orders.fRegionID = order.fRegionID;
            }

            orders.fAddressDetail = order.fAddressDetail;
            orders.fPayWayID = order.fPayWayID;
            orders.fStatus = order.fStatus;


            db.SaveChanges();

            return RedirectToAction("OrderList");

        }

        public JsonResult Delete(int fOrderID)
        {
            var orders = (from o in db.tOrders
                          where o.fOrderID == fOrderID
                          select o).FirstOrDefault();

            if (orders.fStatus == "已出貨")
            {
                return Json("已出貨");
            }


            var ordersdetail = from od in db.tOrder_Detail
                                where od.fOrderID == fOrderID
                                select od;

            foreach (var q in ordersdetail) {
                db.tOrder_Detail.Remove(q);
            }

           
            db.tOrders.Remove(orders);

            db.SaveChanges();

           
            return Json("sucess");
        }

        public JsonResult ChangeStatus(int OrderId)
        {
            var q = (from o in db.tOrders
                    where o.fOrderID == OrderId
                    select o).FirstOrDefault();

            if (q.fStatus == "未出貨") {
                q.fStatus = "已出貨";
                q.fShipOutDate = DateTime.Now;

            }
            else if(q.fStatus == "已出貨")
            {
                q.fStatus = "未出貨";
                q.fShipOutDate = null;
            }
           

            db.SaveChanges();

            return this.Json(true);
        }

        


    }
}