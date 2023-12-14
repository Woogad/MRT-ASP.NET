using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MRT_Demo.Models
{
    public class GoalMetadata
    {
        public int ID { get; set; }
        [DisplayName("รหัส")]
        public Nullable<int> NO { get; set; }
        [DisplayName("เป้าหมายประสงค์ (Goals)")]
        [Required(ErrorMessage = "Goal is required.")]
        public string Goal1 { get; set; }
        public Nullable<int> StrategicObjectiveID { get; set; }
    }

    [MetadataType(typeof(GoalMetadata))]
    public partial class Goal
    {
        public bool IsDeleteGoal { get; set; }
    }
}