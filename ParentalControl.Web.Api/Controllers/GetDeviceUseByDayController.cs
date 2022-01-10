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
    [RoutePrefix("api/GetDeviceUseByDay")]
    public class GetDeviceUseByDayController : ApiController
    {
        [HttpPost]
        public DeviceUseByDayResponseModel Post([FromBody] GetTimeByDayModel getTimeByDayModel)
        {
            DeviceUseByDayResponseModel deviceUseByDayResponseModel = new DeviceUseByDayResponseModel();
            List<string> Hours = new List<string>();
            List<string> Minutes = new List<string>();

            try
            {
                if(getTimeByDayModel.DevicePhoneId > 0 && !string.IsNullOrEmpty(getTimeByDayModel.DevicePhoneUseDay))
                {
                    using (var db = new ParentalControlDBEntities())
                    {
                        var useDay = (from deviceUse in db.DevicePhoneUse
                                      where deviceUse.DevicePhoneId == getTimeByDayModel.DevicePhoneId
                                      && deviceUse.DevicePhoneUseDay.ToLower().Equals(getTimeByDayModel.DevicePhoneUseDay.ToLower())
                                      && deviceUse.ScheduleId != null
                                      select deviceUse).FirstOrDefault();

                        if(useDay != null)
                        {
                            var schedule = (from scheduleInfo in db.Schedule
                                            where scheduleInfo.ScheduleId == useDay.ScheduleId
                                            select scheduleInfo).FirstOrDefault();

                            int hour = Convert.ToInt32(schedule.ScheduleEndTime.ToString("HH"));
                            int minutes = Convert.ToInt32(schedule.ScheduleEndTime.ToString("mm"));
                            int hoursAvailable = 0;
                            int minutesAvailable = 0;

                            hoursAvailable = 24 - (hour + 1);
                            minutesAvailable = 60 - minutes;

                            // Horas
                            int cont = 1;
                            while (cont <= hoursAvailable)
                            {
                                Hours.Add(cont.ToString());
                                cont++;
                            }

                            // Minutos
                            cont = 10;
                            while (cont <= minutesAvailable)
                            {
                                if (cont != 60)
                                {
                                    Minutes.Add(cont.ToString());
                                }
                                cont = cont + 10;
                            }

                            deviceUseByDayResponseModel.Hours = Hours;
                            deviceUseByDayResponseModel.Minutes = Minutes;
                        }
                    }
                }
                else
                {
                    deviceUseByDayResponseModel.MessageError = "Error al obtener información sobre el tiempo de uso.";
                }
            }
            catch (Exception ex)
            {
                deviceUseByDayResponseModel.MessageError = "Ocurrió un error inesperado. Inténtelo de nuevo.";
            } 

            return deviceUseByDayResponseModel;
        }
    }
}