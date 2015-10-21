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
    public class EncomendasClientesController : ApiController
    {
        // GET api/encomendasclientes/idCliente/
        // Se nao colocar nenhuma quantidade, por defeito vai ser 10
        public IEnumerable<Lib_Primavera.Model.Encomenda> Get(string id)
        {
            return Lib_Primavera.EncomendasGes.getEncomendas(id, 10);
        }

        // GET api/encomendasclientes/idCliente/quantidade    
        public IEnumerable<Lib_Primavera.Model.Encomenda> Get(string id, string id2)
        {
            int quantidade = Convert.ToInt32(id2);
            return Lib_Primavera.EncomendasGes.getEncomendas(id, quantidade);
        }

        // GET api/encomendasclientes/idCliente/dataInicio/dataFim    
        public IEnumerable<Lib_Primavera.Model.Encomenda> Get(string id, string id2, string id3)
        {
            return Lib_Primavera.EncomendasGes.getEncomendasNoIntervalo(id, id2, id3);
        }

    }
}
