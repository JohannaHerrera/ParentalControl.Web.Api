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
        [HttpGet]
        public string Get()
        {
            return "value";
        }

        [HttpPost]
        public LoginResponseModel Post([FromBody] LoginModel loginModel)
        {
            LoginResponseModel loginResponseModel = new LoginResponseModel();

            try
            {
                using (var db = new ParentalControlDBEntities())
                {
                    var loginParent = (from parent in db.Parent
                                       where parent.ParentEmail == loginModel.ParentEmail
                                       && parent.ParentPassword == loginModel.ParentPassword
                                       select parent).FirstOrDefault();

                    if (loginParent != null)
                    {
                        loginResponseModel.ParentId = loginParent.ParentId;
                        loginResponseModel.ParentUsername = loginParent.ParentUsername;
                        loginResponseModel.ParentEmail = loginParent.ParentEmail;
                    }
                    else
                    {
                        loginResponseModel.MessageError = "No existe el usuario";
                    }
                }
            }
            catch (Exception ex)
            {
                loginResponseModel.MessageError = "Ocurrió un error inesperado";
            }

            return loginResponseModel;
        }
    }
}