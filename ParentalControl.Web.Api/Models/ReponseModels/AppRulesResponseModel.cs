using ParentalControl.Web.Api.Models.EntityModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ParentalControl.Web.Api.Models.ReponseModels
{
    public class AppRulesResponseModel
    {
        public List<AppRulesModel> appRulesModelList { get; set; }
        public string MessageError { get; set; }
        public bool IsSuccess { get; set; }
    }
}