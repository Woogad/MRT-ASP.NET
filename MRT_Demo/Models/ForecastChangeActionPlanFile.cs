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
    
    public partial class ForecastChangeActionPlanFile
    {
        public int ID { get; set; }
        public Nullable<int> ForecastPeriodResultRemarkID { get; set; }
        public Nullable<int> FilePathID { get; set; }
        public Nullable<int> CreateBy { get; set; }
        public Nullable<int> UpdateBy { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }
        public Nullable<System.DateTime> UpdateDate { get; set; }
        public Nullable<bool> IsDelete { get; set; }
        public Nullable<bool> IsLastDelete { get; set; }
    
        public virtual ForecastPeriodResultRemark ForecastPeriodResultRemark { get; set; }
        public virtual MRTFile MRTFile { get; set; }
    }
}
