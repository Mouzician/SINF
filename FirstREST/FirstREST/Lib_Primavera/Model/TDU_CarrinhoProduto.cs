using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FirstREST.Lib_Primavera.Model
{
    public class TDU_CarrinhoProduto //TDU_CarrinhoProduto
    {

        public string CDU_idCarrinhoProduto //CDU_idCarrinhoCompras
        {
            get;
            set;
        }

        public string CDU_idCarrinho //CDU_idCarrinhoCompras
        {
            get;
            set;
        }


        public string CDU_idProduto //CDU_idProduto
        {
            get;
            set;
        }

        public string CDU_Quantidade //nao precisa para remover, so para inserir
        {
            get;
            set;
        }
    }
}