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
            ViewBag.descricao = artigo.DescArtigo;
            ViewBag.stoke = artigo.SubFamilia;
            ViewBag.imagem = artigo.CDU_Imagem;
           
            //fazer os recomendados , que acho que nao esta a dar a outra funçao.
            //Lib_Primavera.Model.Artigo artigos = Lib_Primavera.PriIntegration.GetArtigoByCategoria(artigo.SubFamilia);

            return View();
        }

       

   
    }

}
