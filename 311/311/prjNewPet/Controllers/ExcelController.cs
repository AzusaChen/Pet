using OfficeOpenXml;
using OfficeOpenXml.Drawing.Chart;
using OfficeOpenXml.Style;

using prjNewPet.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace prjNewPet.Controllers
{
    public class ExcelController : Controller
    {
        dbNewPetEntities db = new dbNewPetEntities();
        //會員資料
        private DataTable GetMemberDataTable()
        {
            
            var q = from mb in db.tMembers
                    select new {
                        姓名 = mb.fName ,
                        性別 =mb.fGender,
                        身分證字號 =mb.fIDCardNumber,
                        帳號=mb.fAccount,                        
                        出生日期 =mb.fBirthDate,
                        經濟狀況=mb.fEnconomicStatus,
                        電話=mb.fPhone,
                        電子郵件=mb.fEMail,
                        居住城市=mb.tCity.fName,
                        地區=mb.tRegion.fName,
                        詳細地址=mb.fAddressDetail
                    };
                    
            DataTable dt = IEnumerableExtensions.ToDataTable(q);
           
           

            return dt;
        }
        //產品售價分布圖
        private DataTable GetProductPriceDataTable()
        {

            var model = from p in db.tProducts.AsEnumerable()
                        where p.tCategory != null
                        group p by p.tCategory.fName into g
                        select new
                        {
                            產品分類 = g.Key,
                            平均價格 = g.Average(p => p.fUnitPrice),
                            //Min = g.Min(p => p.UnitPrice),
                            //Max = g.Max(p => p.UnitPrice),
                        };

            DataTable dt = IEnumerableExtensions.ToDataTable(model);



            return dt;
        }
        //會員人數成長
        private DataTable GetMemberGrowthDataTable()
        {

            var model = from p in db.tMembers.AsEnumerable()
                        orderby p.fRegisterDate ascending
                        group p by p.fRegisterDate.Value.Year into g
                        select new
                        {
                            年份 = g.Key,
                            會員人數 = g.Count()

                        };

            DataTable dt = IEnumerableExtensions.ToDataTable(model);



            return dt;
        }
        //產品價錢分布
        private DataTable GetALLProductPriceDataTable()
        {

            var model = from p in db.tProducts.AsEnumerable()

                        select new
                        {
                            產品名稱 = p.fName,
                            產品價錢 = p.fUnitPrice,
                            產品待出貨 =p.fUnitOnOrder,
                            產品庫存 = p.fUnitInStock
                            

                        };

            DataTable dt = IEnumerableExtensions.ToDataTable(model);



            return dt;
        }
        //會員寵物類別分布圖
        private DataTable GetPetClassDataTable()
        {

            var q = from ptc in db.tPetMembers.AsEnumerable()
                    group ptc by ptc.tPetClass.fName into g
                    select new
                    {
                        寵物類別 = g.Key,
                        數量 = g.Count(),

                    };

            DataTable dt = IEnumerableExtensions.ToDataTable(q);



            return dt;
        }
        //會員養狗品種分布圖
        private DataTable GetDogBreedDataTable()
        {

            var q = from ptb in db.tPetMembers.AsEnumerable()
                    where ptb.fPetClassID == 1//狗
                    group ptb by ptb.tBreed.fName into g
                    select new
                    {
                        寵物品種 = g.Key,
                        數量 = g.Count(),

                    };

            DataTable dt = IEnumerableExtensions.ToDataTable(q);



            return dt;
        }
        //會員養貓品種比例圖
        private DataTable GetCatBreedDataTable()
        {

            var q = from ptb in db.tPetMembers.AsEnumerable()
                    where ptb.fPetClassID == 2//貓
                    group ptb by ptb.tBreed.fName into g
                    select new
                    {
                        寵物品種 = g.Key,
                        數量 = g.Count(),

                    };

            DataTable dt = IEnumerableExtensions.ToDataTable(q);



            return dt;
        }
        //PetCoin會員
        private DataTable GetPetCoinDataTable()
        {
            var q = from mem in db.tMembers.AsEnumerable()
                    orderby mem.fPetCoin descending
                    select new
                    {
                        會員暱稱 = mem.fNickName,
                        PetCoin數 = Convert.ToInt32(mem.fPetCoin)
                    };

            DataTable dt = IEnumerableExtensions.ToDataTable(q);



            return dt;
        }
        //商品數量排行榜
        private DataTable GetTopProductDataTable()
        {
            var q = (from mem in db.tOrder_Detail.AsEnumerable()
                     group mem by mem.tProduct.fName into g
                     select new
                     {
                         產品名稱 = g.Key,
                         訂單數量 = g.Sum(p => p.fQuantity),
                     }).OrderByDescending(p => p.訂單數量);

            DataTable dt = IEnumerableExtensions.ToDataTable(q);



            return dt;
        }

        // GET: Excel

        public ActionResult ExportExcel(DataTable datatable)
        {
            
            using (ExcelPackage package =new ExcelPackage())
            {


                //新增worksheet
                ExcelWorksheet ws = package.Workbook.Worksheets.Add("會員資料表");
                //取得資料
                DataTable dt = GetMemberDataTable();
                int length = datatable.Columns.Count;//抓到的length
                //將datatable資料塞進sheet中
                ws.Cells["A1"].LoadFromDataTable(dt, true);
                
                //自動調整大小
                ws.Cells[ws.Dimension.Address].AutoFitColumns();
                //設定EXCEL樣式
                using (ExcelRange rng = ws.Cells["A1:L1"])
                {
                    rng.Style.Font.Bold = true;
                    rng.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    rng.Style.Fill.BackgroundColor.SetColor(Color.Gray);
                    rng.Style.Font.Color.SetColor(Color.White);
                    rng.Style.HorizontalAlignment = ExcelHorizontalAlignment.General;
                }
                //============================================================================================
                ExcelWorksheet wsProductPrice = package.Workbook.Worksheets.Add("產品種類平均銷售價格");

                DataTable dtpp = GetProductPriceDataTable();
                wsProductPrice.Cells["A1"].LoadFromDataTable(dtpp, true);

                //自動調整大小
                wsProductPrice.Cells[wsProductPrice.Dimension.Address].AutoFitColumns();
                //設定EXCEL樣式
                using (ExcelRange rng = wsProductPrice.Cells["A1:B1"])
                {
                    rng.Style.Font.Bold = true;
                    rng.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    rng.Style.Fill.BackgroundColor.SetColor(Color.Gray);
                    rng.Style.Font.Color.SetColor(Color.White);
                    rng.Style.HorizontalAlignment = ExcelHorizontalAlignment.General;
                }
                //===========================================================================
                ExcelWorksheet wsMem = package.Workbook.Worksheets.Add("會員成長表");
                //取得資料
                DataTable dtMem = GetMemberGrowthDataTable();

                //將datatable資料塞進sheet中
                wsMem.Cells["A1"].LoadFromDataTable(dtMem, true);

                //自動調整大小
                wsMem.Cells[wsMem.Dimension.Address].AutoFitColumns();
                //設定EXCEL樣式
                using (ExcelRange rng = wsMem.Cells["A1:B1"])
                {
                    rng.Style.Font.Bold = true;
                    rng.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    rng.Style.Fill.BackgroundColor.SetColor(Color.Gray);
                    rng.Style.Font.Color.SetColor(Color.White);
                    rng.Style.HorizontalAlignment = ExcelHorizontalAlignment.General;
                }
                //========================================================================
                ExcelWorksheet wsPD = package.Workbook.Worksheets.Add("產品相關資料");
                //取得資料
                DataTable dtpd = GetALLProductPriceDataTable();

                //將datatable資料塞進sheet中
                wsPD.Cells["A1"].LoadFromDataTable(dtpd, true);

                //自動調整大小
                wsPD.Cells[wsPD.Dimension.Address].AutoFitColumns();
                //設定EXCEL樣式
                using (ExcelRange rng = wsPD.Cells["A1:D1"])
                {
                    rng.Style.Font.Bold = true;
                    rng.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    rng.Style.Fill.BackgroundColor.SetColor(Color.Gray);
                    rng.Style.Font.Color.SetColor(Color.White);
                    rng.Style.HorizontalAlignment = ExcelHorizontalAlignment.General;
                }
                //==============================================================================
                ExcelWorksheet wspetcoin = package.Workbook.Worksheets.Add("會員petcoin統計表");
                //取得資料
                DataTable dtptcoin = GetPetCoinDataTable();

                //將datatable資料塞進sheet中
                wspetcoin.Cells["A1"].LoadFromDataTable(dtptcoin, true);

                //自動調整大小
                wspetcoin.Cells[wspetcoin.Dimension.Address].AutoFitColumns();
                //設定EXCEL樣式
                using (ExcelRange rng = wspetcoin.Cells["A1:B1"])
                {
                    rng.Style.Font.Bold = true;
                    rng.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    rng.Style.Fill.BackgroundColor.SetColor(Color.Gray);
                    rng.Style.Font.Color.SetColor(Color.White);
                    rng.Style.HorizontalAlignment = ExcelHorizontalAlignment.General;
                }
                //=================================================================================
                ExcelWorksheet wstopproduct = package.Workbook.Worksheets.Add("商品銷售排行榜");
                //取得資料
                DataTable dttopproduct = GetTopProductDataTable();

                //將datatable資料塞進sheet中
                wstopproduct.Cells["A1"].LoadFromDataTable(dttopproduct, true);

                //自動調整大小
                wstopproduct.Cells[wstopproduct.Dimension.Address].AutoFitColumns();
                //設定EXCEL樣式
                using (ExcelRange rng = wstopproduct.Cells["A1:B1"])
                {
                    rng.Style.Font.Bold = true;
                    rng.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    rng.Style.Fill.BackgroundColor.SetColor(Color.Gray);
                    rng.Style.Font.Color.SetColor(Color.White);
                    rng.Style.HorizontalAlignment = ExcelHorizontalAlignment.General;
                }


                //========================================
                var stream = new MemoryStream();
                //存檔
                package.SaveAs(stream);
                

                string fileName = DateTime.Now.ToShortDateString() + ".xlsx";

                
                
                string contentType = "application / vnd.openxmlformats - officedocument.spreadsheetml.sheet";

                stream.Position = 0;
                
              
                return File(stream, contentType, fileName);
            }
            
           
           
        }
        public ActionResult ExportChart()
        {
          
            
            using (ExcelPackage excelPackage = new ExcelPackage())
            {
                int Man = db.tMembers.Where(x => x.fGender == "男").Count();
                int Woman = db.tMembers.Where(x => x.fGender == "女").Count();


                ExcelWorksheet wsPClass = excelPackage.Workbook.Worksheets.Add("會員寵物類別比例圖");

                DataTable dtpc = GetPetClassDataTable();
                wsPClass.Cells["A1"].LoadFromDataTable(dtpc, true);

                //自動調整大小
                wsPClass.Cells[wsPClass.Dimension.Address].AutoFitColumns();
                //設定EXCEL樣式
                using (ExcelRange rng = wsPClass.Cells["A1:B1"])
                {
                    rng.Style.Font.Bold = true;
                    rng.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    rng.Style.Fill.BackgroundColor.SetColor(Color.Gray);
                    rng.Style.Font.Color.SetColor(Color.White);
                    rng.Style.HorizontalAlignment = ExcelHorizontalAlignment.General;
                }
                //新增圓餅圖
                ExcelPieChart ChartPetClass = wsPClass.Drawings.AddChart("會員寵物類別比例圖", eChartType.Pie3D) as ExcelPieChart;
                ChartPetClass.Title.Text = "會員寵物類別比例圖";//圖表名字
                //圓餅圖資料範圍
                ChartPetClass.Series.Add(ExcelRange.GetAddress(2, 2, 3, 2), ExcelRange.GetAddress(2, 1, 3, 1));
                //圖例
                ChartPetClass.Legend.Position = eLegendPosition.Bottom;
                //是否秀%數
                ChartPetClass.DataLabel.ShowPercent = true;

                ChartPetClass.SetSize(300, 400);
                ChartPetClass.SetPosition(3, 0, 2, 0);
                //==============================================================================
                ExcelWorksheet wsDogBreed = excelPackage.Workbook.Worksheets.Add("會員養狗品種分布圖");

                DataTable dtdb = GetDogBreedDataTable();
                wsDogBreed.Cells["A1"].LoadFromDataTable(dtdb, true);

                //自動調整大小
                wsDogBreed.Cells[wsDogBreed.Dimension.Address].AutoFitColumns();
                //設定EXCEL樣式
                using (ExcelRange rng = wsDogBreed.Cells["A1:B1"])
                {
                    rng.Style.Font.Bold = true;
                    rng.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    rng.Style.Fill.BackgroundColor.SetColor(Color.Gray);
                    rng.Style.Font.Color.SetColor(Color.White);
                    rng.Style.HorizontalAlignment = ExcelHorizontalAlignment.General;
                }
                //新增圓餅圖
                ExcelPieChart ChartDogBreed = wsDogBreed.Drawings.AddChart("會員養狗品種分布圖", eChartType.Pie3D) as ExcelPieChart;
                ChartDogBreed.Title.Text = "會員養狗品種分布圖";//圖表名字
                //圓餅圖資料範圍
                ChartDogBreed.Series.Add(ExcelRange.GetAddress(2, 2, 17, 2), ExcelRange.GetAddress(2, 1, 17, 1));
                //圖例
                ChartDogBreed.Legend.Position = eLegendPosition.Bottom;
                //是否秀%數
                ChartDogBreed.DataLabel.ShowPercent = true;

                ChartDogBreed.SetSize(300, 400);
                ChartDogBreed.SetPosition(3, 0, 2, 0);
                //=========================================================================
                ExcelWorksheet wsCatBreed = excelPackage.Workbook.Worksheets.Add("會員養貓品種分布圖");

                DataTable dtcb = GetCatBreedDataTable();
                wsCatBreed.Cells["A1"].LoadFromDataTable(dtcb, true);

                //自動調整大小
                wsCatBreed.Cells[wsCatBreed.Dimension.Address].AutoFitColumns();
                //設定EXCEL樣式
                using (ExcelRange rng = wsCatBreed.Cells["A1:B1"])
                {
                    rng.Style.Font.Bold = true;
                    rng.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    rng.Style.Fill.BackgroundColor.SetColor(Color.Gray);
                    rng.Style.Font.Color.SetColor(Color.White);
                    rng.Style.HorizontalAlignment = ExcelHorizontalAlignment.General;
                }
                //新增圓餅圖
                ExcelPieChart ChartCatBreed = wsCatBreed.Drawings.AddChart("會員養貓品種分布圖", eChartType.Pie3D) as ExcelPieChart;
                ChartCatBreed.Title.Text = "會員養貓品種分布圖";//圖表名字
                //圓餅圖資料範圍
                ChartCatBreed.Series.Add(ExcelRange.GetAddress(2, 2, 11, 2), ExcelRange.GetAddress(2, 1, 11, 1));
                //圖例
                ChartCatBreed.Legend.Position = eLegendPosition.Bottom;
                //是否秀%數
                ChartCatBreed.DataLabel.ShowPercent = true;

                ChartCatBreed.SetSize(300, 400);
                ChartCatBreed.SetPosition(3, 0, 2, 0);

                //==============================================================================



                //新增工作表
                ExcelWorksheet wsGender =excelPackage.Workbook.Worksheets.Add("男女分布圖");
                //加入資料
                wsGender.Cells[1, 1].Value = "男";
                wsGender.Cells[1, 2].Value = "女";
                wsGender.Cells[2, 1].Value = Man;
                wsGender.Cells[2, 2].Value = Woman;
                //新增圓餅圖
                ExcelPieChart ChartGender = wsGender.Drawings.AddChart("會員男女分布圖", eChartType.Pie3D) as ExcelPieChart;
                ChartGender.Title.Text = "會員男女分布圖";//圖表名字
                //圓餅圖資料範圍
                ChartGender.Series.Add(ExcelRange.GetAddress(2, 1, 2, 2), ExcelRange.GetAddress(1, 1, 1, 2));
                //圖例
                ChartGender.Legend.Position = eLegendPosition.Bottom;
                //是否秀%數
                ChartGender.DataLabel.ShowPercent = true;

                ChartGender.SetSize(300, 400);
                ChartGender.SetPosition(3, 0, 2, 0);
                //==============================================================================
                
                ExcelWorksheet wsCity = excelPackage.Workbook.Worksheets.Add("會員直轄市分布圖");

                wsCity.Cells[1, 1].Value = "台北市";
                wsCity.Cells[1, 2].Value = "新北市";
                wsCity.Cells[1, 3].Value = "桃園市";
                wsCity.Cells[1, 4].Value = "台中市";
                wsCity.Cells[1, 5].Value = "台南市";
                wsCity.Cells[1, 6].Value = "高雄市";

                int TPEMEM = db.tMembers.Where(x => x.fCityID ==3).Count();
                int NTMEM = db.tMembers.Where(x => x.fCityID ==4).Count();
                int TYMEM = db.tMembers.Where(x => x.fCityID ==6).Count();
                int TCMEM = db.tMembers.Where(x => x.fCityID ==11).Count();
                int TNMEM = db.tMembers.Where(x => x.fCityID ==15).Count();
                int KSHMEM = db.tMembers.Where(x => x.fCityID ==16).Count();

                wsCity.Cells[2, 1].Value = TPEMEM;
                wsCity.Cells[2, 2].Value = NTMEM;
                wsCity.Cells[2, 3].Value = TYMEM;
                wsCity.Cells[2, 4].Value = TCMEM;
                wsCity.Cells[2, 5].Value = TNMEM;
                wsCity.Cells[2, 6].Value = KSHMEM;

                //新增圓餅圖
                ExcelBarChart Chart6Cities = wsCity.Drawings.AddChart("會員直轄市分布圖", eChartType.BarClustered) as ExcelBarChart;
                Chart6Cities.Title.Text = "會員直轄市分布圖";//圖表名字
                //圓餅圖資料範圍
                var o  =Chart6Cities.Series.Add(ExcelRange.GetAddress(2, 1, 2, 6), ExcelRange.GetAddress(1, 1, 1, 6));
                //圖例
                Chart6Cities.Legend.Position = eLegendPosition.Bottom;
                Chart6Cities.XAxis.Title.Text = "6大直轄市";
                Chart6Cities.YAxis.Title.Text = "會員人數";
                o.Header = "會員人數";//bar數值的值
                //是否秀%數
                Chart6Cities.DataLabel.ShowValue = true;

                Chart6Cities.SetSize(300, 400);
                Chart6Cities.SetPosition(3, 0, 2, 0);

                //================================================

                ExcelWorksheet wsMem = excelPackage.Workbook.Worksheets.Add("會員人數成長圖");
                //取得資料
                DataTable dtMem = GetMemberGrowthDataTable();

                //將datatable資料塞進sheet中
                wsMem.Cells["A1"].LoadFromDataTable(dtMem, true);

                //自動調整大小
                wsMem.Cells[wsMem.Dimension.Address].AutoFitColumns();
                //設定EXCEL樣式
                using (ExcelRange rng = wsMem.Cells["A1:B1"])
                {
                    rng.Style.Font.Bold = true;
                    rng.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    rng.Style.Fill.BackgroundColor.SetColor(Color.Gray);
                    rng.Style.Font.Color.SetColor(Color.White);
                    rng.Style.HorizontalAlignment = ExcelHorizontalAlignment.General;
                }

                ExcelBarChart ChartMemGrowth = wsMem.Drawings.AddChart("會員人數成長圖", eChartType.BarClustered) as ExcelBarChart;
                ChartMemGrowth.Title.Text = "會員人數成長圖";//圖表名字
                //圓餅圖資料範圍
                var chmg = ChartMemGrowth.Series.Add(ExcelRange.GetAddress(2, 2, 7, 2), ExcelRange.GetAddress(2, 1, 7, 1));
                //圖例
                ChartMemGrowth.Legend.Position = eLegendPosition.Bottom;
                ChartMemGrowth.XAxis.Title.Text = "年份";
                ChartMemGrowth.YAxis.Title.Text = "會員人數";
                chmg.Header = "會員人數";//bar數值的值
                //是否秀%數
                ChartMemGrowth.DataLabel.ShowValue = true;

                ChartMemGrowth.SetSize(300, 400);
                ChartMemGrowth.SetPosition(3, 0, 2, 0);
                //=======================================================================================

                ExcelWorksheet wsProductPrice = excelPackage.Workbook.Worksheets.Add("產品種類平均銷售價格");

                DataTable dtpp = GetProductPriceDataTable();
                wsProductPrice.Cells["A1"].LoadFromDataTable(dtpp, true);

                //自動調整大小
                wsProductPrice.Cells[wsProductPrice.Dimension.Address].AutoFitColumns();
                //設定EXCEL樣式
                using (ExcelRange rng = wsProductPrice.Cells["A1:B1"])
                {
                    rng.Style.Font.Bold = true;
                    rng.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    rng.Style.Fill.BackgroundColor.SetColor(Color.Gray);
                    rng.Style.Font.Color.SetColor(Color.White);
                    rng.Style.HorizontalAlignment = ExcelHorizontalAlignment.General;
                }

                ExcelBarChart ChartProductPrice = wsProductPrice.Drawings.AddChart("產品種類平均銷售價格", eChartType.BarClustered) as ExcelBarChart;
                ChartProductPrice.Title.Text = "產品種類平均銷售價格";//圖表名字
                //圓餅圖資料範圍
                var chpp = ChartProductPrice.Series.Add(ExcelRange.GetAddress(2, 2, 9, 2), ExcelRange.GetAddress(2, 1, 9, 1));
                //圖例
                ChartProductPrice.Legend.Position = eLegendPosition.Bottom;
                ChartProductPrice.XAxis.Title.Text = "產品名稱";
                ChartProductPrice.YAxis.Title.Text = "產品平均價格";
                chpp.Header = "產品平均價格";//bar數值的值
                //是否秀%數
                ChartProductPrice.DataLabel.ShowValue = true;

                ChartProductPrice.SetSize(300, 400);
                ChartProductPrice.SetPosition(3, 0, 2, 0);
                //============================================================================================
                ExcelWorksheet wstopproduct = excelPackage.Workbook.Worksheets.Add("商品銷售排行榜");
                //取得資料
                DataTable dttopproduct = GetTopProductDataTable();

                //將datatable資料塞進sheet中
                wstopproduct.Cells["A1"].LoadFromDataTable(dttopproduct, true);

                //自動調整大小
                wstopproduct.Cells[wstopproduct.Dimension.Address].AutoFitColumns();
                //設定EXCEL樣式
                using (ExcelRange rng = wstopproduct.Cells["A1:B1"])
                {
                    rng.Style.Font.Bold = true;
                    rng.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    rng.Style.Fill.BackgroundColor.SetColor(Color.Gray);
                    rng.Style.Font.Color.SetColor(Color.White);
                    rng.Style.HorizontalAlignment = ExcelHorizontalAlignment.General;
                }

                ExcelBarChart ChartTopProduct = wstopproduct.Drawings.AddChart("商品銷售排行榜", eChartType.BarClustered) as ExcelBarChart;
                ChartTopProduct.Title.Text = "商品銷售排行榜";//圖表名字
                //圓餅圖資料範圍
                var chtop = ChartTopProduct.Series.Add(ExcelRange.GetAddress(2, 2, 9, 2), ExcelRange.GetAddress(2, 1, 9, 1));
                //圖例
                ChartTopProduct.Legend.Position = eLegendPosition.Bottom;
                ChartTopProduct.XAxis.Title.Text = "產品名稱";
                ChartTopProduct.YAxis.Title.Text = "產品數量";
                chpp.Header = "產品數量";//bar數值的值
                //是否秀%數
                ChartTopProduct.DataLabel.ShowValue = true;

                ChartTopProduct.SetSize(600, 800);
                ChartTopProduct.SetPosition(3, 0, 2, 0);



                //===================
                var stream = new MemoryStream();
                //存檔
                excelPackage.SaveAs(stream);


                string fileName = DateTime.Now.ToShortDateString()+"Chart" + ".xlsx";

                

                string contentType = "application / vnd.openxmlformats - officedocument.spreadsheetml.sheet";

                stream.Position = 0;
                return File(stream, contentType, fileName);

            }


            

        }
    }
}