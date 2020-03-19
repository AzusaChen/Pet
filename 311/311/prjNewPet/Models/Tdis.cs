using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace prjNewPet.Models
{
    public class Tdis
    {
        public int fArticleID { get; set; }
        public string fMemberID { get; set; }

        public string fDiscussionClassID { get; set; }
        public string fTitle { get; set; }
        public Nullable<System.DateTime> fCreateDate { get; set; }
        public Nullable<int> fLike { get; set; }
        public Nullable<bool> fLock { get; set; }
        public Nullable<bool> fReportcheck { get; set; }
        public virtual tDiscussionClass tDiscussionClass { get; set; }
        public virtual tMember tMember { get; set; }
    }
}