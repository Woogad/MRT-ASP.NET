using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MRT_Demo.Models
{
    public class IndicatorMetadata
    {
        public int ID { get; set; }

        [DisplayName("ตัวชี้วัด/เกณฑ์วัดการดำเนินงาน")]
        public Nullable<int> Indicator1 { get; set; }

        [DisplayName("กำหนดสูตรการคำนวน")]
        public string Formula { get; set; }

        [DisplayName("รายละเอียดตัวชี้วัด")]
        public Nullable<int> IndicatorDetailStatusID { get; set; }

        [DisplayName("สถานะของตัวชี้วัด")]
        public Nullable<bool> IsActive { get; set; }

        [DisplayName("วันที่ปรับปรุง")]
        public Nullable<System.DateTime> UpdateDate { get; set; }

        [DisplayName("หน่วยงานผู้รับผิดชอบตัวชี้วัด")]
        public virtual ICollection<IndicatorOwner> IndicatorOwner { get; set; }

        [DisplayName("หน่วยวัด")]
        public virtual ICollection<IndicatorUnit> IndicatorUnit { get; set; }

        [DisplayName("ประเภทตัวชี้วัด")]
        public virtual ICollection<IndicatorXIndicatorType> IndicatorXIndicatorType { get; set; }


    }

    [MetadataType(typeof(IndicatorMetadata))]
    public partial class Indicator
    {
        public int PeriodSelected_Index { get; set; }
        public int ForeCastPeriodSelected_Index { get; set; }
    }
}