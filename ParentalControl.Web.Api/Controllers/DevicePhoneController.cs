using System;
using System.Collections.Generic;
using System.Web.Http;
using System.Linq;
using ParentalControl.Web.Api.Models.ReponseModels;
using ParentalControl.Web.Api.Models.EntityModels;
using ParentalControl.Web.Api.Data;
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
                    var deviceInfo = (from device in db.DevicePhone
                                      where device.ParentId == getDeviceInfoModel.ParentId
                                      && device.DevicePhoneCode == getDeviceInfoModel.DevicePhoneCode
                                      select device).FirstOrDefault();

                    if(deviceInfo != null)
                    {
                        var deviceUseInfo = (from deviceUse in db.DevicePhoneUse
                                             where deviceUse.DevicePhoneId == deviceInfo.DevicePhoneId
                                             select deviceUse).ToList();

                        if(deviceUseInfo.Count() > 0)
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
            }
            catch (Exception ex)
            {
                devicePhoneResponseModel.MessageError = "Ocurrió un error inesperado. Inténtelo de nuevo.";
            }

            return devicePhoneResponseModel;
        }
    }
}