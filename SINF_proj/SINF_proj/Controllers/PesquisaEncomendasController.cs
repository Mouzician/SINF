using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using SINF_proj.Lib_Primavera.Model;
using System.Text.RegularExpressions;

namespace SINF_proj.Controllers
{
    public class PesquisaEncomendasController : ApiController
    {
        //
        // GET: /pesquisaencomendas/
        public Lib_Primavera.Model.Encomenda Get(string id)
        {
            return Lib_Primavera.EncomendasGes.getPesquisaEncomenda(id);
        }

        public Lib_Primavera.Model.Encomenda Get(string id, string id2)
        {
            return Lib_Primavera.EncomendasGes.getPesquisaEncomenda(id, id2);
        }
    }
}
