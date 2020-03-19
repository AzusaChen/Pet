using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace prjNewPet.Models
{
    public class Trep
    {
        public int fReportID { get; set; }
        public string fArticleID { get; set; }
        public string fMemberID { get; set; }
        public string fReportComment { get; set; }

        public virtual tDiscussion tDiscussion { get; set; }
        public virtual tMember tMember { get; set; }
    }
}