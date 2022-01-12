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
    [RoutePrefix("api/WebConfigurationRules")]
    public class WebConfigurationRulesController : ApiController
    {
        [HttpGet]
        public WebConfigurationRulesResponseModel GetWebConfigurationRules([FromUri] string infantAccountId)
        {
            WebConfigurationRulesResponseModel webConfigurationRulesResponseModel = new WebConfigurationRulesResponseModel();
            List<WebConfigurationRulesModel> webConfigurationRulesModelList = new List<WebConfigurationRulesModel>();

            try
            {
                int infantId = Convert.ToInt32(infantAccountId);
                using (var db = new ParentalControlDBEntities())
                {
                    var webConfigurationList = (from WebConfiguration in db.WebConfiguration
                                                where WebConfiguration.InfantAccountId == infantId
                                                select WebConfiguration).ToList();

                    if (webConfigurationList.Count() > 0)
                    {
                        foreach (var item in webConfigurationList)
                        {
                            WebConfigurationRulesModel listWebConfigurationRulesModel = new WebConfigurationRulesModel();
                            listWebConfigurationRulesModel.WebConfigurationId = item.WebConfigurationId;
                            listWebConfigurationRulesModel.CategoryId = item.CategoryId;
                            listWebConfigurationRulesModel.WebConfigurationAccess = item.WebConfigurationAccess;                            
                            listWebConfigurationRulesModel.InfantAccountId = item.InfantAccountId;

                            if(item.CategoryId == 1)
                            {
                                listWebConfigurationRulesModel.CategoryName = "Drogas";
                            }
                            else if(item.CategoryId == 2)
                            {
                                listWebConfigurationRulesModel.CategoryName = "Pornografía";
                            }
                            else if(item.CategoryId == 3)
                            {
                                listWebConfigurationRulesModel.CategoryName = "Videojuegos";
                            }
                            else
                            {
                                listWebConfigurationRulesModel.CategoryName = "Violencia";
                            }

                            webConfigurationRulesModelList.Add(listWebConfigurationRulesModel);
                        }

                        webConfigurationRulesResponseModel.webConfigurationRulesModelList = webConfigurationRulesModelList;
                        webConfigurationRulesResponseModel.IsSuccess = true;
                    }
                    else
                    {
                        webConfigurationRulesResponseModel.MessageError = "No se obtuvo información sobre la Configuración Web.";
                    }

                }
            }
            catch (Exception ex)
            {
                webConfigurationRulesResponseModel.MessageError = "Ha ocurrido un error inesperado.";
            }

            return webConfigurationRulesResponseModel;
        }

        [HttpPut]
        public bool UpdateWebConfigurationRules([FromBody] List<UpdateWebConfigurationRulesModel> updateWebConfigurationRulesModel)
        {
            bool result = false;

            try
            {
                foreach (var webConfig in updateWebConfigurationRulesModel)
                {
                    if (webConfig.InfantAccountId > 0)
                    {
                        using (var db = new ParentalControlDBEntities())
                        {
                            var webConfigurationList = (from WebConfiguration in db.WebConfiguration
                                                        join WebCateogires in db.WebCategory
                                                        on WebConfiguration.CategoryId
                                                        equals WebCateogires.CategoryId
                                                        where WebCateogires.CategoryName == webConfig.CategoryName
                                                        && WebConfiguration.InfantAccountId == webConfig.InfantAccountId
                                                        select WebConfiguration).FirstOrDefault();

                            if (webConfigurationList != null)
                            {
                                // Actualizo el nombre del dispositivo
                                webConfigurationList.WebConfigurationAccess = webConfig.WebConfigurationAccess;
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
                
            }
            catch (Exception ex)
            {
                result = false;
            }

            return result;
        }
    }
}