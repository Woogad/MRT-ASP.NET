using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MRT_Demo.Models
{
    public class IndicatorDetailStatusMetadata
    {
        public int ID { get; set; }
        public string Status { get; set; }
    }
    [MetadataType(typeof(IndicatorDetailStatusMetadata))]
    public partial class IndicatorDetailStatus
    {
    }
}