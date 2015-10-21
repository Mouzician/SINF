using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SINF_proj.Lib_Primavera.Model
{
    public class Artigo
    {
        public string idPrimaveraArtigo
        {
            get;
            set;
        }

        public string idPrimaveraArtigo2
        {
            get;
            set;
        }

        public string codArtigo
        {
            get;
            set;
        }

        public string descricaoArtigo
        {
            get;
            set;
        }

        public double quantidade
        {
            get;
            set;
        }

        public string unidade
        {
            get;
            set;
        }

        public double desconto
        {
            get;
            set;
        }

        public double precoUnitario
        {
            get;
            set;
        }

        public double totalILiquido
        {
            get;
            set;
        }

        public double totalLiquido
        {
            get;
            set;
        }

        public string estadoEntrega 
        { 
            get; 
            set; 
        }

        public string ratingEntrega
        {
            get;
            set;
        }

        public bool devolucao
        {
            get;
            set;
        }
    }
}
