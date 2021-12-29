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
    [RoutePrefix("api/ScheduleManage")]
    public class ScheduleManageController : ApiController
    {
        [HttpPost]
        public ScheduleResponseModel Post([FromBody] ScheduleRegisterModel scheduleRegisterModel)
        {
            ScheduleResponseModel scheduleResponseModel = new ScheduleResponseModel();
            try
            {

                scheduleResponseModel.Registered = false;
                //*********************** NECESITO LA VARIABLE DEL PADRE LOGUEADO ************************

                if (scheduleRegisterModel.ParentId > 0
                && scheduleRegisterModel.ScheduleStartTime != null
                && scheduleRegisterModel.ScheduleEndTime != null)
                {
                    using (var db = new ParentalControlDBEntities())
                    {
                        var scheduleVerification = (from scheduleV in db.Schedule
                                                    where scheduleV.ScheduleStartTime == scheduleRegisterModel.ScheduleStartTime
                                                    where scheduleV.ScheduleEndTime == scheduleRegisterModel.ScheduleEndTime
                                                    where scheduleV.ParentId == scheduleRegisterModel.ParentId /*scheduleRegisterModel.ParentId*/
                                                    select scheduleV).FirstOrDefault();

                        if (scheduleVerification != null)
                        {
                            // Verifico si ya existe un horario 
                            scheduleResponseModel.MessageError = "Error. Registro existente.";
                        }
                        else
                        {
                            // Realizo el registro del horario
                            Schedule schedule = new Schedule();
                            schedule.ParentId = scheduleRegisterModel.ParentId;
                            schedule.ScheduleStartTime = scheduleRegisterModel.ScheduleStartTime;
                            schedule.ScheduleEndTime = scheduleRegisterModel.ScheduleEndTime;
                            schedule.ScheduleCreationDate = DateTime.Now;

                            db.Schedule.Add(schedule);
                            db.SaveChanges();

                            scheduleResponseModel.Registered = true;
                        }
                    }
                }
                else
                {
                    scheduleResponseModel.MessageError = "Ingrese todos los datos requeridos.";
                }
                
                
            }
            catch (Exception ex)
            {
                scheduleResponseModel.MessageError = "Ocurrió un error inesperado. Inténtelo de nuevo.";
            }

            return scheduleResponseModel;
        }
        [HttpPut]
        public bool Put([FromBody] ScheduleUpdateModel scheduleUpdateModel)
        {
            bool result = false;

            try
            {
                if (scheduleUpdateModel.ParentId > 0
                    && scheduleUpdateModel.ScheduleId != null
                    && scheduleUpdateModel.ScheduleStartTime != null
                    && scheduleUpdateModel.ScheduleEndTime != null)
                {
                    using (var db = new ParentalControlDBEntities())
                    {
                        var scheduleInfo = (from schedule in db.Schedule
                                            where schedule.ScheduleId == scheduleUpdateModel.ScheduleId
                                            select schedule).FirstOrDefault();

                        if (scheduleInfo != null)
                        {
                            Schedule schedule = new Schedule();
                            schedule = db.Schedule.Find(scheduleUpdateModel.ScheduleId);
                            schedule.ScheduleStartTime = scheduleUpdateModel.ScheduleStartTime;
                            schedule.ScheduleEndTime = scheduleUpdateModel.ScheduleEndTime;
                            db.Entry(schedule).State = System.Data.Entity.EntityState.Modified;
                            db.SaveChanges();
                            result = true;
                        }
                    }
                }
                else
                {
                    result = false;
                }
            }
            catch (Exception ex)
            {
                result = false;
            }

            return result;
        }
        
    }
}