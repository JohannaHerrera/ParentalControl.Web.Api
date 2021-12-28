using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ParentalControl.Web.Api.Models.EntityModels
{
    public class ScheduleRegisterModel
    {
        //public int? ScheduleId { get; set; }
        public int ParentId { get; set; }
        public DateTime ScheduleStartTime { get; set; }
        public DateTime ScheduleEndTime { get; set; }
        public int Action { get; set; }



    }
}