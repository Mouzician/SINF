using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Http;
using System.Text.RegularExpressions;

namespace FirstREST.Controllers
{
    [HandleError]
    public class HomeController : Controller
    {
        // GET: /Home/
        [System.Web.Mvc.HttpGet]
        public ActionResult Index(String op, String op_dois)
        {

            if (op == "Artigos")
            {

                if (op_dois == null)
                {

                    IEnumerable<Lib_Primavera.Model.Artigo> artigos = Lib_Primavera.PriIntegration.ListaArtigos();

                    IEnumerable<Lib_Primavera.Model.Artigo> temp = artigos.Take(6);


                    ViewBag.artigos = temp;

                    return View("/Views/ArtigoPage/produtos.cshtml");
                }
                else
                {

                    Lib_Primavera.Model.Artigo artigo = Lib_Primavera.PriIntegration.GetArtigo(op_dois);

                    ViewBag.id = artigo.ID;
                    ViewBag.preco = artigo.Preço;
                    ViewBag.descricao = artigo.Descricao;
                    ViewBag.stoke = artigo.SubFamilia;
                    ViewBag.imagem = artigo.CDU_Imagem;

                    //fazer os recomendados , que acho que nao esta a dar a outra funçao.
                    //Lib_Primavera.Model.Artigo artigos = Lib_Primavera.PriIntegration.GetArtigoByCategoria(artigo.SubFamilia);

                    return View("/Views/ArtigoPage/Index.cshtml");
                }
            }
            else
                return null;
        }

    }
       
}

