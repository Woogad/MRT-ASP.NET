using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MRT_Demo.Models
{
    public class ForecastPeriodCompetitorValueMetadata
    {
        [DisplayName("ผลดำเนินงานคู่เทียบ/คู่แข่ง")]
        public string Detail { get; set; }
    }

    [MetadataType(typeof(ForecastPeriodCompetitorValueMetadata))]
    public partial class ForecastPeriodCompetitorValue
    {

    }
}