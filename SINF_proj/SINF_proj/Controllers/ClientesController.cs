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
    public class ClientesController : ApiController
    {
        //
        // GET: api/clientes/

        public IEnumerable<Lib_Primavera.Model.Cliente> Get()
        {
            return Lib_Primavera.ClienteGes.getUtilizadores();
        }


        // GET api/clientes/5    
        public Cliente Get(string id)
        {
            Lib_Primavera.Model.Cliente cliente = Lib_Primavera.ClienteGes.getInfoUtilizador(id);
            if (cliente == null)
            {
                throw new HttpResponseException(
                        Request.CreateResponse(HttpStatusCode.NotFound));
                
            }
            else
            {
                return cliente;
            }
        }

        public Lib_Primavera.Model.Cliente Get(string id, string id2)
        {
            return Lib_Primavera.ClienteGes.loginUtilizador(id, id2);
        }
    }
}
