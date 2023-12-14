using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MRT_Demo.Models
{
    public class StrategyMetadata
    {
        public int ID { get; set; }
        [DisplayName("รหัส")]
        public Nullable<int> NO { get; set; }
        [DisplayName("ยุทธศาสตร์ (Strategies)")]
        [Required]
        public string Strategy1 { get; set; }
        public Nullable<int> StrategicObjectiveID { get; set; }
        [DisplayName("กลยุทธ์ (Tactic)")]
        public virtual ICollection<Tactic> Tactic { get; set; }

    }

    [MetadataType(typeof(StrategyMetadata))]
    public partial class Strategy
    {

    }
}