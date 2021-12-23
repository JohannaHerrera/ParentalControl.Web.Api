using ParentalControl.Web.Api.Data;
using ParentalControl.Web.Api.Models.EntityModels;
using ParentalControl.Web.Api.Models.ReponseModels;
using System;
using System.Net;
using System.Threading;
using System.Web.Http;
using System.Linq;
using System.Collections.Generic;

namespace ParentalControl.Web.Api.Controllers
{
    [AllowAnonymous]
    [RoutePrefix("api/News")]
    public class NewsController : ApiController
    {
        [HttpGet]
        public NewsResponseModel Get()
        {
            NewsResponseModel newsResponseModel = new NewsResponseModel();
            List<NewsModel> newsModelList = new List<NewsModel>();

            try
            {
                using(var db = new ParentalControlDBEntities())
                {
                    var newsInfo = (from ns in db.News
                                    select ns).ToList();

                    if(newsInfo.Count() > 0)
                    {
                        foreach (var item in newsInfo)
                        {
                            NewsModel newsModel = new NewsModel();
                            newsModel.NewsTitle = item.NewsTitle;
                            newsModel.NewsDescription = item.NewsDescription;
                            newsModel.NewsLink = item.NewsLink;
                            newsModelList.Add(newsModel);
                        }

                        newsResponseModel.NewsModelList = newsModelList; 
                    }
                    else
                    {
                        newsResponseModel.MessageError = "No se obtuvo información sobre las últimas noticias.";
                    }
                    
                }
            }
            catch(Exception ex)
            {
                newsResponseModel.MessageError = "Ha ocurrido un error inesperado.";
            }

            return newsResponseModel;
        }
    }
}