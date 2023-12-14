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
    
    public partial class ImportantIndicatorTargetMeasurement
    {
        public int ID { get; set; }
        public Nullable<int> IndicatorID { get; set; }
        public Nullable<int> IndicatorTypeID { get; set; }
        public Nullable<int> IndicatorUnitID { get; set; }
        public Nullable<int> IndicatorLevel { get; set; }
        public Nullable<int> Year { get; set; }
        public Nullable<double> Target { get; set; }
        public Nullable<int> CreateBy { get; set; }
        public Nullable<int> UpdateBy { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }
        public Nullable<System.DateTime> UpdateDate { get; set; }
        public Nullable<bool> IsDelete { get; set; }
    
        public virtual Indicator Indicator { get; set; }
        public virtual IndicatorType IndicatorType { get; set; }
        public virtual IndicatorUnit IndicatorUnit { get; set; }
    }
}