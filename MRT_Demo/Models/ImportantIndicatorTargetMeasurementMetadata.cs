using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MRT_Demo.Models
{
    public class ImportantIndicatorTargetMeasurementMetadata
    {
        [DisplayName("ชื่อตัวชี้วัด")]
        public Nullable<int> IndicatorTypeID { get; set; }

        [DisplayName("หน่วยวัด")]
        public Nullable<int> IndicatorUnitID { get; set; }

        [DisplayName("ปีงบประมาณ")]
        public Nullable<int> Year { get; set; }

        [DisplayName("ค่าเป้าหมาย/เกณฑ์วัด")]
        public Nullable<double> Target { get; set; }
    }

    [MetadataType(typeof(ImportantIndicatorTargetMeasurementMetadata))]
    public partial class ImportantIndicatorTargetMeasurement
    {
        public bool IsDeleteTarget {  get; set; }
        public bool IsDispaly { get; set; }
        public bool IsUnCheck { get; set; }
        public List<ImportantIndicatorTargetMeasurement> TargetList { get; set; }
    }
}