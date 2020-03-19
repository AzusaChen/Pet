using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using static System.Collections.Specialized.BitVector32;

namespace prjNewPet.Models
{
    public class CSendMail
    {
        public async Task sendOrderDetail(string email, int orderId)
        {
            dbNewPetEntities db = new dbNewPetEntities();

            var q = from od in db.tOrder_Detail
                    where od.tOrder.fReceiverMail == email && od.fOrderID == orderId
                    select od;

            decimal total = 0;
            foreach (var y in q)
            {
                total += (y.fQuantity) * (y.fUnitPrice);
            }
            MailMessage mail = new MailMessage();
            //前面是發信email後面是顯示的名稱

            //內容使用html
            mail.IsBodyHtml = true;

            string name = "";
            foreach (var z in q)
            {
                name = z.tOrder.tMember.fName;
            }

            string htmlBody = $@"親愛的會員{name}您好，您本次訂購的訂單如下：";
            htmlBody += $@"<div>
                            <table style='border:solid'>
                                 <thead>
                                      <tr style='border: solid,1px'>
                                         <th>產品名稱</th>
                                        <th>數量</th>
                                        <th>價錢</th>
                                    </tr>
                                </thead><tbody>";
            foreach (var x in q)
            {
                htmlBody +=
                    $@"
                         <tr style='border: solid'>
                             <td>{x.fName}</td>
                             <td>{x.fQuantity}</td>
                             <td>NT.{(x.fUnitPrice * x.fQuantity).ToString("F0")}</td>
                         </tr>

                    ";
            };

            htmlBody += $@" <tr>
                            <td colspan = '3' >

                                 <div style = 'text-align:center;margin-bottom:10px;'>

                                      <span class='lighter-text'>總金額：</span>
                                       <span class='main-color-text'>NT.{total.ToString("F0")}</span>
                                </div>
                             </td>
                            </tr></tbody></table></div>";
            //建立AlternativeView
            AlternateView altView = AlternateView.CreateAlternateViewFromString(htmlBody, null, MediaTypeNames.Text.Html);

            //將AlternativeView加入MailMessage
            mail.AlternateViews.Add(altView);

            //收信者email
            mail.To.Add(email);

            mail.From = new MailAddress("kof7835377@gmail.com", "寵愛有家_訂單通知");

            //設定優先權
            mail.Priority = MailPriority.Normal;

            //標題
            mail.Subject = "寵愛有家_訂單通知";

            //設定gmail的smtp (這是google的)
            SmtpClient MySmtp = new SmtpClient("smtp.gmail.com", 587);

            //您在gmail的帳號密碼
            MySmtp.Credentials = new System.Net.NetworkCredential("kof7835377@gmail.com", "cwlcisaavcahicve");

            //開啟ssl
            MySmtp.EnableSsl = true;

            //發送郵件
            MySmtp.Send(mail);

            //放掉宣告出來的MySmtp
            MySmtp = null;

            //放掉宣告出來的mail
            mail.Dispose();
        }

        public async Task sendPassword(string email)
        {
            dbNewPetEntities db = new dbNewPetEntities();

            string pwd = (from p in db.tMembers
                          where p.fEMail == email
                          select p).FirstOrDefault().fPassword;

            MailMessage mail = new MailMessage();
            //前面是發信email後面是顯示的名稱

            //內容使用html
            mail.IsBodyHtml = true;

            string htmlBody =

                    $@"<div>
                        您好，這是您的密碼{pwd}。

                   </div>";

            //建立AlternativeView
            AlternateView altView = AlternateView.CreateAlternateViewFromString(htmlBody, null, MediaTypeNames.Text.Html);

            //將AlternativeView加入MailMessage
            mail.AlternateViews.Add(altView);

            //收信者email
            mail.To.Add(email);

            mail.From = new MailAddress("kof7835377@gmail.com", "寵愛有家_密碼通知");

            //設定優先權
            mail.Priority = MailPriority.Normal;

            //標題
            mail.Subject = "寵愛有家_密碼通知";

            //設定gmail的smtp (這是google的)
            SmtpClient MySmtp = new SmtpClient("smtp.gmail.com", 587);

            //您在gmail的帳號密碼
            MySmtp.Credentials = new System.Net.NetworkCredential("kof7835377@gmail.com", "cwlcisaavcahicve");

            //開啟ssl
            MySmtp.EnableSsl = true;

            //發送郵件
            MySmtp.Send(mail);

            //放掉宣告出來的MySmtp
            MySmtp = null;

            //放掉宣告出來的mail
            mail.Dispose();
        }

        public async Task sendGmail(List<string> email, tFoundPet fp)
        {
            dbNewPetEntities db = new dbNewPetEntities();

            var fpinfo = (from m in db.tMembers
                          where m.fMemberID == fp.fMemberID
                          select m).FirstOrDefault();

            var location = (from l in db.tCities
                            where l.fCityID == fp.fCityID
                            select l).FirstOrDefault();

            fpForMemNeed fpneed = new fpForMemNeed()
            {
                fContactNum = fpinfo.fPhone,
                fContactName = fpinfo.fName,
                fFoundTime = fp.fFoundTime.ToShortDateString(),
                fCity = location.fName,
                fRegion = (location.tRegions.Where(r => r.fRegionID == fp.fRegionID)).FirstOrDefault().fName,
                fRemark = fp.fRemark,
                fPetPic = fp.fPetPic,
            };

            //如果偵錯中斷，可能是找不到圖片路徑
            string imgPath = @"C:\Users\leon\Downloads\定版2.0\311\311\prjNewPet\images\";

            MailMessage mail = new MailMessage();
            //前面是發信email後面是顯示的名稱

            //內容使用html
            mail.IsBodyHtml = true;

            //建立連結資源 如果偵錯中斷，可能是找不到圖片路徑
            LinkedResource res = new LinkedResource(imgPath + fpneed.fPetPic);

            res.ContentId = "1";
            //使用<img src="/img/loading.svg" data-src="cid:..."方式引用內嵌圖片

            string htmlBody = "";

            if (fpneed.fRemark.Contains("Perfect"))
            {
                string newaddress = fpneed.fRemark.Remove(0, 7);
                htmlBody +=
                    $@"<div>
                        您好，這是來自寵愛有家的最新尋獲資料，
                        根據晶片號碼比對結果，這是您走失的寵物，
                        請馬上聯絡尋獲者。
                        <ul>
                            <li>
                             尋獲時間:{fpneed.fFoundTime}
                            </li>
                            <li>
                             尋獲地址:{fpneed.fCity},{fpneed.fRegion},{newaddress}
                            </li>
                            <li>
                              聯絡人:{fpneed.fContactName},電話:{fpneed.fContactNum}
                            </li>
                        </ul>
                   </div>
                   <div>
                        <img src='cid:{res.ContentId}'/>
                   </div>";
            }
            else
            {
                htmlBody +=
                   $@"<div>
                        您好，這是來自寵愛有家的最新尋獲資料，
                        如果是您走失的寵物，請馬上聯絡尋獲者。
                        <ul>
                            <li>
                             尋獲時間:{fpneed.fFoundTime}
                            </li>
                            <li>
                             尋獲地址:{fpneed.fCity},{fpneed.fRegion},{fpneed.fRemark}
                            </li>
                            <li>
                              聯絡人:{fpneed.fContactName},電話:{fpneed.fContactNum}
                            </li>
                        </ul>
                   </div>
                   <div>
                        <img src='cid:{res.ContentId}'/>
                   </div>";
            }

            //建立AlternativeView
            AlternateView altView = AlternateView.CreateAlternateViewFromString(htmlBody, null, MediaTypeNames.Text.Html);

            //將圖檔資源加入AlternativeView
            altView.LinkedResources.Add(res);

            //將AlternativeView加入MailMessage
            mail.AlternateViews.Add(altView);

            //收信者email
            foreach (string e in email)
            {
                mail.To.Add(e);
            }

            mail.From = new MailAddress("kof7835377@gmail.com", "寵物尋獲通知");

            //設定優先權
            mail.Priority = MailPriority.Normal;

            //標題
            mail.Subject = "尋獲通知";

            //設定gmail的smtp (這是google的)
            SmtpClient MySmtp = new SmtpClient("smtp.gmail.com", 587);

            //您在gmail的帳號密碼
            MySmtp.Credentials = new System.Net.NetworkCredential("kof7835377@gmail.com", "cwlcisaavcahicve");

            //開啟ssl
            MySmtp.EnableSsl = true;

            //發送郵件
            MySmtp.Send(mail);

            //放掉宣告出來的MySmtp
            MySmtp = null;

            //放掉宣告出來的mail
            mail.Dispose();
        }

        public async Task sendPurchaseDetail(string email, int purchaseID)
        {
            dbNewPetEntities db = new dbNewPetEntities();

            var q = from pd in db.tPurchase_Detials
                    where pd.fPurchaseID == purchaseID
                    select pd;

            decimal total = 0;
            foreach (var y in q)
            {
                total += (y.fQuantity) * (y.fUnitPrice);
            }
            MailMessage mail = new MailMessage();
            //前面是發信email後面是顯示的名稱

            //內容使用html
            mail.IsBodyHtml = true;

            //string name = "";
            //foreach (var z in q)
            //{
            //    name = z.tOrder.tMember.fName;
            //}

            string htmlBody = $@"寵愛有家採購部您好，本次採購的訂單如下：";
            htmlBody += $@"<div>
                            <table style='border:solid'>
                                 <thead>
                                      <tr style='border: solid,1px'>
                                         <th>產品名稱</th>
                                        <th>數量</th>
                                        <th>價錢</th>
                                    </tr>
                                </thead><tbody>";
            foreach (var x in q)
            {
                htmlBody +=
                    $@"
                         <tr style='border: solid'>
                             <td>{x.fName}</td>
                             <td>{x.fQuantity}</td>
                             <td>NT.{(x.fUnitPrice * x.fQuantity).ToString("F0")}</td>
                         </tr>

                    ";
            };

            htmlBody += $@" <tr>
                            <td colspan = '3' >

                                 <div style = 'text-align:center;margin-bottom:10px;'>

                                      <span class='lighter-text'>總金額：</span>
                                       <span class='main-color-text'>NT.{total.ToString("F0")}</span>
                                </div>
                             </td>
                            </tr></tbody></table></div>";
            //建立AlternativeView
            AlternateView altView = AlternateView.CreateAlternateViewFromString(htmlBody, null, MediaTypeNames.Text.Html);

            //將AlternativeView加入MailMessage
            mail.AlternateViews.Add(altView);

            //收信者email
            mail.To.Add(email);

            mail.From = new MailAddress("kof7835377@gmail.com", "寵愛有家_採購單通知");

            //設定優先權
            mail.Priority = MailPriority.Normal;

            //標題
            mail.Subject = "寵愛有家_採購單通知";

            //設定gmail的smtp (這是google的)
            SmtpClient MySmtp = new SmtpClient("smtp.gmail.com", 587);

            //您在gmail的帳號密碼
            MySmtp.Credentials = new System.Net.NetworkCredential("kof7835377@gmail.com", "cwlcisaavcahicve");

            //開啟ssl
            MySmtp.EnableSsl = true;

            //發送郵件
            MySmtp.Send(mail);

            //放掉宣告出來的MySmtp
            MySmtp = null;

            //放掉宣告出來的mail
            mail.Dispose();
        }
    }
}

public class mailInfo
{
    public List<string> MailTos { get; set; }
    public string mailBody { get; set; }
    //public List<string> filePaths { get; set; }
}

public class fpForMemNeed
{
    public string fContactNum { get; set; }
    public string fFoundTime { get; set; }
    public string fCity { get; set; }
    public string fRegion { get; set; }
    public string fRemark { get; set; }
    public string fPetPic { get; set; }
    public string fContactName { get; set; }
    public string fCompareLevel { get; set; }
}