using System;
using System.Collections.Generic;
using System.Web.Http;
using System.Linq;
using ParentalControl.Web.Api.Models.ReponseModels;
using ParentalControl.Web.Api.Models.EntityModels;
using ParentalControl.Web.Api.Data;

namespace ParentalControl.Web.Api.Controllers
{
    [AllowAnonymous]
    [RoutePrefix("api/Register")]
    public class RegisterController : ApiController
    {
        [HttpPost]
        public RegisterResponseModel Post([FromBody] RegisterModel registerModel)
        {
            RegisterResponseModel registerResponseModel = new RegisterResponseModel();
            registerResponseModel.Registered = false;

            try
            {
                if (registerModel.ParentUsername != null && registerModel.ParentEmail != null 
                        && registerModel.ParentPassword != null)
                {
                    using (var db = new ParentalControlDBEntities())
                    {
                        var loginParent = (from parent in db.Parent
                                           where parent.ParentEmail == registerModel.ParentEmail
                                           select parent).FirstOrDefault();

                        if (loginParent != null)
                        {
                            // Verifico si ya existe una cuenta con el mismo correo
                            registerResponseModel.MessageError = "Error. Ya existe una cuenta con el mismo correo.";
                        }
                        else
                        {
                            // Realizo el registro de la cuenta
                            Parent parent = new Parent();
                            parent.ParentUsername = registerModel.ParentUsername;
                            parent.ParentEmail = registerModel.ParentEmail;
                            parent.ParentPassword = registerModel.ParentPassword;
                            parent.ParentCreationDate = DateTime.Now;
                            db.Parent.Add(parent);
                            db.SaveChanges();

                            registerResponseModel.Registered = true;
                        }
                    }
                }
                else
                {
                    registerResponseModel.MessageError = "Ingrese todos los datos requeridos.";
                }

            }
            catch (Exception ex)
            {
                registerResponseModel.MessageError = "Ocurrió un error inesperado. Inténtelo de nuevo.";
            }

            return registerResponseModel;
        }
    }
}