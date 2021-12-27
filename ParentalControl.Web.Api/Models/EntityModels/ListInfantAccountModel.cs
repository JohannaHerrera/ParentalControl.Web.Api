using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ParentalControl.Web.Api.Models.EntityModels
{
    public class ListInfantAccountModel
    {
        public int InfantAccountId { get; set; }
        public string InfantName { get; set; }
        public string InfantGender { get; set; }
        public DateTime InfantCreationDate { get; set; }
        public int? ParentId { get; set; }
    }
}