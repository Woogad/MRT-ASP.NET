using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MRT_Demo.Models
{
    public class StrategicObjectiveMetadata
    {
        public int ID { get; set; }
        [DisplayName("รหัส")]
        public Nullable<int> NO { get; set; }

        [DisplayName("วัตถุประสงค์เชิงยุทธศาสตร์ (Strategic Objective)")]
        [Required]
        public string StrategicObjective1 { get; set; }
        public Nullable<int> SOEPlanID { get; set; }
        [DisplayName("เป้าประสงค์ (Goals)")]
        public virtual ICollection<Goal> Goal { get; set; }
    }

    [MetadataType(typeof(StrategicObjectiveMetadata))]
    public partial class StrategicObjective
    {

    }

}