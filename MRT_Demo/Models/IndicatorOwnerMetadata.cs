using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MRT_Demo.Models
{
    public class IndicatorOwnerMetadata
    {
        [DisplayName("หน่วยงาน")]
        public string Division { get; set; }
    }

    [MetadataType(typeof(IndicatorOwnerMetadata))]
    public partial class IndicatorOwner
    {
        public bool IsDeleteOwner { get; set; }
    }
}