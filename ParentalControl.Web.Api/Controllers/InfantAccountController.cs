﻿using ParentalControl.Web.Api.Constants;
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
    [RoutePrefix("api/InfantAccount")]
    public class InfantAccountController : ApiController
    {
        /// <summary>
        /// Obtener la lista de todos los infantes
        /// </summary>
        /// <param name="parentId"></param>
        /// <returns></returns>
        [HttpGet]
        public InfantAccountResponseModel GetInfantAccount(string parentId)
        {
            InfantAccountResponseModel infantAccountResponseModel = new InfantAccountResponseModel();
            List<ListInfantAccountModel> infantAccountModelList = new List<ListInfantAccountModel>();

            try
            {
                int IdParent = Convert.ToInt32(parentId);
                using (var db = new ParentalControlDBEntities())
                {
                    var infantList = (from InfantAccount in db.InfantAccount
                                      where InfantAccount.ParentId == IdParent
                                      select InfantAccount).ToList();

                    if (infantList.Count() > 0)
                    {
                        foreach (var item in infantList)
                        {
                            ListInfantAccountModel listInfantAccountModel = new ListInfantAccountModel();
                            listInfantAccountModel.InfantAccountId = item.InfantAccountId;
                            listInfantAccountModel.InfantName = item.InfantName;
                            listInfantAccountModel.InfantGender = item.InfantGender;
                            listInfantAccountModel.InfantCreationDate = item.InfantCreationDate;
                            listInfantAccountModel.ParentId = item.ParentId;
                            infantAccountModelList.Add(listInfantAccountModel);
                        }

                        infantAccountResponseModel.InfantAccountModelList = infantAccountModelList;
                    }
                    else
                    {
                        infantAccountResponseModel.MessageError = "No se obtuvo información sobre los infantes.";
                    }

                }
            }
            catch (Exception ex)
            {
                infantAccountResponseModel.MessageError = "Ha ocurrido un error inesperado.";
            }

            return infantAccountResponseModel;
        }



        //Para obtener solo un registro y para eliminar
        [HttpPost]
        public InfantAccountResponseModel GetDeleteInfantAccount([FromBody] GetInfantAccountInfoModel getInfantAccountInfoModel)
        {
            InfantAccountResponseModel infantAccountResponseModel = new InfantAccountResponseModel();
            List<ListInfantAccountModel> infantAccountModelList = new List<ListInfantAccountModel>();

            try
            {
                using (var db = new ParentalControlDBEntities())
                {
                    // GetInfantAccountInfo
                    if (getInfantAccountInfoModel.Action == 1)
                    {
                        var infantAccountInfo = (from InfantAccount in db.InfantAccount
                                                 where InfantAccount.ParentId == getInfantAccountInfoModel.ParentId
                                                 && InfantAccount.InfantName == getInfantAccountInfoModel.InfantName
                                                 select InfantAccount).FirstOrDefault();

                        if (infantAccountInfo != null)
                        {
                            ListInfantAccountModel listInfantAccountModel = new ListInfantAccountModel();
                            listInfantAccountModel.InfantAccountId = infantAccountInfo.InfantAccountId;
                            listInfantAccountModel.InfantName = infantAccountInfo.InfantName;
                            listInfantAccountModel.InfantGender = infantAccountInfo.InfantGender;
                            listInfantAccountModel.InfantCreationDate = infantAccountInfo.InfantCreationDate;
                            listInfantAccountModel.ParentId = infantAccountInfo.ParentId;
                            infantAccountModelList.Add(listInfantAccountModel);
                            infantAccountResponseModel.InfantAccountModelList = infantAccountModelList;
                        }
                        else
                        {
                            infantAccountResponseModel.MessageError = "No se encontró información del infante.";
                        }
                    }
                    else if (getInfantAccountInfoModel.Action == 2)
                    {
                        infantAccountResponseModel.IsSuccess = false;

                        // DeleteInfantAccount
                        var infantAccountInfo = (from InfantAccount in db.InfantAccount
                                                 where InfantAccount.ParentId == getInfantAccountInfoModel.ParentId
                                                 && InfantAccount.InfantName == getInfantAccountInfoModel.InfantName
                                                 select InfantAccount).FirstOrDefault();

                        if (infantAccountInfo != null)
                        {
                            int infantAccountId = infantAccountInfo.InfantAccountId;
                            var activity = db.Activity.Find(infantAccountId);
                            if (activity != null)
                            {
                                db.Activity.Remove(activity);
                                db.SaveChanges();
                            }
                            var app = db.App.Find(infantAccountId);
                            if (app != null)
                            {
                                db.App.Remove(app);
                                db.SaveChanges();
                            }
                            var devicePhone = db.DevicePhone.Find(infantAccountId);
                            if (devicePhone != null)
                            {
                                devicePhone.InfantAccountId = null;
                                db.Entry(devicePhone).State = System.Data.Entity.EntityState.Modified;
                                db.SaveChanges();
                            }
                            var deviceUse = db.DeviceUse.Find(infantAccountId);
                            if (deviceUse != null)
                            {
                                db.DeviceUse.Remove(deviceUse);
                                db.SaveChanges();
                            }
                            var request = db.Request.Find(infantAccountId);
                            if (request != null)
                            {
                                db.Request.Remove(request);
                                db.SaveChanges();
                            }
                            var webConfiguration = db.WebConfiguration.Find(infantAccountId);
                            if (webConfiguration != null)
                            {
                                db.WebConfiguration.Remove(webConfiguration);
                                db.SaveChanges();
                            }
                            var windowsAccount = db.WindowsAccount.Find(infantAccountId);
                            if (windowsAccount != null)
                            {
                                windowsAccount.InfantAccountId = null;
                                db.Entry(windowsAccount).State = System.Data.Entity.EntityState.Modified;
                                db.SaveChanges();
                            }

                            db.InfantAccount.Remove(infantAccountInfo);
                            db.SaveChanges();
                            infantAccountResponseModel.IsSuccess = true;
                            infantAccountResponseModel.InfantAccountModelList = infantAccountModelList;
                        }
                    }
                    else
                    {
                        infantAccountResponseModel.MessageError = "Error. No se especificó la acción a realizar.";
                    }
                }
            }
            catch (Exception ex)
            {
                infantAccountResponseModel.MessageError = "Ocurrió un error inesperado. Inténtelo de nuevo.";
            }

            return infantAccountResponseModel;
        }
        
        //Este va a ser para actualizar
        [HttpPut]
        public bool UpdateInfantAccount([FromBody] UpdateInfantAccountModel updateInfantAccountModel)
        {
            bool result = false;

            try
            {
                if (!string.IsNullOrEmpty(updateInfantAccountModel.InfantName) && !string.IsNullOrEmpty(updateInfantAccountModel.InfantGender)
                    && updateInfantAccountModel.ParentId > 0 && updateInfantAccountModel.InfantAccountId > 0)
                {
                    using (var db = new ParentalControlDBEntities())
                    {
                        var infantAccount = (from InfantAccount in db.InfantAccount
                                          where InfantAccount.InfantAccountId == updateInfantAccountModel.InfantAccountId
                                          && InfantAccount.ParentId == updateInfantAccountModel.ParentId
                                          select InfantAccount).FirstOrDefault();

                        if (infantAccount != null)
                        {
                            InfantAccount infantAccount1 = infantAccount;
                            infantAccount1.InfantName = updateInfantAccountModel.InfantName;
                            infantAccount1.InfantGender = updateInfantAccountModel.InfantGender;
                            db.Entry(infantAccount1).State = System.Data.Entity.EntityState.Modified;
                            db.SaveChanges();

                            return true;
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