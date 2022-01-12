using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ParentalControl.Web.Api.Models.EntityModels
{
    public class GetAppConfigurationModel
    {
        public int InfantAccountId { get; set; }
        public string DevicePhoneCode { get; set; }
    }
}