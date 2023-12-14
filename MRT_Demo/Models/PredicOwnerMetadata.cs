using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MRT_Demo.Models
{
    public class PredicOwnerMetadata
    {
    }

    [MetadataType(typeof(PredicOwnerMetadata))]
    public partial class PredicOwner
    {
        public bool IsDeletePredicOwner { get; set; }
    }
}