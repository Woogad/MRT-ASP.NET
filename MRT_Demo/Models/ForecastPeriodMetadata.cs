using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MRT_Demo.Models
{
    public class ForecastPeriodMetadata
    {
        [DisplayName("ผลดำเนินงานของ คู่เทียบ/คู่แข่ง (หน่วยงาน/องค์กร/บริษัท)")]
        public virtual ICollection<ForecastPeriodCompetitorValue> ForecastPeriodCompetitorValue { get; set; }
    }

    [MetadataType(typeof(ForecastPeriodMetadata))]
    public partial class ForecastPeriod
    {
        public bool IsSelect { get; set; }
    }
}