using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Interop.ErpBS800;
using Interop.StdPlatBS800;
using Interop.StdBE800;
using Interop.GcpBE800;
using ADODB;
using Interop.IGcpBS800;
using System.Text.RegularExpressions;


namespace SINF_proj.Lib_Primavera
{
    public class ArtigosGes
    {
        public static List<Model.Artigo> Artigo_Lista()
        {
            ErpBS objMotor = new ErpBS();
            List<Model.Artigo> retorno = new List<Model.Artigo>();
            StdBELista objListCab;

            if (PriEngine.InitializeCompany("BELAFLOR", "", "") == true)
            {
                objListCab = PriEngine.Engine.Consulta("SELECT DISTINCT Artigo.Descricao, Artigo.Artigo, PrecUnit from LinhasDoc, Artigo where LinhasDoc.Descricao = Artigo.Descricao");

                while(!objListCab.NoFim())
                {
                    Model.Artigo aux = new Model.Artigo();

                    aux.descricaoArtigo = objListCab.Valor("Descricao");
                    aux.codArtigo = objListCab.Valor("Artigo");
                    aux.precoUnitario = objListCab.Valor("PrecUnit");

                    if (aux.precoUnitario > 0)
                    {
                        retorno.Add(aux);
                    }
                    objListCab.Seguinte();
                }

                return retorno;
            }

            return null;
        }
    }
}