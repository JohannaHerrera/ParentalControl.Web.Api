using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ParentalControl.Web.Api.Models.ReponseModels
{
    public class GetMyProfileResponseModel
    {
        public string ParentUsername { get; set; }
        public string ParentEmail { get; set; }
        public string ParentPassword { get; set; }
        public string MessageError { get; set; }
    }
}