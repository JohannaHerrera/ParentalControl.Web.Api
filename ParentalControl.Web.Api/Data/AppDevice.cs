//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ParentalControl.Web.Api.Data
{
    using System;
    using System.Collections.Generic;
    
    public partial class AppDevice
    {
        public int AppDeviceId { get; set; }
        public Nullable<int> DevicePhoneId { get; set; }
        public Nullable<int> DevicePCId { get; set; }
        public string AppDeviceName { get; set; }
        public System.DateTime AppDeviceCreationDate { get; set; }
    
        public virtual DevicePC DevicePC { get; set; }
        public virtual DevicePhone DevicePhone { get; set; }
    }
}
