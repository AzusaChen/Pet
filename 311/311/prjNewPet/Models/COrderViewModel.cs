using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace prjNewPet.Models
{
    public class COrderViewModel
    {
        public string fRn { get; set; }
        public string fRp { get; set; }
        public int fcId { get; set; }
        public int frId { get; set; }
        public string fAd { get; set; }
        public string fRm { get; set; }
        public int fPwId { get; set; }
        public bool fPC { get; set; }
    }
}