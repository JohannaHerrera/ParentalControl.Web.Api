using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ParentalControl.Web.Api.Models.EntityModels
{
    public class RegisterModel
    {
        public string ParentUsername { get; set; }
        public string ParentEmail { get; set; }
        public string ParentPassword { get; set; }
    }
}