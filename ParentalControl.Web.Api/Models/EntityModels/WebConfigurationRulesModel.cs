using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ParentalControl.Web.Api.Models.EntityModels
{
    public class WebConfigurationRulesModel
    {
        public int WebConfigurationId { get; set; }
        public bool? WebConfigurationAccess { get; set; }
        public int CategoryId { get; set; }
        public int InfantAccountId { get; set; }
    }
}