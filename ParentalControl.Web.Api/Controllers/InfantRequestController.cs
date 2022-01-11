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
    [RoutePrefix("api/InfantRequest")]
    public class InfantRequestController : ApiController
    {
        [HttpPost]
        public GetInfantRequestResponseModel Post([FromBody] string devicePhoneCode)
        {
            GetInfantRequestResponseModel getInfantRequestResponseModel = new GetInfantRequestResponseModel();
            List<InfantRequestModel> requestModelList = new List<InfantRequestModel>();
            AppConstants constants = new AppConstants();

            try
            {
                if (!string.IsNullOrEmpty(devicePhoneCode))
                {
                    using (var db = new ParentalControlDBEntities())
                    {
                        var deviceInfo = (from device in db.DevicePhone
                                          where device.DevicePhoneCode.ToLower().Equals(devicePhoneCode.ToLower())
                                          && device.InfantAccountId != null
                                          select device).FirstOrDefault();

                        if(deviceInfo != null)
                        {
                            var requestList = (from request in db.Request
                                               join device in db.DevicePhone
                                               on request.DevicePhoneId
                                               equals device.DevicePhoneId
                                               where device.DevicePhoneCode.ToLower().Equals(devicePhoneCode.ToLower())
                                               && request.InfantAccountId == deviceInfo.InfantAccountId
                                               select request).ToList();

                            if (requestList != null && requestList.Count() > 0)
                            {
                                foreach (var request in requestList)
                                {
                                    InfantRequestModel infantRequestModel = new InfantRequestModel();

                                    if (request.RequestState == 0)
                                    {
                                        infantRequestModel.StateRequest = "En espera";
                                    }
                                    else if (request.RequestState == 1)
                                    {
                                        infantRequestModel.StateRequest = "Aprobado";
                                    }
                                    else if (request.RequestState == 2)
                                    {
                                        infantRequestModel.StateRequest = "Desaprobado";
                                    }
                                    else if (request.RequestState == 3)
                                    {
                                        infantRequestModel.StateRequest = "Sin Respuesta";
                                    }

                                    if (request.RequestTypeId == constants.WebConfiguration)
                                    {
                                        infantRequestModel.RequestDescription = $"Petición para habilitar el acceso a la categoría" +
                                                                                $" web: {request.RequestObject}.";
                                    }
                                    else if (request.RequestTypeId == constants.AppConfiguration)
                                    {
                                        infantRequestModel.RequestDescription = $"Petición para habilitar el acceso a la aplicación:" +
                                                                                $" {request.RequestObject}.";                                        
                                    }
                                    else if (request.RequestTypeId == constants.DeviceConfiguration)
                                    {
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
                                                    infantRequestModel.RequestDescription = $"Petición para extender el tiempo de uso del " +
                                                                                  $"dispositivo por {numEntero} hora y {numDecimal}" +
                                                                                  $" minutos.";
                                                }
                                                else
                                                {
                                                    infantRequestModel.RequestDescription = $"Petición para extender el tiempo de uso del " +
                                                                                  $"dispositivo por {numEntero} hora.";
                                                }
                                            }
                                            else
                                            {
                                                if (numDecimal > 0)
                                                {
                                                    infantRequestModel.RequestDescription = $"Petición para extender el tiempo de uso del " +
                                                                                  $"dispositivo por {numEntero} " +
                                                                                  $"horas y {numDecimal}" +
                                                                                  $" minutos.";
                                                }
                                                else
                                                {
                                                    infantRequestModel.RequestDescription = $"Petición para extender el tiempo de uso del " +
                                                                                  $"dispositivo por {numEntero} horas.";
                                                }
                                            }
                                        }
                                        else
                                        {
                                            infantRequestModel.RequestDescription = $"Petición para extender el tiempo de uso del " +
                                                                          $"dispositivo por {numDecimal} minutos.";
                                        }
                                    }

                                    requestModelList.Add(infantRequestModel);
                                }

                                getInfantRequestResponseModel.requestModelList = requestModelList;
                            }
                            else
                            {
                                getInfantRequestResponseModel.MessageError = "Aún no tienes notificaciones por revisar.";
                            }
                        }
                        else
                        {
                            getInfantRequestResponseModel.MessageError = "El dispositivo no está asignado a ningún infante.";
                        }
                    }
                }
                else
                {
                    getInfantRequestResponseModel.MessageError = "No se pudo encontrar información sobre las notificaciones.";
                }
            }
            catch (Exception ex)
            {
                getInfantRequestResponseModel.MessageError = "Ocurrió un error inesperado. Inténtelo de nuevo.";
            }

            return getInfantRequestResponseModel;
        }
    }
}