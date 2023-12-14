using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MRT_Demo.Models
{
    public class IndicatorUnitMetadata
    {
        [DisplayName("หน่วยวัด")]
                public Nullable<int> Unit { get; set; }
    }

    [MetadataType(typeof(IndicatorUnitMetadata))]
    public partial class IndicatorUnit
    {
        public bool IsDeleteUnit { get; set; }
    }
}