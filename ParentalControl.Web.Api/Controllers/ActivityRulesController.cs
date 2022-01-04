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
    [RoutePrefix("api/ActivityRules")]
    public class ActivityRulesController : ApiController
    {
        /// <summary>
        /// Obtener la lista de todas las Actividades
        /// </summary>
        /// <param name="infantAccountId"></param>
        /// <returns></returns>
        [HttpGet]
        public ActivityRulesResponseModel GetActivityRules([FromUri] string infantAccountId)
        {
            ActivityRulesResponseModel activityRulesResponseModel = new ActivityRulesResponseModel();
            List<ActivityRulesModel> activityRulesModelList = new List<ActivityRulesModel>();

            try
            {
                int infantId = Convert.ToInt32(infantAccountId);
                using (var db = new ParentalControlDBEntities())
                {
                    var activityList = (from Activity in db.Activity
                                      where Activity.InfantAccountId == infantId
                                        select Activity).ToList();

                    if (activityList.Count() > 0)
                    {
                        foreach (var item in activityList)
                        {
                            ActivityRulesModel listActivityRulesModel = new ActivityRulesModel();
                            listActivityRulesModel.ActivityId = item.ActivityId;
                            listActivityRulesModel.InfantAccountId = item.InfantAccountId;
                            listActivityRulesModel.ActivityObject = item.ActivityObject;
                            listActivityRulesModel.ActivityDescription = item.ActivityDescription;
                            listActivityRulesModel.ActivityCreationDate = item.ActivityCreationDate;
                            listActivityRulesModel.ActivityTimesAccess = item.ActivityTimesAccess;
                            activityRulesModelList.Add(listActivityRulesModel);
                        }

                        activityRulesResponseModel.activityRulesModelList = activityRulesModelList;
                    }
                    else
                    {
                        activityRulesResponseModel.MessageError = "No se obtuvo información sobre la actividad del infante.";
                    }

                }
            }
            catch (Exception ex)
            {
                activityRulesResponseModel.MessageError = "Ha ocurrido un error inesperado.";
            }

            return activityRulesResponseModel;
        }


        ////Este va a ser para actualizar
        //[HttpPut]
        //public bool UpdateInfantAccount([FromBody] UpdateInfantAccountModel updateInfantAccountModel)
        //{
        //    bool result = false;

        //    try
        //    {
        //        if (!string.IsNullOrEmpty(updateInfantAccountModel.InfantName) && !string.IsNullOrEmpty(updateInfantAccountModel.InfantGender)
        //            && updateInfantAccountModel.ParentId > 0 && updateInfantAccountModel.InfantAccountId > 0)
        //        {
        //            using (var db = new ParentalControlDBEntities())
        //            {
        //                var infantAccount = (from InfantAccount in db.InfantAccount
        //                                     where InfantAccount.InfantAccountId == updateInfantAccountModel.InfantAccountId
        //                                     && InfantAccount.ParentId == updateInfantAccountModel.ParentId
        //                                     select InfantAccount).FirstOrDefault();

        //                if (infantAccount != null)
        //                {
        //                    InfantAccount infantAccount1 = infantAccount;
        //                    infantAccount1.InfantName = updateInfantAccountModel.InfantName;
        //                    infantAccount1.InfantGender = updateInfantAccountModel.InfantGender;
        //                    db.Entry(infantAccount1).State = System.Data.Entity.EntityState.Modified;
        //                    db.SaveChanges();

        //                    return true;
        //                }
        //            }
        //        }
        //        else
        //        {
        //            result = false;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        result = false;
        //    }

        //    return result;
        //}

    }
}