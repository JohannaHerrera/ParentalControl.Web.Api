using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ParentalControl.Web.Api.Models.EntityModels
{
    public class UpdateRequestModel
    {
        public int RequestId { get; set; }
        public int RequestAction { get; set; } // 1: Aprobar, 2: Desaprobar
    }
}