using ParentalControl.Web.Api.Data;
using ParentalControl.Web.Api.Models.EntityModels;
using ParentalControl.Web.Api.Models.ReponseModels;
using System;
using System.Net;
using System.Threading;
using System.Web.Http;
using System.Linq;
using System.Collections.Generic;
using ParentalControl.Web.Api.Constants;

namespace ParentalControl.Web.Api.Controllers
{
    [AllowAnonymous]
    [RoutePrefix("api/GetDeviceConfiguration")]
    public class GetDeviceConfigurationController : ApiController
    {
        [HttpPost]
        public GetDeviceConfigurationResponseModel Post([FromBody] string devicePhoneCode)
        {
            GetDeviceConfigurationResponseModel getDeviceConfigurationResponseModel = new GetDeviceConfigurationResponseModel();
            List<WebConfigurationRulesModel> WebsLocked = new List<WebConfigurationRulesModel>();
            List<AppRulesModel> appLockedList = new List<AppRulesModel>();
            List<DevicePhoneUseModel> DeviceConfig = new List<DevicePhoneUseModel>();
            List<ScheduleModel> scheduleModelList = new List<ScheduleModel>();
            AppConstants appConstants = new AppConstants();

            getDeviceConfigurationResponseModel.HaveRules = false;

            try
            {
                if (!string.IsNullOrEmpty(devicePhoneCode))
                {
                    using (var db = new ParentalControlDBEntities())
                    {
                        var devicePhone = (from device in db.DevicePhone
                                           where device.DevicePhoneCode.ToLower().Equals(devicePhoneCode.ToLower())
                                           select device).FirstOrDefault();

                        if (devicePhone != null)
                        {
                            if(devicePhone.InfantAccountId != null && devicePhone.InfantAccountId > 0)
                            {
                                var webs = (from webConfig in db.WebConfiguration
                                            where webConfig.InfantAccountId == devicePhone.InfantAccountId
                                            select webConfig).ToList();

                                if(webs != null && webs.Count() > 0)
                                {
                                    foreach(var web in webs)
                                    {
                                        if(web.WebConfigurationAccess == appConstants.NoAccess)
                                        {
                                            WebConfigurationRulesModel webConfigurationRulesModel = new WebConfigurationRulesModel();

                                            if (web.CategoryId == 1)
                                            {
                                                webConfigurationRulesModel.CategoryName = "Drogras";
                                            }
                                            else if (web.CategoryId == 2)
                                            {
                                                webConfigurationRulesModel.CategoryName = "Pornografía";
                                            }
                                            else if (web.CategoryId == 3)
                                            {
                                                webConfigurationRulesModel.CategoryName = "Videojuegos";
                                            }
                                            else
                                            {
                                                webConfigurationRulesModel.CategoryName = "Violencia";
                                            }

                                            WebsLocked.Add(webConfigurationRulesModel);
                                        }  
                                    } 
                                }

                                var apps = (from app in db.App
                                            where app.DevicePhoneId == devicePhone.DevicePhoneId
                                            select app).ToList();

                                if(apps != null && apps.Count() > 0)
                                {
                                    foreach (var app in apps)
                                    {
                                        if (app.AppAccessPermission == appConstants.NoAccess || app.ScheduleId != null)
                                        {
                                            AppRulesModel appRulesModel = new AppRulesModel();
                                            appRulesModel.AppId = app.AppId;
                                            appRulesModel.ScheduleId = app.ScheduleId;
                                            appRulesModel.AppName = app.AppName;
                                            appLockedList.Add(appRulesModel);
                                        }
                                    }
                                }

                                var deviceUses = (from deviceUse in db.DevicePhoneUse
                                                  where deviceUse.DevicePhoneId == devicePhone.DevicePhoneId
                                                  select deviceUse).ToList();

                                if(deviceUses != null && deviceUses.Count() > 0)
                                {
                                    foreach(var use in deviceUses)
                                    {
                                        if (use.ScheduleId != null && use.ScheduleId > 0)
                                        {
                                            DevicePhoneUseModel devicePhoneUseModel = new DevicePhoneUseModel();
                                            devicePhoneUseModel.DevicePhoneUseDay = use.DevicePhoneUseDay;
                                            devicePhoneUseModel.ScheduleId = use.ScheduleId;
                                            DeviceConfig.Add(devicePhoneUseModel);
                                        }
                                    }
                                }

                                var schedules = (from schedule in db.Schedule
                                                 where schedule.ParentId == devicePhone.ParentId
                                                 select schedule).ToList();

                                foreach (var schdule in schedules)
                                {
                                    ScheduleModel scheduleModel = new ScheduleModel();
                                    scheduleModel.ScheduleId = schdule.ScheduleId;
                                    scheduleModel.ScheduleTime = $"{schdule.ScheduleStartTime.ToString("HH:mm")} - " +
                                                                 $"{schdule.ScheduleEndTime.ToString("HH:mm")}";
                                    scheduleModelList.Add(scheduleModel);
                                }

                                getDeviceConfigurationResponseModel.HaveRules = true;
                                getDeviceConfigurationResponseModel.DevicePhoneId = devicePhone.DevicePhoneId;
                                getDeviceConfigurationResponseModel.ParentId = devicePhone.ParentId;
                                getDeviceConfigurationResponseModel.InfantAccountId = devicePhone.InfantAccountId;
                                getDeviceConfigurationResponseModel.WebsLocked = WebsLocked;
                                getDeviceConfigurationResponseModel.AppsLocked = appLockedList;
                                getDeviceConfigurationResponseModel.DeviceConfig = DeviceConfig;
                                getDeviceConfigurationResponseModel.scheduleModelList = scheduleModelList;
                            } 
                        }
                        else
                        {
                            getDeviceConfigurationResponseModel.MessageError = "No se pudo encontrar información sobre la configuración del dispositivo.";
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                getDeviceConfigurationResponseModel.MessageError = "Ocurrió un error inesperado. Inténtelo nuevamente";
            }

            return getDeviceConfigurationResponseModel;
        }
    }
}