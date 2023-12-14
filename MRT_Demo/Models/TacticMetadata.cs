using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MRT_Demo.Models
{
    public class TacticMetadata
    {
        public int ID { get; set; }
        [DisplayName("รหัส")]
        public Nullable<int> NO { get; set; }
        [DisplayName("กลยุทธ์ (Tactics)")]
        [Required]
        public string Tactic1 { get; set; }
        public Nullable<int> StrategyID { get; set; }
    }
    [MetadataType(typeof(TacticMetadata))]
    public partial class Tactic
    {
        public bool IsDeleteTactic { get; set; }
    }
}