using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ParentalControl.Web.Api.Models.EntityModels
{
    public class AppRulesModel
    {
        public int AppId { get; set; }
        public int? DevicePhoneId { get; set; }
        public int InfantAccountId { get; set; }
        public int? ScheduleId { get; set; }
        public string AppName { get; set; }
        public bool? AppAccessPermission { get; set; }
    }
}