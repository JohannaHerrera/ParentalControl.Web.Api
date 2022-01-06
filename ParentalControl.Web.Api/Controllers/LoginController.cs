using ParentalControl.Web.Api.Data;
using ParentalControl.Web.Api.Models.EntityModels;
using ParentalControl.Web.Api.Models.ReponseModels;
using System;
using System.Net;
using System.Threading;
using System.Web.Http;
using System.Linq;

namespace ParentalControl.Web.Api.Controllers
{
    [AllowAnonymous]
    [RoutePrefix("api/Login")]
    public class LoginController : ApiController
    {
        [HttpPost]
        public LoginResponseModel Post([FromBody] LoginModel loginModel)
        {
            LoginResponseModel loginResponseModel = new LoginResponseModel();
            loginResponseModel.IsFirstTime = false;

            try
            {
                if (loginModel.ParentEmail != null && loginModel.ParentPassword != null)
                {
                    using (var db = new ParentalControlDBEntities())
                    {
                        var loginParent = (from parent in db.Parent
                                           where parent.ParentEmail == loginModel.ParentEmail
                                           && parent.ParentPassword == loginModel.ParentPassword
                                           select parent).FirstOrDefault();

                        if (loginParent != null)
                        {
                            // Preparo el modelo de respuesta
                            loginResponseModel.ParentId = loginParent.ParentId;
                            loginResponseModel.ParentUsername = loginParent.ParentUsername;
                            loginResponseModel.ParentEmail = loginParent.ParentEmail;
                            // Verifico si ya está registrado el dispositivo
                            var devicePhone = (from device in db.DevicePhone
                                               where device.DevicePhoneCode == loginModel.deviceModel.DevicePhoneCode
                                               select device).FirstOrDefault();

                            if(devicePhone != null)
                            {
                                // Valido si está vinculado al padre que se está logeando
                                if (devicePhone.ParentId != loginParent.ParentId)
                                {
                                    loginResponseModel.MessageError = "Error. El dispositivo ya está vinculado a una cuenta padre.";
                                }
                            }
                            else
                            {
                                // Si no está registrado el dispositivo, realizo el registro en la BD
                                DevicePhone devicePhoneRegister = new DevicePhone();
                                devicePhoneRegister.DevicePhoneName = loginModel.deviceModel.DevicePhoneName;
                                devicePhoneRegister.DevicePhoneCode = loginModel.deviceModel.DevicePhoneCode;
                                devicePhoneRegister.DevicePhoneCreationDate = DateTime.Now;
                                devicePhoneRegister.InfantAccountId = null;
                                devicePhoneRegister.ParentId = loginParent.ParentId;
                                db.DevicePhone.Add(devicePhoneRegister);
                                db.SaveChanges();

                                //Obtengo el ID del dispositivo que acabo de registrar
                                var deviceRegistered = (from deviceR in db.DevicePhone
                                                        where deviceR.DevicePhoneName == loginModel.deviceModel.DevicePhoneName
                                                        && deviceR.DevicePhoneCode == loginModel.deviceModel.DevicePhoneCode
                                                        select deviceR).FirstOrDefault();

                                if(deviceRegistered != null)
                                {
                                    // Registro las aplicaciones
                                    foreach (var app in loginModel.deviceModel.AppsInstalled)
                                    {
                                        if (!(app.AppDeviceName.ToLower().Contains("android") || 
                                            app.AppDeviceName.ToLower().Contains("storage") || 
                                            app.AppDeviceName.ToLower().Contains("settings") ||
                                            app.AppDeviceName.ToLower().Contains("shell") ||
                                            app.AppDeviceName.ToLower().Contains("service") ||
                                            app.AppDeviceName.ToLower().Contains("system")))
                                        {
                                            AppDevice appDevice = new AppDevice();
                                            appDevice.DevicePhoneId = deviceRegistered.DevicePhoneId;
                                            appDevice.DevicePCId = null;
                                            appDevice.AppDeviceName = app.AppDeviceName;
                                            appDevice.AppDeviceCreationDate = DateTime.Now;
                                            db.AppDevice.Add(appDevice);
                                            db.SaveChanges();
                                        }
                                    }

                                    // Registro el uso del dispositivo
                                    DevicePhoneUse devicePhoneUse = new DevicePhoneUse();

                                    devicePhoneUse.DevicePhoneUseDay = "Lunes";
                                    devicePhoneUse.DevicePhoneCreationDate = DateTime.Now;
                                    devicePhoneUse.DevicePhoneId = deviceRegistered.DevicePhoneId;
                                    devicePhoneUse.ScheduleId = null;
                                    db.DevicePhoneUse.Add(devicePhoneUse);
                                    db.SaveChanges();

                                    devicePhoneUse.DevicePhoneUseDay = "Martes";
                                    devicePhoneUse.DevicePhoneCreationDate = DateTime.Now;
                                    devicePhoneUse.DevicePhoneId = deviceRegistered.DevicePhoneId;
                                    devicePhoneUse.ScheduleId = null;
                                    db.DevicePhoneUse.Add(devicePhoneUse);
                                    db.SaveChanges();

                                    devicePhoneUse.DevicePhoneUseDay = "Miércoles";
                                    devicePhoneUse.DevicePhoneCreationDate = DateTime.Now;
                                    devicePhoneUse.DevicePhoneId = deviceRegistered.DevicePhoneId;
                                    devicePhoneUse.ScheduleId = null;
                                    db.DevicePhoneUse.Add(devicePhoneUse);
                                    db.SaveChanges();

                                    devicePhoneUse.DevicePhoneUseDay = "Jueves";
                                    devicePhoneUse.DevicePhoneCreationDate = DateTime.Now;
                                    devicePhoneUse.DevicePhoneId = deviceRegistered.DevicePhoneId;
                                    devicePhoneUse.ScheduleId = null;
                                    db.DevicePhoneUse.Add(devicePhoneUse);
                                    db.SaveChanges();

                                    devicePhoneUse.DevicePhoneUseDay = "Viernes";
                                    devicePhoneUse.DevicePhoneCreationDate = DateTime.Now;
                                    devicePhoneUse.DevicePhoneId = deviceRegistered.DevicePhoneId;
                                    devicePhoneUse.ScheduleId = null;
                                    db.DevicePhoneUse.Add(devicePhoneUse);
                                    db.SaveChanges();

                                    devicePhoneUse.DevicePhoneUseDay = "Sábado";
                                    devicePhoneUse.DevicePhoneCreationDate = DateTime.Now;
                                    devicePhoneUse.DevicePhoneId = deviceRegistered.DevicePhoneId;
                                    devicePhoneUse.ScheduleId = null;
                                    db.DevicePhoneUse.Add(devicePhoneUse);
                                    db.SaveChanges();

                                    devicePhoneUse.DevicePhoneUseDay = "Domingo";
                                    devicePhoneUse.DevicePhoneCreationDate = DateTime.Now;
                                    devicePhoneUse.DevicePhoneId = deviceRegistered.DevicePhoneId;
                                    devicePhoneUse.ScheduleId = null;
                                    db.DevicePhoneUse.Add(devicePhoneUse);
                                    db.SaveChanges();

                                    loginResponseModel.IsFirstTime = true;
                                }                               
                            }
                        }
                        else
                        {
                            loginResponseModel.MessageError = "Credenciales incorrectas.";
                        }
                    }
                }
                else
                {
                    loginResponseModel.MessageError = "Ingrese todos los datos requeridos.";
                }
                
            }
            catch (Exception ex)
            {
                loginResponseModel.MessageError = "Ocurrió un error inesperado. Inténtelo de nuevo.";
            }

            return loginResponseModel;
        }
    }
}