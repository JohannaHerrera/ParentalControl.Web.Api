using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ParentalControl.Web.Api.Models.EntityModels
{
    public class DevicePhoneUseModel
    {
        public int DevicePhoneUseId { get; set; }
        public string DevicePhoneUseDay { get; set; }
        public int? ScheduleId { get; set; }
    }
}