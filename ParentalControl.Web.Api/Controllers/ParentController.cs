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
    public class ParentController : ApiController
    {
        [HttpPost]
        public GetMyProfileResponseModel Post([FromBody] int parentId)
        {
            GetMyProfileResponseModel getMyProfileResponseModel = new GetMyProfileResponseModel();

            try
            {
                if(parentId > 0)
                {
                    using(var db = new ParentalControlDBEntities())
                    {
                        var parentInfo = (from parent in db.Parent
                                          where parent.ParentId == parentId
                                          select parent).FirstOrDefault();

                        if(parentInfo != null)
                        {
                            getMyProfileResponseModel.ParentUsername = parentInfo.ParentUsername;
                            getMyProfileResponseModel.ParentEmail = parentInfo.ParentEmail;
                            getMyProfileResponseModel.ParentPassword = parentInfo.ParentPassword;
                        }
                    }
                }
                else
                {
                    getMyProfileResponseModel.MessageError = "No se pudo encontrar su información personal.";
                }
            }
            catch (Exception ex)
            {
                getMyProfileResponseModel.MessageError = "Ocurrió un error inesperado. Inténtelo de nuevo.";
            }

            return getMyProfileResponseModel;
        }

        [HttpPut]
        public bool Put([FromBody] MyProfileModel myProfileModel)
        {
            bool result = false;

            try
            {
                if (myProfileModel != null && myProfileModel.ParentId > 0)
                {
                    using (var db = new ParentalControlDBEntities())
                    {
                        var parentInfo = (from parent in db.Parent
                                          where parent.ParentId == myProfileModel.ParentId
                                          select parent).FirstOrDefault();

                        if (parentInfo != null)
                        {
                            parentInfo.ParentUsername = myProfileModel.ParentUsername;
                            parentInfo.ParentEmail = myProfileModel.ParentEmail;
                            parentInfo.ParentPassword = myProfileModel.ParentPassword;
                            db.SaveChanges();

                            result = true;
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