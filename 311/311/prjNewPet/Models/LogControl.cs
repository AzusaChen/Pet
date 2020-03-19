using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace prjNewPet.Models
{
    public class LogControl
    {
        static object lockLog = new object();
        //Write Log -
        public static void DMMLog(string txt)
        {
            
                //今日日期
                DateTime Date = DateTime.Now;
                string TodyTime = Date.ToString("yyyy-MM-dd HH:mm:ss");
                string Tody = Date.ToString("yyyy-MM-dd");

                //檢查此路徑有無資料夾
                if (!Directory.Exists(@"C:\WebLogRecord"))
                {
                    //新增資料夾
                    Directory.CreateDirectory(@"C:\WebLogRecord");
                }
                lock (lockLog)
                {
                    //把內容寫到目的檔案，若檔案存在則附加在原本內容之後(換行)
                    File.AppendAllText(@"C:\WebLogRecord\" + Tody + ".txt", "\r\n========\n" + TodyTime + "：" + txt);
                }
            
            
           
        }
    }
}