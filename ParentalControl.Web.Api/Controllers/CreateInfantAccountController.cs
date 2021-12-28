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
    [RoutePrefix("api/CreateInfantAccount")]
    public class CreateInfantAccountController : ApiController
    {

        //Este va a ser para crear
        [HttpPost]
        public InfantAccountResponseModel CreateInfantAccount([FromBody] CreateInfantAccountModel createInfantAccountModel)
        {
            InfantAccountResponseModel infantAccountResponseModel = new InfantAccountResponseModel();
            infantAccountResponseModel.IsSuccess = false;

            try
            {
                if (createInfantAccountModel.InfantName != null && createInfantAccountModel.InfantGender != null
                        && createInfantAccountModel.ParentId != 0)
                {
                    using (var db = new ParentalControlDBEntities())
                    {
                        var infantAccount = (from InfantAccount in db.InfantAccount
                                             where InfantAccount.InfantName == createInfantAccountModel.InfantName
                                             && InfantAccount.ParentId == createInfantAccountModel.ParentId
                                             select InfantAccount).FirstOrDefault();

                        if (infantAccount != null)
                        {
                            // Verifico si ya existe una cuenta con el mismo nombre y el mismo id de padre
                            infantAccountResponseModel.MessageError = "Error. Ya existe una cuenta de infante con el mismo nombre.";
                        }
                        else
                        {
                            var creationDate = DateTime.Now;

                            // Realizo el registro de la cuenta
                            InfantAccount infantAccount1 = new InfantAccount();
                            infantAccount1.InfantName = createInfantAccountModel.InfantName;
                            infantAccount1.InfantGender = createInfantAccountModel.InfantGender;
                            infantAccount1.ParentId = createInfantAccountModel.ParentId;
                            infantAccount1.InfantCreationDate = creationDate;
                            db.InfantAccount.Add(infantAccount1);
                            db.SaveChanges();
                            
                            //Creo los registros de categorías para ese usuario
                            List<WebCategory> webCategorieList = db.WebCategory.ToList();
                            foreach (var category in webCategorieList)
                            {
                                WebConfiguration webConfiguration = new WebConfiguration();
                                webConfiguration.WebConfigurationAccess = false;
                                webConfiguration.CategoryId = category.CategoryId;
                                webConfiguration.InfantAccountId = infantAccount1.InfantAccountId;
                                db.WebConfiguration.Add(webConfiguration);
                                db.SaveChanges();
                            }

                            //Creo los registros de tiempo de uso del dispositivo para ese usuario
                            string[] dias = new string[7] { "Lunes", "Martes", "Miércoles", "Jueves", "Viernes", "Sábado", "Domingo" };
                            foreach (var dia in dias)
                            {
                                DeviceUse deviceUse = new DeviceUse();
                                deviceUse.DeviceUseDay = dia;
                                deviceUse.DeviceUseCreationDate = creationDate;
                                deviceUse.InfantAccountId = infantAccount1.InfantAccountId;
                                deviceUse.ScheduleId = null;
                                db.DeviceUse.Add(deviceUse);
                                db.SaveChanges();
                            }

                            infantAccountResponseModel.IsSuccess = true;
                        }
                    }
                }
                else
                {
                    infantAccountResponseModel.MessageError = "Ingrese todos los datos requeridos.";
                }

            }
            catch (Exception ex)
            {
                infantAccountResponseModel.MessageError = "Ocurrió un error inesperado. Inténtelo de nuevo.";
            }

            return infantAccountResponseModel;
        }
    }
}