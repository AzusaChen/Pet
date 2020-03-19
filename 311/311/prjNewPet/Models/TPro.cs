using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace prjNewPet.Models
{
    public class TPro
    {
        public int fProductID { get; set; }
        public string fName { get; set; }
        public string fPic { get; set; }
        public string fCategoryName { get; set; }
        public double fUnitPrice { get; set; }
        
        public int fUnitInStock { get; set; }
        
        public string fSupplierName { get; set; }
        
       

        public virtual tCategory tCategory { get; set; }

        public virtual tSupplier tSupplier { get; set; }



    }
}