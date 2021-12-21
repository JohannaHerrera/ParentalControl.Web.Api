using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ParentalControl.Web.Api.Models.EntityModels
{
    public class GetDeviceInfoModel
    {
        public int ParentId { get; set; }
        public string DevicePhoneCode { get; set; }
    }
}