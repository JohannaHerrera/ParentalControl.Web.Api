using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ParentalControl.Web.Api.Models.ReponseModels
{
    public class DeviceUseByDayResponseModel
    {
        public List<string> Hours { get; set; }
        public List<string> Minutes { get; set; }
        public string MessageError { get; set; }
    }
}