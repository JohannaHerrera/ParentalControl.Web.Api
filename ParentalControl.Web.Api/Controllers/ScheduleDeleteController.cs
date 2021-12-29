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
    [RoutePrefix("api/ScheduleDelete")]
    public class ScheduleDeleteController : ApiController
    {
        [HttpPost]
        public ScheduleResponseModel Post([FromBody] GetScheduleInfoModel scheduleInfoModel)
        {
            ScheduleResponseModel scheduleResponseModel = new ScheduleResponseModel();
            scheduleResponseModel.IsSuccess = false;
            try
            {
                using (var db = new ParentalControlDBEntities())
                {
                    var schedule = (from scheduleInfo in db.Schedule
                                    where scheduleInfo.ScheduleId == scheduleInfoModel.ScheduleId
                                    select scheduleInfo).FirstOrDefault();
                    if (schedule != null)
                    {
                        //***************** Valida que se borre la llave foranea
                        var apps = db.App.Where(x => x.ScheduleId == schedule.ScheduleId).ToList();
                        foreach (var i in apps)
                        {
                            i.ScheduleId = null;
                            db.Entry(i).State = EntityState.Modified;
                            db.SaveChanges();
                        }

                        var deviceUse = db.DeviceUse.Where(x => x.ScheduleId == schedule.ScheduleId).ToList();
                        foreach (var i in deviceUse)
                        {
                            i.ScheduleId = null;
                            db.Entry(i).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                        var devicePhoneUse = db.DevicePhoneUse.Where(x => x.ScheduleId == schedule.ScheduleId).ToList();
                        foreach (var i in devicePhoneUse)
                        {
                            i.ScheduleId = null;
                            db.Entry(i).State = EntityState.Modified;
                            db.SaveChanges();
                        }

                        db.Schedule.Remove(schedule);
                        db.SaveChanges();
                        scheduleResponseModel.IsSuccess = true;
                    }
                    else
                    {
                        scheduleResponseModel.MessageError = "Ha ocurrido un error, inténtelo de nuevo";
                    }
                }
            }
            catch (Exception ex)
            {
                scheduleResponseModel.MessageError = "Ha ocurrido un error, inténtelo de nuevo";
            }
            return scheduleResponseModel;
        }
    }
}