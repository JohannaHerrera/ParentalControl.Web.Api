using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ParentalControl.Web.Api.Models.ReponseModels
{
    public class RegisterResponseModel
    {
        public string MessageError { get; set; }

        public bool Registered { get; set; }
    }
}