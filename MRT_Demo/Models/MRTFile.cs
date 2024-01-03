//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MRT_Demo.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class MRTFile
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public MRTFile()
        {
            this.ForecastAnalysisResultFile = new HashSet<ForecastAnalysisResultFile>();
            this.ForecastChangeActionPlanFile = new HashSet<ForecastChangeActionPlanFile>();
            this.ForecastPeriodDocFile = new HashSet<ForecastPeriodDocFile>();
        }
    
        public int ID { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public Nullable<bool> isDelete { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ForecastAnalysisResultFile> ForecastAnalysisResultFile { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ForecastChangeActionPlanFile> ForecastChangeActionPlanFile { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ForecastPeriodDocFile> ForecastPeriodDocFile { get; set; }
    }
}
