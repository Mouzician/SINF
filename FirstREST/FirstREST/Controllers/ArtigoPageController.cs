using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FirstREST.Controllers
{
    public class ArtigoPageController : Controller
    {

        //
        // GET: /ArtigoPage/

        public ActionResult Index()
        {

            Lib_Primavera.Model.Artigo artigo = Lib_Primavera.PriIntegration.GetArtigo("A0002");

            ViewBag.id = artigo.ID;
            ViewBag.preco = artigo.Preço;
            return View();
        }

        
    }
}
