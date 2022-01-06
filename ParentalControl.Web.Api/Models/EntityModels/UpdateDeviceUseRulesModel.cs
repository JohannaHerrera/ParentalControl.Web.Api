﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ParentalControl.Web.Api.Models.EntityModels
{
    public class UpdateDeviceUseRulesModel
    {
        public string DeviceUseDay { get; set; }
        public int InfantAccountId { get; set; }
        public Nullable<int> ScheduleId { get; set; }
    }
}