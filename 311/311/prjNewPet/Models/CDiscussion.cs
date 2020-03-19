using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace prjNewPet.Models
{
    public class CDiscussion
    {
        public List<tDiscussion> Discussion { get; set; }
        public List<tDiscussionClass> DiscussionClass { get; set; }
    }
}