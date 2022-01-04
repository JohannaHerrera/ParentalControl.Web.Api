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
    [RoutePrefix("api/DeviceUseRules")]
    public class DeviceUseRulesController : ApiController
    {
        [HttpGet]
        public DeviceUseRulesResponseModel GetDeviceUseRules([FromUri] string infantAccountId)
        {
            DeviceUseRulesResponseModel deviceUseRulesResponseModel = new DeviceUseRulesResponseModel();
            List<DeviceUseRulesModel> deviceUseRulesModelList = new List<DeviceUseRulesModel>();

            try
            {
                int infantId = Convert.ToInt32(infantAccountId);
                using (var db = new ParentalControlDBEntities())
                {
                    var deviceUseList = (from DeviceUse in db.DeviceUse
                                        where DeviceUse.InfantAccountId == infantId
                                        select DeviceUse).ToList();

                    if (deviceUseList.Count() > 0)
                    {
                        foreach (var item in deviceUseList)
                        {
                            DeviceUseRulesModel listDeviceRulesModel = new DeviceUseRulesModel();
                            listDeviceRulesModel.DeviceUseId = item.DeviceUseId;
                            listDeviceRulesModel.DeviceUseDay = item.DeviceUseDay;
                            listDeviceRulesModel.DeviceUseCreationDate = item.DeviceUseCreationDate;
                            listDeviceRulesModel.InfantAccountId = item.InfantAccountId;
                            listDeviceRulesModel.ScheduleId = item.ScheduleId;
                            deviceUseRulesModelList.Add(listDeviceRulesModel);
                        }

                        deviceUseRulesResponseModel.deviceUseRulesModelList = deviceUseRulesModelList;
                    }
                    else
                    {
                        deviceUseRulesResponseModel.MessageError = "No se obtuvo información sobre el uso del dispositivo.";
                    }

                }
            }
            catch (Exception ex)
            {
                deviceUseRulesResponseModel.MessageError = "Ha ocurrido un error inesperado.";
            }

            return deviceUseRulesResponseModel;
        }

    }
}