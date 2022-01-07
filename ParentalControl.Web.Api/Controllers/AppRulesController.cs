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
        [HttpPost]
        public AppRulesResponseModel Post([FromBody] GetAppConfigurationModel getAppConfigurationModel)
        {
            AppRulesResponseModel appRulesResponseModel = new AppRulesResponseModel();
            List<AppRulesModel> appRulesModelList = new List<AppRulesModel>();

            try
            {
                using (var db = new ParentalControlDBEntities())
                {
                    var appRulesList = (from App in db.App
                                        join Device in db.DevicePhone
                                        on App.DevicePhoneId
                                        equals Device.DevicePhoneId
                                        where App.InfantAccountId == getAppConfigurationModel.InfantAccountId    
                                        && Device.DevicePhoneCode.ToLower().Equals(getAppConfigurationModel.DevicePhoneCode.ToLower())
                                        select App).ToList();

                    if (appRulesList.Count() > 0)
                    {
                        foreach (var item in appRulesList)
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
                        appRulesResponseModel.IsSuccess = true;
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

        [HttpPut]
        public bool UpdateAppRules([FromBody] List<UpdateAppRulesModel> updateAppRulesModel)
        {
            AppConstants constants = new AppConstants();
            bool result = false;

            try
            {
                foreach(var app in updateAppRulesModel)
                {
                    if (app.DevicePhoneId > 0 && app.InfantAccountId > 0)
                    {
                        using (var db = new ParentalControlDBEntities())
                        {
                            var appList = (from App in db.App
                                           where App.AppName == app.AppName
                                           && App.InfantAccountId == app.InfantAccountId
                                           && App.DevicePhoneId == app.DevicePhoneId
                                           select App).FirstOrDefault();

                            if (appList != null)
                            {                              
                                if (app.ScheduleId == 0)
                                {
                                    appList.AppAccessPermission = app.AppAccessPermission;
                                    appList.ScheduleId = null;
                                    db.SaveChanges();
                                }
                                else
                                {
                                    appList.AppAccessPermission = constants.Access;
                                    appList.ScheduleId = app.ScheduleId;
                                    db.SaveChanges();
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
            }
            catch (Exception ex)
            {
                result = false;
            }

            return result;
        }
    }
}