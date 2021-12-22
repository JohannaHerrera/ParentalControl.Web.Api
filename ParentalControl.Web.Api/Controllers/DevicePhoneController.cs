using ParentalControl.Web.Api.Constants;
using ParentalControl.Web.Api.Data;
using ParentalControl.Web.Api.Models.EntityModels;
using ParentalControl.Web.Api.Models.ReponseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
namespace ParentalControl.Web.Api.Controllers
{
    [AllowAnonymous]
    [RoutePrefix("api/DevicePhone")]
    public class DevicePhoneController : ApiController
    {
        [HttpPost]
        public DevicePhoneResponseModel Post([FromBody] GetDeviceInfoModel getDeviceInfoModel)
        {
            DevicePhoneResponseModel devicePhoneResponseModel = new DevicePhoneResponseModel();
            List<DevicePhoneUseModel> devicePhoneUseModelList = new List<DevicePhoneUseModel>();
            List<InfantAccountModel> infantAccountModelList = new List<InfantAccountModel>();
            List<ScheduleModel> scheduleModelList = new List<ScheduleModel>();

            try
            {
                using(var db = new ParentalControlDBEntities())
                {
                    // GetDeviceInfo
                    if(getDeviceInfoModel.Action == 1)
                    {
                        var deviceInfo = (from device in db.DevicePhone
                                          where device.ParentId == getDeviceInfoModel.ParentId
                                          && device.DevicePhoneCode == getDeviceInfoModel.DevicePhoneCode
                                          select device).FirstOrDefault();

                        if (deviceInfo != null)
                        {
                            var deviceUseInfo = (from deviceUse in db.DevicePhoneUse
                                                 where deviceUse.DevicePhoneId == deviceInfo.DevicePhoneId
                                                 select deviceUse).ToList();

                            if (deviceUseInfo.Count() > 0)
                            {
                                foreach (var use in deviceUseInfo)
                                {
                                    DevicePhoneUseModel devicePhoneUseModel = new DevicePhoneUseModel();
                                    devicePhoneUseModel.DevicePhoneUseId = use.DevicePhoneUseId;
                                    devicePhoneUseModel.DevicePhoneUseDay = use.DevicePhoneUseDay;
                                    devicePhoneUseModel.ScheduleId = use.ScheduleId;
                                    devicePhoneUseModelList.Add(devicePhoneUseModel);
                                }

                                var infantList = (from infant in db.InfantAccount
                                                  where infant.ParentId == getDeviceInfoModel.ParentId
                                                  select infant).ToList();

                                // Agrego primero "No Protegido"
                                InfantAccountModel noProtected = new InfantAccountModel();
                                noProtected.InfantAccountId = 0;
                                noProtected.InfantName = "No Protegido";
                                infantAccountModelList.Add(noProtected);

                                // Agrego las demás cuentas infantiles
                                foreach (var infant in infantList)
                                {
                                    InfantAccountModel infantAccountModel = new InfantAccountModel();
                                    infantAccountModel.InfantAccountId = infant.InfantAccountId;
                                    infantAccountModel.InfantName = infant.InfantName;
                                    infantAccountModelList.Add(infantAccountModel);
                                }

                                var schedules = (from scheduleInfo in db.Schedule
                                                 where scheduleInfo.ParentId == getDeviceInfoModel.ParentId
                                                 select scheduleInfo).ToList();

                                // Agrego primero "Ninguno"
                                ScheduleModel none = new ScheduleModel();
                                none.ScheduleId = 0;
                                none.ScheduleTime = "Ninguno";
                                scheduleModelList.Add(none);

                                // Agrego los demás horarios
                                foreach (var schdule in schedules)
                                {
                                    ScheduleModel scheduleModel = new ScheduleModel();
                                    scheduleModel.ScheduleId = schdule.ScheduleId;
                                    scheduleModel.ScheduleTime = $"{schdule.ScheduleStartTime.ToString("HH:mm")} - " +
                                        $"{schdule.ScheduleEndTime.ToString("HH:mm")}";
                                    scheduleModelList.Add(scheduleModel);
                                }


                                devicePhoneResponseModel.DevicePhoneId = deviceInfo.DevicePhoneId;
                                devicePhoneResponseModel.DevicePhoneName = deviceInfo.DevicePhoneName;
                                devicePhoneResponseModel.DevicePhoneCode = deviceInfo.DevicePhoneCode;
                                devicePhoneResponseModel.InfantAccountId = deviceInfo.InfantAccountId;
                                devicePhoneResponseModel.devicePhoneUseList = devicePhoneUseModelList;
                                devicePhoneResponseModel.infantAccountList = infantAccountModelList;
                                devicePhoneResponseModel.scheduleList = scheduleModelList;
                            }
                            else
                            {
                                devicePhoneResponseModel.MessageError = "No se encontró información de los horarios de uso del dispositivo.";
                            }
                        }
                        else
                        {
                            devicePhoneResponseModel.MessageError = "No se encontró información del dispositivo.";
                        }
                    }
                    else if(getDeviceInfoModel.Action == 2)
                    {
                        devicePhoneResponseModel.IsSuccess = false;

                        // DeleteDevice
                        var devicePhone = (from device in db.DevicePhone
                                           where device.DevicePhoneCode == getDeviceInfoModel.DevicePhoneCode
                                           && device.ParentId == getDeviceInfoModel.ParentId
                                           select device).FirstOrDefault();

                        if (devicePhone != null)
                        {
                            var deviceUseSchedules = db.DevicePhoneUse.Where(x => x.DevicePhoneId == devicePhone.DevicePhoneId);
                            var apps = db.App.Where(x => x.DevicePhoneId == devicePhone.DevicePhoneId);
                            var appsDevice = db.AppDevice.Where(x => x.DevicePhoneId == devicePhone.DevicePhoneId);

                            db.DevicePhoneUse.RemoveRange(deviceUseSchedules);
                            db.App.RemoveRange(apps);
                            db.AppDevice.RemoveRange(appsDevice);
                            db.DevicePhone.Remove(devicePhone);
                            db.SaveChanges();
                            devicePhoneResponseModel.IsSuccess = true;
                        }
                    }
                    else
                    {
                        devicePhoneResponseModel.MessageError = "Error. No se especificó la acción a realizar.";
                    }
                }
            }
            catch (Exception ex)
            {
                devicePhoneResponseModel.MessageError = "Ocurrió un error inesperado. Inténtelo de nuevo.";
            }

            return devicePhoneResponseModel;
        }

        [HttpPut]
        public bool Put([FromBody] UpdateDevicePhoneInfoModel updateDevicePhoneInfo)
        {
            bool result = false;

            try
            {
                if(updateDevicePhoneInfo.ParentId > 0 && updateDevicePhoneInfo.DevicePhoneCode != null)
                {
                    using (var db = new ParentalControlDBEntities())
                    {
                        var deviceInfo = (from device in db.DevicePhone
                                          where device.ParentId == updateDevicePhoneInfo.ParentId
                                          && device.DevicePhoneCode.ToLower().Equals(updateDevicePhoneInfo.DevicePhoneCode.ToLower())
                                          select device).FirstOrDefault();

                        if(deviceInfo != null)
                        {
                            // Actualizo el nombre del dispositivo
                            deviceInfo.DevicePhoneName = updateDevicePhoneInfo.DevicePhoneName;
                            db.SaveChanges();

                            // Actualizo el Id de la cuenta infantil
                            if(updateDevicePhoneInfo.InfantAccountId == 0)
                            {
                                // Se cambió a No Protegido
                                if(deviceInfo.InfantAccountId != null)
                                {
                                    // Elimino las aplicaciones que tenía asignado el infante anteriormente
                                    var appsRemove = (from apps in db.App
                                                      where apps.InfantAccountId == deviceInfo.InfantAccountId
                                                      && apps.DevicePhoneId == deviceInfo.DevicePhoneId
                                                      select apps).ToList();

                                    db.App.RemoveRange(appsRemove);

                                    // Actualizo el Id del Infante
                                    deviceInfo.InfantAccountId = null;
                                    db.SaveChanges();
                                }
                            }
                            else
                            {
                                AppConstants appConstants = new AppConstants();

                                // Vrifico si anteriormente estaba como No Protegido
                                if (deviceInfo.InfantAccountId == null)
                                {
                                    // Actualizo el Id del Infante
                                    deviceInfo.InfantAccountId = updateDevicePhoneInfo.InfantAccountId;
                                    db.SaveChanges();

                                    // Registro las aplicaciones 
                                    var appsDevice = (from apps in db.AppDevice
                                                      where apps.DevicePhoneId == deviceInfo.DevicePhoneId
                                                      select apps).ToList();

                                    foreach(var app in appsDevice)
                                    {
                                        App appRegister = new App();
                                        appRegister.InfantAccountId = updateDevicePhoneInfo.InfantAccountId;
                                        appRegister.DevicePhoneId = app.DevicePhoneId;
                                        appRegister.AppAccessPermission = appConstants.Access;
                                        appRegister.AppCreationDate = DateTime.Now;
                                        appRegister.AppName = app.AppDeviceName;
                                        appRegister.AppStatus = appConstants.Access;
                                        db.App.Add(appRegister);
                                        db.SaveChanges();
                                    }
                                }
                                else
                                {
                                    // Sino, tenía anteriormente otra cuenta infantil registrada

                                    // Elimino las aplicaciones a la cuenta infantil anterior
                                    var appsRemove = (from apps in db.App
                                                      where apps.InfantAccountId == deviceInfo.InfantAccountId
                                                      && apps.DevicePhoneId == deviceInfo.DevicePhoneId
                                                      select apps).ToList();

                                    db.App.RemoveRange(appsRemove);
                                    db.SaveChanges();

                                    // Registro las aplicaciones a la nueva cuenta
                                    var appsDevice = (from apps in db.AppDevice
                                                      where apps.DevicePhoneId == deviceInfo.DevicePhoneId
                                                      select apps).ToList();

                                    foreach (var app in appsDevice)
                                    {
                                        App appRegister = new App();
                                        appRegister.InfantAccountId = updateDevicePhoneInfo.InfantAccountId;
                                        appRegister.DevicePhoneId = app.DevicePhoneId;
                                        appRegister.AppAccessPermission = appConstants.Access;
                                        appRegister.AppCreationDate = DateTime.Now;
                                        appRegister.AppName = app.AppDeviceName;
                                        appRegister.AppStatus = appConstants.Access;
                                        db.App.Add(appRegister);
                                        db.SaveChanges();
                                    }

                                    // Actualizo el Id del Infante
                                    deviceInfo.InfantAccountId = updateDevicePhoneInfo.InfantAccountId;
                                    db.SaveChanges();
                                }
                            }

                            // Actualizo los horarios de uso
                            foreach(var use in updateDevicePhoneInfo.devicePhoneUseModelList)
                            {
                                var useDay = (from deviceUse in db.DevicePhoneUse
                                              where deviceUse.DevicePhoneUseDay == use.DevicePhoneUseDay
                                              && deviceUse.DevicePhoneId == deviceInfo.DevicePhoneId
                                              select deviceUse).FirstOrDefault();

                                if(useDay != null)
                                {
                                    if (use.ScheduleId == 0 || use.ScheduleId == null)
                                    {
                                        useDay.ScheduleId = null;
                                        db.SaveChanges();
                                    }
                                    else
                                    {
                                        useDay.ScheduleId = use.ScheduleId;
                                        db.SaveChanges();
                                    }
                                }                                
                            }

                            result = true;
                        }
                    }
                }
                else
                {
                    result = false;
                }
            }
            catch(Exception ex)
            {
                result = false;
            }

            return result;
        }
    }
}