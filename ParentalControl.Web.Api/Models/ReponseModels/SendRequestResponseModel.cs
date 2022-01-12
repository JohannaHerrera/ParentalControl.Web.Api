using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ParentalControl.Web.Api.Models.ReponseModels
{
    public class SendRequestResponseModel
    {
        public string MessageError { get; set; }
        public bool IsSend { get; set; }
    }
}