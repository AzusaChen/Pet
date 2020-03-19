using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace prjNewPet.ViewModel
{
    public class CLogin
    {
        //View/Login電子郵件、密碼、checkbox輸入欄位的name
        public string account { get; set; }
        public string password { get; set; }
        public bool isRememberMe { get; set; }
        public string code { get; set; }
    }
}