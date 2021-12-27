using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ParentalControl.Web.Api.Models.ReponseModels
{
    public class ScheduleResponseModel
    {
        public int ScheduleId { get; set; }
        public DateTime ScheduleStartTime { get; set; }
        public DateTime ScheduleEndTime { get; set; }
        //public DateTime ScheduleCreationDate { get; set; }
        public int? ParentId { get; set; }
        public string MessageError { get; set; }
        public bool IsSuccess { get; set; }
        public bool Registered { get; set; }
    }
}