using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ParentalControl.Web.Api.Models.EntityModels
{
    public class GetTimeByDayModel
    {
        public int DevicePhoneId { get; set; }
        public string DevicePhoneUseDay { get; set; }
    }
}