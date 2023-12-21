using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MRT_Demo.Models
{
    public class ImportantIndicatorResultMeasurementMetadata
    {
        [DisplayName("ปีงบประมาณ")]
        public Nullable<int> Year { get; set; }
        [DisplayName("ความถี่ในการคาดการณ์")]
        public Nullable<int> PeriodMounthOrQuarterOrYearID { get; set; }
        [DisplayName("งวดการคาดการณ์")]
        public virtual ICollection<ForecastPeriod> ForecastPeriod { get; set; }
    }

    [MetadataType(typeof(ImportantIndicatorResultMeasurementMetadata))]
    public partial class ImportantIndicatorResultMeasurement
    {
        public string PMoYName {  get; set; } 
    }
}