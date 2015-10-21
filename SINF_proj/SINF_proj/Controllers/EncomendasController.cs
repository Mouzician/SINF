using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using SINF_proj.Lib_Primavera.Model;

namespace SINF_proj.Controllers
{
    public class EncomendasController : ApiController
    {
        
         //GET: api/encomendas/
        public IEnumerable<Lib_Primavera.Model.Encomenda> Get()
        {
            return Lib_Primavera.EncomendasGes.Encomendas_Lista();
        }

        // GET api/encomendas/5    
        public Encomenda Get(string id)
        {
            Lib_Primavera.Model.Encomenda doc_venda = Lib_Primavera.EncomendasGes.getEncomenda(id);
            if (doc_venda == null)
            {
                throw new HttpResponseException(
                        Request.CreateResponse(HttpStatusCode.NotFound));

            }
            else
            {
                return doc_venda;
            }
        }

        //GET api/encomendas/dataInicio/dataFim
        public IEnumerable<Lib_Primavera.Model.Encomenda> Get(string id, string id2)
        {
            return Lib_Primavera.EncomendasGes.getEncomendasNoIntervalo(id, id2);
        }
    }
}
