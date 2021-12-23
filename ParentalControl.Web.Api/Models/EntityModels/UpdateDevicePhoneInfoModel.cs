using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ParentalControl.Web.Api.Models.EntityModels
{
    public class UpdateDevicePhoneInfoModel
    {
        public string DevicePhoneCode { get; set; }
        public string DevicePhoneName { get; set; }
        public List<DevicePhoneUseModel> devicePhoneUseModelList { get; set; }
        public int InfantAccountId { get; set; }
        public int ParentId { get; set; }
    }
}