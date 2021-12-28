using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ParentalControl.Web.Api.Models.EntityModels
{
    public class ScheduleModel
    {
        public int ScheduleId { get; set; }
        public string ScheduleTime { get; set; }

        /*public DateTime ScheduleStartTime { get; set; }
        public DateTime ScheduleEndTime { get; set; }
        public DateTime ScheduleCreationDate { get; set; }
	    public int ParentId { get; set; }*/
    }
}