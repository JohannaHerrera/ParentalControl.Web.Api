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
    [RoutePrefix("api/Request")]
    public class RequestController : ApiController
    {
        [HttpPost]
        public GetRequestsResponseModel Post([FromBody] int parentId)
        {
            GetRequestsResponseModel getRequestsResponseModel = new GetRequestsResponseModel();
            List<RequestModel> requestModelList = new List<RequestModel>();
            AppConstants constants = new AppConstants();

            try
            {
                if (parentId > 0)
                {
                    using (var db = new ParentalControlDBEntities())
                    {
                        var requestList = (from request in db.Request
                                           join infant in db.InfantAccount
                                           on request.InfantAccountId
                                           equals infant.InfantAccountId
                                           where request.ParentId == parentId
                                           select new RequestModel
                                           {
                                               RequestId = request.RequestId,
                                               RequestTypeId = request.RequestTypeId,
                                               InfantAccountId = request.InfantAccountId,
                                               RequestObject = request.RequestObject,
                                               RequestState = request.RequestState,
                                               RequestTime = request.RequestTime,
                                               InfantGender = infant.InfantGender,
                                               InfantName = infant.InfantName,
                                               DevicePCId = request.DevicePCId,
                                               DevicePhoneId = request.DevicePCId
                                           }).ToList();

                        if (requestList != null && requestList.Count() > 0)
                        {
                            foreach (var request in requestList)
                            {
                                if (request.RequestTypeId == constants.WebConfiguration)
                                {
                                    request.RequestDescription = $"Petición para habilitar el acceso a la categoría" +
                                                                  $" web: {request.RequestObject}.";
                                }
                                else if (request.RequestTypeId == constants.AppConfiguration)
                                {
                                    if (request.DevicePhoneId != null)
                                    {
                                        var deviceInfo = (from device in db.DevicePhone
                                                          where device.DevicePhoneId == request.DevicePhoneId
                                                          select device).FirstOrDefault();
                                        request.RequestDescription = $"Petición para habilitar el acceso a la aplicación:" +
                                                                  $" {request.RequestObject} del dispositivo {deviceInfo.DevicePhoneName}.";
                                    }
                                    else if (request.DevicePCId != null)
                                    {
                                        var deviceInfo = (from device in db.DevicePC
                                                          where device.DevicePCId == request.DevicePCId
                                                          select device).FirstOrDefault();
                                        request.RequestDescription = $"Petición para habilitar el acceso a la aplicación:" +
                                                                  $" {request.RequestObject} del dispositivo {deviceInfo.DevicePCName}.";
                                    }   
                                }
                                else if (request.RequestTypeId == constants.DeviceConfiguration)
                                {
                                    string nombre = string.Empty;
                                    
                                    if(request.DevicePCId != null)
                                    {
                                        nombre = "PC";
                                    }
                                    else if (request.DevicePhoneId != null)
                                    {
                                        nombre = (from device in db.DevicePhone
                                                  where device.DevicePhoneId == request.DevicePhoneId
                                                  select device).FirstOrDefault().DevicePhoneName;
                                    }

                                    string[] time = request.RequestTime.ToString().Split('.');
                                    int numEntero = 0;
                                    int numDecimal = 0;

                                    if (time.Count() > 1)
                                    {
                                        numEntero = int.Parse(time[0]);
                                        numDecimal = int.Parse(time[1]);
                                    }
                                    else
                                    {
                                        numEntero = int.Parse(time[0]);
                                    }

                                    if (numEntero > 0)
                                    {
                                        if (numEntero == 1)
                                        {
                                            if (numDecimal > 0)
                                            {
                                                request.RequestDescription = $"Petición para extender el tiempo de uso del " +
                                                                              $"dispositivo {nombre} " +
                                                                              $"por {numEntero} hora y {numDecimal}" +
                                                                              $" minutos.";
                                            }
                                            else
                                            {
                                                request.RequestDescription = $"Petición para extender el tiempo de uso del " +
                                                                              $"dispositivo {nombre} por {numEntero} hora.";
                                            }
                                        }
                                        else
                                        {
                                            if (numDecimal > 0)
                                            {
                                                request.RequestDescription = $"Petición para extender el tiempo de uso del " +
                                                                              $"dispositivo {nombre} por {numEntero} " +
                                                                              $"horas y {numDecimal}" +
                                                                              $" minutos.";
                                            }
                                            else
                                            {
                                                request.RequestDescription = $"Petición para extender el tiempo de uso del " +
                                                                              $"dispositivo {nombre} por {numEntero} horas.";
                                            }
                                        }
                                    }
                                    else
                                    {
                                        request.RequestDescription = $"Petición para extender el tiempo de uso del " +
                                                                      $"dispositivo {nombre} por {numDecimal} minutos.";
                                    }
                                }
                            }

                            getRequestsResponseModel.requestModelList = requestList;
                        }
                    }
                }
                else
                {
                    getRequestsResponseModel.MessageError = "No se pudo encontrar información sobre las peticiones.";
                }
            }
            catch(Exception ex)
            {
                getRequestsResponseModel.MessageError = "Ocurrió un error inesperado. Inténtelo de nuevo.";
            }

            return getRequestsResponseModel;
        }

        [HttpPut]
        public bool Put([FromBody] UpdateRequestModel updateRequestModel)
        {
            bool result = false;

            try
            {
                if(updateRequestModel.RequestId > 0 && updateRequestModel.RequestAction > 0)
                {
                    using(var db = new ParentalControlDBEntities())
                    {
                        var requestUpdate = (from request in db.Request
                                             where request.RequestId == updateRequestModel.RequestId
                                             select request).FirstOrDefault();

                        if(requestUpdate != null)
                        {
                            requestUpdate.RequestState = updateRequestModel.RequestAction;
                            db.SaveChanges();
                            result = true;
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                result = false;
            }

            return result;
        }
    }
}