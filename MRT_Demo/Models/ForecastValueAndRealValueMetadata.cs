using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MRT_Demo.Models
{
    public class ForecastValueAndRealValueMetadata
    {
        [DisplayName("ผลดำเนินการงานจริง ณ งวดที่ทำการคาดการณ์")]
        public Nullable<int> ForecastValue { get; set; }
        [DisplayName("ค่าคาดการณ์ผลดำเนินงานจริง ณ 30 ก.ย")]
        public Nullable<int> RealValue { get; set; }

    }

    [MetadataType(typeof(ForecastValueAndRealValueMetadata))]
    public partial class ForecastValueAndRealValue
    {

    }
}