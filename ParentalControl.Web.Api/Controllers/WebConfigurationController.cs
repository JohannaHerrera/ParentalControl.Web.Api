using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ParentalControl.Web.Api.Constants;
using ParentalControl.Web.Api.Data;
using ParentalControl.Web.Api.Models.EntityModels;
using ParentalControl.Web.Api.Models.ReponseModels;
using System.Web.Http;
using System.Reflection;
using System.IO;

namespace ParentalControl.Web.Api.Controllers
{
    [AllowAnonymous]
    [RoutePrefix("api/WebConfiguration")]
    public class WebConfigurationController : ApiController
    {
        //Banderas de validacion de contenido

        public static bool drugsFlag = true;
        public static bool adultFlag = true;
        public static bool gameFlag = true;
        public static bool violenceFlag = true;
        
        [HttpPost]
        public WebConfigurationResponseModel Post([FromBody] string information)
        {
            char delimit =';';
            string[] valores = information.Split(delimit);
            string url = valores[0];
            string accesoWeb = url;
            string phoneCode = valores[1];
            drugsFlag = true;
            adultFlag = true;
            gameFlag = true;
            violenceFlag = true;
            
            WebConfigurationResponseModel webConfigurationResponseModel = new WebConfigurationResponseModel();
            try
            {
                using (var db = new ParentalControlDBEntities())
                {
                    if (phoneCode!=null)
                    {
                        //Verifico si tiene deviceID sino el dispositivo aun no existe
                        var deviceInfo = (from device in db.DevicePhone
                                          where device.DevicePhoneCode == phoneCode
                                          select device).FirstOrDefault();

                        if (deviceInfo!=null)
                        {

                            //Valida si ya se asigno un infante al dispositivo
                            if (deviceInfo.InfantAccountId > 0)
                            {
                                var infantInfo = (from infant in db.InfantAccount
                                                  where infant.InfantAccountId == deviceInfo.InfantAccountId
                                                  select infant).FirstOrDefault();

                                var webConfigInfoList = (from webConfiguration in db.WebConfiguration
                                                         where webConfiguration.InfantAccountId == infantInfo.InfantAccountId
                                                         select webConfiguration).ToList();

                                //Valido si existe una configuracion web para este infante
                                if (webConfigInfoList.Count() > 0)
                                {

                                    var assembly = IntrospectionExtensions.GetTypeInfo(typeof(WebConfigurationController)).Assembly;
                                    Stream stream = assembly.GetManifestResourceStream("ParentalControl.Web.Api.FilesTxt.drugs.txt");
                                    string drugsRead;
                                    List<string> listDrugs = new List<string>();
                                    using (var reader = new System.IO.StreamReader(stream))
                                    {

                                        bool state = false;
                                        //Asigno busqueda al historial
                                        Activity activity = new Activity();
                                        try
                                        {
                                            if (!accesoWeb.Contains("https://www.google.com/"))
                                            {
                                                activity.InfantAccountId = infantInfo.InfantAccountId;
                                                activity.ActivityObject = "Búsqueda Web";
                                                activity.ActivityDescription = "El infante ingresó a: " + accesoWeb;
                                                activity.ActivityCreationDate = DateTime.Now;
                                                activity.ActivityTimesAccess = 1;
                                                db.Activity.Add(activity);
                                                db.SaveChanges();
                                            }
                                            
                                        }
                                        catch (Exception ex)
                                        {

                                        }
                                        do
                                        {
                                            if ((drugsRead = reader.ReadLine()) != null)
                                            {
                                                state = true;
                                                string newDrugs = drugsRead.Replace(' ','+');
                                                listDrugs.Add(newDrugs);
                                            }
                                            else
                                            {
                                                state = false;
                                            }

                                        } while (state==true);
                                    }
                                    var assemblyAdult = IntrospectionExtensions.GetTypeInfo(typeof(WebConfigurationController)).Assembly;
                                    Stream streamAdult = assemblyAdult.GetManifestResourceStream("ParentalControl.Web.Api.FilesTxt.adult.txt");
                                    string adultRead;
                                    List<string> listAdult = new List<string>();
                                    using (var reader = new System.IO.StreamReader(streamAdult))
                                    {
                                        bool state = false;
                                        do
                                        {
                                            if ((adultRead = reader.ReadLine()) != null)
                                            {
                                                state = true;
                                                string newAdult = adultRead.Replace(' ', '+');
                                                listAdult.Add(newAdult);
                                            }
                                            else
                                            {
                                                state = false;
                                            }

                                        } while (state == true);
                                    }


                                    var assemblyGames = IntrospectionExtensions.GetTypeInfo(typeof(WebConfigurationController)).Assembly;
                                    Stream streamGames = assemblyGames.GetManifestResourceStream("ParentalControl.Web.Api.FilesTxt.games.txt");
                                    string gamesRead;
                                    List<string> listGames = new List<string>();
                                    using (var reader = new System.IO.StreamReader(streamGames))
                                    {
                                        bool state = false;
                                        do
                                        {
                                            if ((gamesRead = reader.ReadLine()) != null)
                                            {
                                                state = true;
                                                string newGames = gamesRead.Replace(' ', '+');
                                                listGames.Add(newGames);
                                            }
                                            else
                                            {
                                                state = false;
                                            }

                                        } while (state == true);
                                    }
                                    var assemblyViolence = IntrospectionExtensions.GetTypeInfo(typeof(WebConfigurationController)).Assembly;
                                    Stream streamViolence = assemblyViolence.GetManifestResourceStream("ParentalControl.Web.Api.FilesTxt.violence.txt");
                                    string violenceRead;
                                    List<string> listViolence = new List<string>();
                                    using (var reader = new System.IO.StreamReader(streamViolence))
                                    {
                                        bool state = false;
                                        do
                                        {
                                            if ((violenceRead = reader.ReadLine()) != null)
                                            {
                                                state = true;
                                                string newViolence = violenceRead.Replace(' ', '+');
                                                listViolence.Add(newViolence);
                                            }
                                            else
                                            {
                                                state = false;
                                            }

                                        } while (state == true);
                                    }

                                    foreach (var webInfo in webConfigInfoList)
                                    {
                                        if (webInfo.WebConfigurationAccess == true && webInfo.CategoryId == 1)
                                        {
                                            foreach (var drugs in listDrugs)
                                            {
                                                if (url.ToUpper().Contains(drugs.ToUpper()))
                                                {
                                                    drugsFlag = false;
                                                    break;
                                                }
                                            }
                                            if (drugsFlag == false)
                                            {
                                                break;
                                            }
                                        }

                                        if (webInfo.WebConfigurationAccess == true && webInfo.CategoryId == 2)
                                        {
                                            foreach (var adult in listAdult)
                                            {
                                                if (url.ToUpper().Contains(adult.ToUpper()))
                                                {
                                                    adultFlag = false;
                                                    break;
                                                }
                                            }
                                            if (adultFlag == false)
                                            {
                                                break;
                                            }
                                        }

                                        if (webInfo.WebConfigurationAccess == true && webInfo.CategoryId == 3)
                                        {
                                            foreach (var game in listGames)
                                            {
                                                if (url.ToUpper().Contains(game.ToUpper()))
                                                {
                                                    gameFlag = false;
                                                    break;
                                                }
                                            }
                                            if (gameFlag == false)
                                            {
                                                break;
                                            }
                                        }
                                        if (webInfo.WebConfigurationAccess == true && webInfo.CategoryId == 4)
                                        {
                                            foreach (var violence in listViolence)
                                            {
                                                if (url.ToUpper().Contains(violence.ToUpper()))
                                                {
                                                    violenceFlag = false;
                                                    break;
                                                }
                                            }
                                            if (violenceFlag == false)
                                            {
                                                break;
                                            }
                                        }
                                    }
                                    if (drugsFlag == false || adultFlag == false || gameFlag == false || violenceFlag == false)
                                    {
                                        webConfigurationResponseModel.MessageError = "Contenido WEB bloqueado";
                                        webConfigurationResponseModel.IsSuccess = false;
                                    }
                                    else
                                    {
                                        webConfigurationResponseModel.IsSuccess = true;
                                        webConfigurationResponseModel.MessageError = null;
                                    }
                                }
                                else
                                {
                                    webConfigurationResponseModel.MessageError = "No se han asignado reglas de web a esta cuenta";
                                    webConfigurationResponseModel.IsSuccess = true;
                                }
                            }
                            else
                            {
                                webConfigurationResponseModel.MessageError = "Aún no se ha asignado una cuenta infantil a este dispositivo";
                                webConfigurationResponseModel.IsSuccess = true;
                            }


                        }
                        else
                        {
                            webConfigurationResponseModel.MessageError = "El dispositivo aún no está configurado";
                            webConfigurationResponseModel.IsSuccess = true;
                        }
                    }
                    else
                    {
                        webConfigurationResponseModel.MessageError = "Error al obtener codigo del smartphone";
                        webConfigurationResponseModel.IsSuccess = true;
                    }


                }

            }catch(Exception ex)
            {
                webConfigurationResponseModel.MessageError = "Ocurrio un error, intentelo más tarde";
            }
            return webConfigurationResponseModel;
        }
    }
}