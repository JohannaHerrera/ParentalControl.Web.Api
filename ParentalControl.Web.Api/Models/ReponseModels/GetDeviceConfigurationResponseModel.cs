using ParentalControl.Web.Api.Models.EntityModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ParentalControl.Web.Api.Models.ReponseModels
{
    public class GetDeviceConfigurationResponseModel
    {
        public bool HaveRules { get; set; }
        public int DevicePhoneId { get; set; }
        public int? InfantAccountId { get; set; }
        public int ParentId { get; set; }
        public List<ScheduleModel> scheduleModelList { get; set; }
        public List<WebConfigurationRulesModel> WebsLocked { get; set; }
        public List<AppRulesModel> AppsLocked { get; set; }
        public List<DevicePhoneUseModel> DeviceConfig { get; set; }
        public string MessageError { get; set; }
    }
}