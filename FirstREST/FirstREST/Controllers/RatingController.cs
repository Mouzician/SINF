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
    public class RatingController : ApiController
    {
        //POST /api/Comentario/
        public HttpResponseMessage Post(TDU_Rating rate)
        {
            Lib_Primavera.Model.RespostaErro erro = new Lib_Primavera.Model.RespostaErro();
            erro = Lib_Primavera.PriIntegration.InsereRatingObj(rate);
            if (erro.Erro == 0)
            {
                return Request.CreateResponse(HttpStatusCode.Created, erro.Descricao);
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, erro.Descricao);
            }
        }

    }
}
