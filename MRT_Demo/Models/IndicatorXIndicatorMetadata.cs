using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MRT_Demo.Models
{
    public class IndicatorXIndicatorMetadata
    {

        [DisplayName("คำจำกัดความตัวชี้วัด")]
        public string Definition { get; set; }
    }

    [MetadataType(typeof(IndicatorXIndicatorMetadata))]
    public partial class IndicatorXIndicatorType
    {
    }
}