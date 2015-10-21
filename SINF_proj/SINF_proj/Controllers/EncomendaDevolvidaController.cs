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
    public class EncomendaDevolvidaController : ApiController
    {
        public Lib_Primavera.Model.Encomenda Get(string id)
        {
            return Lib_Primavera.EncomendasGes.getEncomendaDevolvida(id);
        }
    }
}
