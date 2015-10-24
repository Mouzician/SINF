using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FirstREST.Lib_Primavera.Model
{
    public class Artigo
    {
        public string ID
        {
            get;
            set;
        }

        public string DescArtigo //descriçao do artigo
        {
            get;
            set;
        }

         public string Desconto 
        {
            get;
            set;
        }

        public string Familia
        {
            get;
            set;
        }

        public string Marca
        {
            get;
            set;
        }

        public string Modelo
        {
            get;
            set;
        }

        public string STKActual
        {
            get;
            set;
        }

        public string SubFamilia
        {
            get;
            set;
        }

        public string Preço //PCPadrao (table)
        {
            get;
            set;
        }

        public string[] imagens //imagens do artigo
        {
            get;
            set;
        }
    }
}