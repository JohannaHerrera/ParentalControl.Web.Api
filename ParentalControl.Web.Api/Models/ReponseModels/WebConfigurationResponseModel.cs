using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ParentalControl.Web.Api.Models.EntityModels;

namespace ParentalControl.Web.Api.Models.ReponseModels
{
    public class WebConfigurationResponseModel
    {
        public string MessageError { get; set; }
        public bool IsSuccess { get; set; }
    }
}