//------------------------------------------------------------------------------
// <auto-generated>
//     這個程式碼是由範本產生。
//
//     對這個檔案進行手動變更可能導致您的應用程式產生未預期的行為。
//     如果重新產生程式碼，將會覆寫對這個檔案的手動變更。
// </auto-generated>
//------------------------------------------------------------------------------

namespace prjNewPet.Models
{
    using System;
    using System.Collections.Generic;
    using System.Web;

    public partial class tMember
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public tMember()
        {
            this.tAdoptMessages = new HashSet<tAdoptMessage>();
            this.tComments = new HashSet<tComment>();
            this.tDiscussions = new HashSet<tDiscussion>();
            this.tFavorites = new HashSet<tFavorite>();
            this.tFoundPets = new HashSet<tFoundPet>();
            this.tLikes = new HashSet<tLike>();
            this.tOrders = new HashSet<tOrder>();
            this.tPetMembers = new HashSet<tPetMember>();
            this.tReports = new HashSet<tReport>();
            this.tShoppingCarts = new HashSet<tShoppingCart>();
        }
    
        public int fMemberID { get; set; }
        public string fName { get; set; }
        public string fGender { get; set; }
        public string fIDCardNumber { get; set; }
        public string fNickName { get; set; }
        public string fAccount { get; set; }
        public string fPassword { get; set; }
        public System.DateTime fBirthDate { get; set; }
        public string fEnconomicStatus { get; set; }
        public string fPhone { get; set; }
        public string fEMail { get; set; }
        public Nullable<System.DateTime> fRegisterDate { get; set; }
        public Nullable<int> fLogInWayID { get; set; }
        public string fIcon { get; set; }
        public Nullable<System.DateTime> fRecentLogInDate { get; set; }
        public int fCityID { get; set; }
        public int fRegionID { get; set; }
        public string fAddressDetail { get; set; }
        public Nullable<decimal> fPetCoin { get; set; }

        public HttpPostedFileBase photo { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tAdoptMessage> tAdoptMessages { get; set; }
        public virtual tCity tCity { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tComment> tComments { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tDiscussion> tDiscussions { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tFavorite> tFavorites { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tFoundPet> tFoundPets { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tLike> tLikes { get; set; }
        public virtual tRegion tRegion { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tOrder> tOrders { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tPetMember> tPetMembers { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tReport> tReports { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tShoppingCart> tShoppingCarts { get; set; }
    }
}