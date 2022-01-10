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
using ParentalControl.Web.Api.Business;
using System.Data.Entity;

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

        [HttpPut]
        public SendRequestResponseModel Put([FromBody] SendRequestModel sendRequestModel)
        {
            SendRequestResponseModel sendRequestResponseModel = new SendRequestResponseModel();
            sendRequestResponseModel.IsSend = false;

            try
            {
                using(var db = new ParentalControlDBEntities())
                {
                    if(sendRequestModel.InfantAccountId > 0)
                    {
                        var infant = (from infantInfo in db.InfantAccount
                                      where infantInfo.InfantAccountId == sendRequestModel.InfantAccountId
                                      select infantInfo).FirstOrDefault();

                        var parent = (from parentInfo in db.Parent
                                      join device in db.DevicePhone
                                      on parentInfo.ParentId
                                      equals device.ParentId
                                      where parentInfo.ParentId == sendRequestModel.ParentId
                                      select parentInfo).FirstOrDefault();

                        if(infant != null && parent != null)
                        {
                            if (sendRequestModel.RequestType > 0)
                            {
                                if (sendRequestModel.RequestType == 1)
                                {
                                    if (string.IsNullOrEmpty(sendRequestModel.Object))
                                    {
                                        sendRequestResponseModel.MessageError = "Especifique la categoría web.";
                                    }
                                    else
                                    {
                                        var request = (from requests in db.Request
                                                       where requests.InfantAccountId == sendRequestModel.InfantAccountId
                                                       && requests.RequestObject.ToLower().Equals(sendRequestModel.Object.ToLower())
                                                       && requests.RequestState == 0
                                                       && requests.RequestTypeId == 1
                                                       select requests).FirstOrDefault();

                                        if (request != null)
                                        {
                                            sendRequestResponseModel.MessageError = "Ya enviaste una petición para este recurso. " +
                                                                                    "Espera la respuesta de tus padres.";
                                        }
                                        else
                                        {
                                            SendEmail sendEmail = new SendEmail();

                                            Request requestModel = new Request();
                                            requestModel.RequestTypeId = 1;
                                            requestModel.InfantAccountId = sendRequestModel.InfantAccountId;
                                            requestModel.RequestObject = sendRequestModel.Object;
                                            requestModel.RequestState = 0;
                                            requestModel.RequestCreationDate = DateTime.Now;
                                            requestModel.ParentId = sendRequestModel.ParentId;
                                            db.Request.Add(requestModel);
                                            db.SaveChanges();

                                            string body = $"<p>¡Hola! <br> <br> Queremos informarte que <b>{infant.InfantName}</b> " +
                                            $"está solicitando que le habilites la categoría web " +
                                            $"<b>{sendRequestModel.Object}</b>. <br>" +
                                            $"Para aprobar o desaprobar esta petición ingresa a nuestro " +
                                            $"sistema y dirígete a la sección de <b>Notificaciones</b>.<p>";

                                            sendEmail.SendEmailRequest(parent.ParentEmail, body);

                                            sendRequestResponseModel.IsSend = true;
                                            sendRequestResponseModel.MessageError = string.Empty;
                                        }
                                    }
                                }
                                else if (sendRequestModel.RequestType == 2)
                                {
                                    if (string.IsNullOrEmpty(sendRequestModel.Object))
                                    {
                                        sendRequestResponseModel.MessageError = "Especifique la aplicación.";
                                    }
                                    else
                                    {
                                        var request = (from requests in db.Request
                                                       where requests.InfantAccountId == sendRequestModel.InfantAccountId
                                                       && requests.RequestObject.ToLower().Equals(sendRequestModel.Object.ToLower())
                                                       && requests.RequestState == 0
                                                       && requests.RequestTypeId == 2
                                                       select requests).FirstOrDefault();

                                        if (request != null)
                                        {
                                            sendRequestResponseModel.MessageError = "Ya enviaste una petición para este recurso. " +
                                                                                    "Espera la respuesta de tus padres.";
                                        }
                                        else
                                        {
                                            SendEmail sendEmail = new SendEmail();

                                            Request requestModel = new Request();
                                            requestModel.RequestTypeId = 2;
                                            requestModel.InfantAccountId = sendRequestModel.InfantAccountId;
                                            requestModel.RequestObject = sendRequestModel.Object;
                                            requestModel.RequestState = 0;
                                            requestModel.RequestCreationDate = DateTime.Now;
                                            requestModel.ParentId = sendRequestModel.ParentId;
                                            db.Request.Add(requestModel);
                                            db.SaveChanges();

                                            string body = $"<p>¡Hola! <br> <br> Queremos informarte que <b>{infant.InfantName}</b> " +
                                                          $"está solicitando que le habilites la aplicación " +
                                                          $"<b>{sendRequestModel.Object}</b>. <br>" +
                                                          $"Para aprobar o desaprobar esta petición ingresa a nuestro " +
                                                          $"sistema y dirígete a la sección de <b>Notificaciones</b>.<p>";

                                            sendEmail.SendEmailRequest(parent.ParentEmail, body);
                                            sendRequestResponseModel.IsSend = true;
                                            sendRequestResponseModel.MessageError = string.Empty;
                                        }
                                    }
                                }
                                else if (sendRequestModel.RequestType == 3)
                                {
                                    if (string.IsNullOrEmpty(sendRequestModel.Hours) && string.IsNullOrEmpty(sendRequestModel.Minutes))
                                    {
                                        sendRequestResponseModel.MessageError = "Especifique el tiempo que desea extender el uso del dispositivo.";                                       
                                    }
                                    else
                                    {
                                        var request = (from requests in db.Request
                                                       where requests.InfantAccountId == sendRequestModel.InfantAccountId
                                                       && requests.RequestTime != null
                                                       && requests.RequestState == 0
                                                       && requests.RequestTypeId == 3
                                                       && DbFunctions.TruncateTime(requests.RequestCreationDate) == DbFunctions.TruncateTime(DateTime.Now)
                                                       select requests).FirstOrDefault();

                                        if (request != null)
                                        {
                                            sendRequestResponseModel.MessageError = "Ya enviaste una petición para extender el tiempo de uso para hoy." +
                                                                                    "Espera la respuesta de tus padres.";
                                        }
                                        else
                                        {
                                            string time = string.Empty;
                                            string extraTime = string.Empty;

                                            if (!string.IsNullOrEmpty(sendRequestModel.Hours))
                                            {
                                                if (!string.IsNullOrEmpty(sendRequestModel.Minutes))
                                                {
                                                    if (Convert.ToInt32(sendRequestModel.Hours) == 1)
                                                    {
                                                        time = $"{sendRequestModel.Hours} hora y {sendRequestModel.Minutes} minutos";
                                                    }
                                                    else
                                                    {
                                                        time = $"{sendRequestModel.Hours} horas y {sendRequestModel.Minutes} minutos";
                                                    }

                                                    extraTime = $"{sendRequestModel.Hours}.{sendRequestModel.Minutes}";
                                                }
                                                else
                                                {
                                                    if (Convert.ToInt32(sendRequestModel.Hours) == 1)
                                                    {
                                                        time = $"{sendRequestModel.Hours} hora";
                                                    }
                                                    else
                                                    {
                                                        time = $"{sendRequestModel.Hours} horas";
                                                    }

                                                    extraTime = $"{sendRequestModel.Hours}.0";
                                                }
                                            }
                                            else
                                            {
                                                time = $"{sendRequestModel.Minutes} minutos";
                                                extraTime = $"0.{sendRequestModel.Minutes}";
                                            }

                                            string body = $"<p>¡Hola! <br> <br> Queremos informarte que <b>{infant.InfantName}</b> " +
                                                          $"está solicitando que le amplíes el tiempo de uso del dispositivo " +
                                                          $"por: <b>{time}</b>. <br>" +
                                                          $"Para aprobar o desaprobar esta petición ingresa a nuestro " +
                                                          $"sistema y dirígete a la sección de <b>Notificaciones</b>.<p>";

                                            SendEmail sendEmail = new SendEmail();

                                            Request requestModel = new Request();
                                            requestModel.RequestTypeId = 3;
                                            requestModel.InfantAccountId = sendRequestModel.InfantAccountId;
                                            requestModel.RequestTime = Convert.ToDecimal(extraTime);
                                            requestModel.RequestState = 0;
                                            requestModel.RequestCreationDate = DateTime.Now;
                                            requestModel.ParentId = sendRequestModel.ParentId;
                                            db.Request.Add(requestModel);
                                            db.SaveChanges();

                                            sendEmail.SendEmailRequest(parent.ParentEmail, body);
                                            sendRequestResponseModel.IsSend = true;
                                            sendRequestResponseModel.MessageError = string.Empty;
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            sendRequestResponseModel.MessageError = "Ha ocurrido un error inesperado. Inténtelo de nuevo.";
                        }
                    }
                    else
                    {
                        sendRequestResponseModel.MessageError = "Ha ocurrido un error inesperado. Inténtelo de nuevo.";
                    }
                }               
            }
            catch(Exception ex)
            {                
                sendRequestResponseModel.MessageError = "Ha ocurrido un error inesperado. Inténtelo de nuevo.";
            }

            return sendRequestResponseModel;
        }
    }
}