using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ParentalControl.Web.Api.Models.EntityModels
{
    public class RequestModel
    {
        public int RequestId { get; set; }
        public int RequestTypeId { get; set; }
        public int InfantAccountId { get; set; }
        public string RequestObject { get; set; }
        public decimal? RequestTime { get; set; }
        public int RequestState { get; set; }
        public string InfantName { get; set; }
        public string InfantGender { get; set; }
        public string RequestDescription { get; set; }
    }
}