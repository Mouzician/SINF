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
            if (op == "Categoria")
            {
                if (op_dois == null)
                {
                    return View("/Views/Home/Index.cshtml");
                }

                else
                {

                    List<Lib_Primavera.Model.Artigo> artigos = Lib_Primavera.PriIntegration.ListaArtigos();
                    List<Lib_Primavera.Model.Artigo> artigos2 = new List<Lib_Primavera.Model.Artigo>();

                    foreach (var ar in artigos)
                    {
                        if (ar.Familia == op_dois)
                        {
                            artigos2.Add(ar);
                        }
                    }

                    if(artigos2.Count() == 0)
                        return View("/Views/Home/Index.cshtml");

                    else{
                        ViewBag.artigos = artigos2;
                        return View("/Views/ArtigoPage/produtos.cshtml");

                    }

                }

            }


            else if (op == "Artigos")
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


                    List<Lib_Primavera.Model.Artigo> listArts = Lib_Primavera.PriIntegration.GetArtigosByCategoria(artigo.SubFamilia);
                    ViewBag.artigos = listArts.Take(3);



                    //fazer os recomendados , que acho que nao esta a dar a outra funçao.
                    //Lib_Primavera.Model.Artigo artigos = Lib_Primavera.PriIntegration.GetArtigoByCategoria(artigo.SubFamilia);

                    return View("/Views/ArtigoPage/Index.cshtml");
                }
            }
            else if (op == "Carrinho")
            {

                if (Session["username"] == null)
                    return View("/Views/Home/Index.cshtml");

                else
                {
                    string session = Session["username"].ToString();

                    Lib_Primavera.Model.Carrinho cart = Lib_Primavera.PriIntegration.GetCarrinhoUser(session);

                    ViewBag.owner = cart.ID_Cliente;
                    ViewBag.produtos = cart.ID_Produtos;
                   

                    return View("/Views/ArtigoPage/carrinho.cshtml");
                }
            }

            else if (op == "Login")
            {

                return View("/Views/Home/Login.cshtml");

            }

       
            else
                return null;
        }


        [System.Web.Mvc.HttpPost]
        public ActionResult addCarrinho(string idProduto)
        {
            Lib_Primavera.Model.RespostaErro erro = new Lib_Primavera.Model.RespostaErro();
            Lib_Primavera.Model.TDU_CarrinhoProduto carrinhoLinha = new Lib_Primavera.Model.TDU_CarrinhoProduto();
            carrinhoLinha.CDU_idProduto = idProduto;
            Lib_Primavera.Model.Carrinho carrinho = Lib_Primavera.PriIntegration.GetCarrinhoUser(Session["username"].ToString());
            carrinhoLinha.CDU_idCarrinho = carrinho.ID;

            erro = Lib_Primavera.PriIntegration.InsereCarrinhoObj(carrinhoLinha);


            return View();
        }

        [System.Web.Mvc.HttpPost]
        public ActionResult Index(String Username, String Password,String Remember)
        {
            IEnumerable<Lib_Primavera.Model.Cliente> clientes = Lib_Primavera.PriIntegration.ListaClientes();
            bool encontrou = false;
            string idCli = "";

            foreach (var cli in clientes)
            {
                if (cli.NomeCliente.Equals(Username))
                {
                    idCli = cli.ID;
                    encontrou = true;
                }
            }

            ViewBag.Username = Username;
            ViewBag.Password = Password;

            if (!encontrou)
                return View("/Views/Home/Login.cshtml");
            else
            {
                Session["username"] = idCli;
                return View("/Views/Home/Index.cshtml");
            }
        }

       
    }
}

