using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ParentalControl.Web.Api.Models.ReponseModels
{
    public class LoginResponseModel
    {
        public int ParentId { get; set; }
        public string ParentUsername { get; set; }
        public string ParentEmail { get; set; }
        public string MessageError { get; set; }
    }
}