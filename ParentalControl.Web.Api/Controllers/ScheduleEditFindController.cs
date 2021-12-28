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
    [RoutePrefix("api/ScheduleEditFind")]
    public class ScheduleEditFindController : ApiController
    {
        //SOLO nombre post
        // otro controlador


        [HttpPost]
        public ScheduleResponseModel Post([FromBody] ScheduleEditFindModel getScheduleInfoModel)
        {
            ScheduleResponseModel scheduleResponseModel = new ScheduleResponseModel();
            try
            {
                using (var db = new ParentalControlDBEntities())
                {
                    var scheduleInfoList = (from schedule in db.Schedule
                                            where schedule.ScheduleId == getScheduleInfoModel.ScheduleId
                                            select schedule).FirstOrDefault();
                    if (scheduleInfoList!=null)
                    {
                        scheduleResponseModel.ScheduleId = scheduleInfoList.ScheduleId;
                        scheduleResponseModel.ParentId = scheduleInfoList.ParentId;
                        scheduleResponseModel.ScheduleStartTime = scheduleInfoList.ScheduleStartTime.ToString();
                        scheduleResponseModel.ScheduleEndTime = scheduleInfoList.ScheduleEndTime.ToString();
                    }
                    else
                    {
                        scheduleResponseModel.MessageError = "No se encontró información de horarios.";
                        
                    }
                }
            }
            catch (Exception ex)
            {
                scheduleResponseModel.MessageError = "Ocurrió un error inesperado. Inténtelo de nuevo.";
                
            }
            return scheduleResponseModel;
        }

    }
    
}