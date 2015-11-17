using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Http;

namespace FirstREST.Controllers
{
    [HandleError]
    public class ArtigoPageController : Controller
    {

        // GET: /ArtigoPage/
        [System.Web.Mvc.HttpGet]
        public ActionResult Index(string id)
        {
            //string idString = id.ToString();

            Lib_Primavera.Model.Artigo artigo = Lib_Primavera.PriIntegration.GetArtigo(id);

            ViewBag.id = artigo.ID;
            ViewBag.preco = artigo.Preço;
            return View();
        }

       

   
    }

}
