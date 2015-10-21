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
    public class EncomendasPagasController : ApiController
    {
        //
        // GET: api/clientes/
        public IEnumerable<Lib_Primavera.Model.Encomenda> Get()
        {
            return Lib_Primavera.EncomendasGes.getEncomendasPagas();
        }


        // GET api/clientes/5    
        public IEnumerable<Lib_Primavera.Model.Encomenda> Get(string id)
        {
            return Lib_Primavera.EncomendasGes.getEncomendasPagas(id);
        }
    }
}
