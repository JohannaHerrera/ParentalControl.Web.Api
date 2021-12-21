using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ParentalControl.Web.Api.Models.EntityModels
{
    public class DevicePhoneModel
    {
        public string DevicePhoneCode { get; set; }
        public string DevicePhoneName { get; set; }
        public List<AppDeviceModel> AppsInstalled { get; set; }
    }
}