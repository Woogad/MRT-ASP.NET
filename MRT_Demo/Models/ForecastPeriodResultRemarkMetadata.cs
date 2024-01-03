using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MRT_Demo.Models
{
    public class ForecastPeriodResultRemarkMetadata
    {
        [DisplayName("ในกรณีที่มีการเปลียนแปลงเครื่องมือระหว่างปีโปรดระบุเหตุผล/สาเหตุในการเปลียนแปลงด้วย เช่น พบว่าค่าคาดการณ์มีความคลาดเคลื่อนหรือไม่แม่นยำ เป็นต้น")]
        public string ReasonForToolChange { get; set; }

        [DisplayName("ผลการวิเคราะห์-ประเมิน-จัดความเสียง ณ งวด")]
        public bool IsAnalysisResults { get; set; }

        [DisplayName("ผลการวิเคราะห์-ประเมิน-จัดความเสียง ณ งวด")]
        public string AnalysisResults { get; set; }

        [DisplayName("ทบทวน/ปรับปรุงและหรือจัดทำเพิ่มแผนปฎิบัติการ")]
        public bool IsChangeActionPlan { get; set; }

        [DisplayName("ทบทวน/ปรับปรุงและหรือจัดทำเพิ่มแผนปฎิบัติการ")]
        public string ChangeActionPlan { get; set; }

        [DisplayName("เร่งรัด/ปรับปรุงการดำเนินงาน")]
        public bool IsChangeOperation { get; set; }

        [DisplayName("เร่งรัด/ปรับปรุงการดำเนินงาน")]
        public string ChangeOperation { get; set; }

        [DisplayName("อื่นๆโปรดระบุ")]
        public bool IsOther { get; set; }

        [DisplayName("อื่นๆโปรดระบุ")]
        public string Other { get; set; }
    }

    [MetadataType(typeof(ForecastPeriodResultRemarkMetadata))]
    public partial class ForecastPeriodResultRemark
    {
        [DisplayName("ข้อมูลประกอบและเอกสารเพิ่มเติมอื่นๆ (ส่ง soft file)")]
        public List<HttpPostedFileBase> FilePeriodDoc { get; set; }

        [DisplayName("ผลการวิเคราะห์-ประเมิน-จัดการความเสี่ยง ณ งวด")]
        public List<HttpPostedFileBase> FileAnalysisResults { get; set; }

        [DisplayName("ทบทวน/ปรับปรุงและหรือจัดทำเพิ่มแผนปฎิบัติการ")]
        public List<HttpPostedFileBase> FileChangeActionPlan { get; set; }
    }
}