using ParentalControl.Web.Api.Models.EntityModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ParentalControl.Web.Api.Models.ReponseModels
{
    public class DevicePhoneResponseModel
    {
        public int DevicePhoneId { get; set; }
        public string DevicePhoneName { get; set; }
        public string DevicePhoneCode { get; set; }
        public int? InfantAccountId { get; set; }
        public List<DevicePhoneUseModel> devicePhoneUseList { get; set; }
        public List<InfantAccountModel> infantAccountList { get; set; }
        public List<ScheduleModel> scheduleList { get; set; }
        public string MessageError { get; set; }
        public bool IsSuccess { get; set; }
    }
}