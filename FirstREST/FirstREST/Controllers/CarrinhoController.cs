using System.Web.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using FirstREST.Lib_Primavera.Model;

namespace FirstREST.Controllers
{
    public class CarrinhoController : ApiController
    {
        // GET: /api/carrinho

        public IEnumerable<Lib_Primavera.Model.Carrinho> Get()
        {
            return Lib_Primavera.PriIntegration.ListaCarrinhos();
        }


        // GET api/cliente/5    
        public Carrinho Get(string id)
        {
            Lib_Primavera.Model.Carrinho carrinho = Lib_Primavera.PriIntegration.GetCarrinhoUser(id);
            if (carrinho == null)
            {
                throw new HttpResponseException(
                        Request.CreateResponse(HttpStatusCode.NotFound));

            }
            else
            {
                return carrinho;
            }
        }
    }
}
