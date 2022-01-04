using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ParentalControl.Web.Api.Models.EntityModels
{
    public class DeviceUseRulesModel
    {
        public int DeviceUseId { get; set; }
        public string DeviceUseDay { get; set; }
        public System.DateTime DeviceUseCreationDate { get; set; }
        public int InfantAccountId { get; set; }
        public Nullable<int> ScheduleId { get; set; }
    }
}