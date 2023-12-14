using System;
using System.CodeDom;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MRT_Demo.Models
{
    public class SOEPlanMetadata
    {
        public int ID { get; set; }
        [DisplayName("ระยะเวลาแผน")]
        [Required]
        public Nullable<int> StartYear { get; set; }
        [DisplayName("ระยะเวลาแผน")]
        [Required]
        public Nullable<int> EndYear { get; set; }

    }
    [MetadataType(typeof(SOEPlanMetadata))]
    public partial class SOEPlan
    {

    }
}