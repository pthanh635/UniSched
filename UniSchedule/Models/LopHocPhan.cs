//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace UniSchedule.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class LopHocPhan
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public LopHocPhan()
        {
            this.PhanCongGiangDays = new HashSet<PhanCongGiangDay>();
        }
    
        public string MaLHP { get; set; }
        public string TenMH { get; set; }
        public double SoTinChi { get; set; }
        public int SoTietMoiTuan { get; set; }
        public int SoLuongSinhVien { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PhanCongGiangDay> PhanCongGiangDays { get; set; }
    }
}
