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
    [RoutePrefix("api/AppRules")]
    public class AppRulesController : ApiController
    {
        [HttpGet]
        public AppRulesResponseModel GetAppRules([FromUri] string infantAccountId)
        {
            AppRulesResponseModel appRulesResponseModel = new AppRulesResponseModel();
            List<AppRulesModel> appRulesModelList = new List<AppRulesModel>();

            try
            {
                int infantId = Convert.ToInt32(infantAccountId);
                using (var db = new ParentalControlDBEntities())
                {
                    var webConfigurationList = (from App in db.App
                                                where App.InfantAccountId == infantId
                                                select App).ToList();

                    if (webConfigurationList.Count() > 0)
                    {
                        foreach (var item in webConfigurationList)
                        {
                            AppRulesModel listAppRulesModel = new AppRulesModel();
                            listAppRulesModel.AppId = item.AppId;
                            listAppRulesModel.DevicePhoneId = item.DevicePhoneId;
                            listAppRulesModel.InfantAccountId = item.InfantAccountId;
                            listAppRulesModel.ScheduleId = item.ScheduleId;
                            listAppRulesModel.AppName = item.AppName;
                            listAppRulesModel.AppAccessPermission = item.AppAccessPermission;
                            appRulesModelList.Add(listAppRulesModel);
                        }

                        appRulesResponseModel.appRulesModelList = appRulesModelList;
                    }
                    else
                    {
                        appRulesResponseModel.MessageError = "No se obtuvo información sobre las aplicaciones.";
                    }

                }
            }
            catch (Exception ex)
            {
                appRulesResponseModel.MessageError = "Ha ocurrido un error inesperado.";
            }

            return appRulesResponseModel;
        }
    }
}