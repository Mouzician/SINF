using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SINF_proj.Lib_Primavera.Model
{
    public class Encomenda
    {
        public string idPrimaveraEncomenda
        {
            get;
            set;
        }

        public string codCliente
        {
            get;
            set;
        }

        public int codEncomenda
        {
            get;
            set;
        }

        public DateTime DataInicio
        {
            get;
            set;
        }

        public DateTime DataFim
        {
            get;
            set;
        }

        public DateTime DataVencimento
        {
            get;
            set;
        }

        public double totalMerc
        {
            get;
            set;
        }

        public string serie
        {
            get;
            set;
        }

        public string estadoEntrega
        {
            get;
            set;
        }

        public float perEntrega
        {
            get;
            set;
        }

        public bool anulado
        {
            get;
            set;
        }

        public List<Tuple<float, string>> missArtigos
        {
            get;
            set;
        }

        public List<Artigo> ListaArtigos
        { 
            get; 
            set; 
        }

        public List<int> ListaGR
        {
            get;
            set;
        }
    }
}
