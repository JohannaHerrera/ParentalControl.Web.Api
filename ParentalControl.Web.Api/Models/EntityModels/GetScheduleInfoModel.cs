using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ParentalControl.Web.Api.Models.EntityModels
{
    public class GetScheduleInfoModel
    {
        public int ScheduleId { get; set; }
        public int ParentId { get; set; }

    }
}