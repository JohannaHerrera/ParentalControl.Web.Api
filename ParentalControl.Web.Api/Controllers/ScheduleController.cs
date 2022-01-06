using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using ParentalControl.Web.Api.Models.ReponseModels;
using ParentalControl.Web.Api.Models.EntityModels;
using ParentalControl.Web.Api.Data;
using ParentalControl.Web.Api.Constants;
using System.Data.Entity; 

namespace ParentalControl.Web.Api.Controllers
{
    [AllowAnonymous]
    [RoutePrefix("api/Schedule")]
    public class ScheduleController : ApiController
    {
        [HttpPost]
        public List<ScheduleResponseModel> Post([FromBody] GetScheduleInfoModel getScheduleInfoModel)
        {
            List<ScheduleResponseModel> scheduleResponseModelList = new List<ScheduleResponseModel>();
            ScheduleResponseModel scheduleResponseModel = new ScheduleResponseModel();
            try
            {
                using (var db = new ParentalControlDBEntities())
                {
                    var scheduleInfoList = (from schedule in db.Schedule
                                            where schedule.ParentId == getScheduleInfoModel.ParentId
                                            select schedule).ToList();
                    if (scheduleInfoList.Count > 0)
                    {
                        foreach (var item in scheduleInfoList)
                        {
                            ScheduleResponseModel scheduleResponseModel2 = new ScheduleResponseModel();
                            scheduleResponseModel2.ScheduleId = item.ScheduleId;;
                            scheduleResponseModel2.ScheduleStartTime = item.ScheduleStartTime.ToString("HH:mm");
                            scheduleResponseModel2.ScheduleEndTime =item.ScheduleEndTime.ToString("HH:mm");
                            scheduleResponseModel2.ParentId = item.ParentId;
                            scheduleResponseModelList.Add(scheduleResponseModel2);
                        }
                    }
                    else
                    {
                        scheduleResponseModel.MessageError = "No se encontró información de horarios.";
                        scheduleResponseModelList.Add(scheduleResponseModel);
                    }
                }
            }
            catch (Exception ex)
            {
                scheduleResponseModel.MessageError = "Ocurrió un error inesperado. Inténtelo de nuevo.";
                scheduleResponseModelList.Add(scheduleResponseModel);
            }
            return scheduleResponseModelList;
        }
        
    }
}