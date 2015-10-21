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
    public class EncomendasGes
    {
        # region Encomendas

        private static List<int> codEncomendasDevolvidas = new List<int>();

        public static List<Model.Encomenda> Encomendas_Lista()
        {
            ErpBS objMotor = new ErpBS();
            StdBELista objListCab;
            StdBELista objListLin;
            StdBELista objListStatus;
            StdBELista objListCabecStatus;
            Model.Encomenda dv = new Model.Encomenda();
            List<Model.Encomenda> listdv = new List<Model.Encomenda>();
            Model.Artigo lindv = new Model.Artigo();
            List<Model.Artigo> listlindv = new List<Model.Artigo>();
            DateTime auxDate = Convert.ToDateTime("1900-01-01");
            bool findP = false;
            bool findT = false;
            float entregues = 0;
            float totais = 0;
            List<Tuple<float, string>> missArtigosAux;

            if (PriEngine.InitializeCompany("BELAFLOR", "", "") == true)
            {
                objListCab = PriEngine.Engine.Consulta("SELECT id, Entidade, Data, DataDescarga, NumDoc, TotalMerc, TotalIva, TotalDesc, Serie From CabecDoc where TipoDoc='ECL' order by Data DESC");

                while (!objListCab.NoFim())
                {
                    findP = false;
                    findT = false;

                    dv = new Model.Encomenda();
                    dv.idPrimaveraEncomenda = objListCab.Valor("id");
                    dv.codCliente = objListCab.Valor("Entidade");
                    dv.codEncomenda = objListCab.Valor("NumDoc");
                    dv.DataInicio = Convert.ToDateTime(objListCab.Valor("Data"));
                    dv.DataVencimento = Convert.ToDateTime("1900-01-01");

                    objListCabecStatus = PriEngine.Engine.Consulta("SELECT Anulado from CabecDocStatus where IdCabecDoc='" + dv.idPrimaveraEncomenda + "'");
                    dv.anulado = objListCabecStatus.Valor("Anulado");

                    objListLin = PriEngine.Engine.Consulta("SELECT id, idCabecDoc, Artigo, Descricao, Quantidade, Unidade, PrecUnit, Desconto1, TotalILiquido, PrecoLiquido, Devolucao from LinhasDoc where IdCabecDoc='" + dv.idPrimaveraEncomenda + "' order By NumLinha");
                    listlindv = new List<Model.Artigo>();
                    missArtigosAux = new List<Tuple<float, string>>();

                    while (!objListLin.NoFim())
                    {
                        lindv = new Model.Artigo();
                        lindv.idPrimaveraArtigo = objListLin.Valor("id");
                        lindv.idPrimaveraArtigo2 = objListLin.Valor("idCabecDoc");
                        lindv.codArtigo = objListLin.Valor("Artigo");
                        lindv.descricaoArtigo = objListLin.Valor("Descricao");
                        lindv.quantidade = objListLin.Valor("Quantidade");
                        lindv.unidade = objListLin.Valor("Unidade");
                        lindv.desconto = objListLin.Valor("Desconto1");
                        lindv.precoUnitario = objListLin.Valor("PrecUnit");
                        lindv.totalILiquido = objListLin.Valor("TotalILiquido");
                        lindv.totalLiquido = objListLin.Valor("PrecoLiquido");
                        lindv.devolucao = objListLin.Valor("Devolucao");

                        objListStatus = PriEngine.Engine.Consulta("SELECT EstadoTrans, QuantReserv, QuantTrans, Fechado FROM LinhasDocStatus WHERE IdLinhasDoc = '" + lindv.idPrimaveraArtigo + "' ");
                        lindv.estadoEntrega = objListStatus.Valor("EstadoTrans");

                        if (lindv.estadoEntrega.Equals("P"))
                        {
                            findP = true;
                        }
                        else if (lindv.estadoEntrega.Equals("T"))
                        {
                            findT = true;
                        }
                        
                        if (objListStatus.Valor("QuantTrans") > 0)
                        {
                            findT = true;
                        }


                        entregues += (float)objListStatus.Valor("QuantTrans");
                        totais += (float)objListLin.Valor("Quantidade");


                        Tuple<float, string> aux;

                        if ((float)objListLin.Valor("Quantidade") - (float)objListStatus.Valor("QuantTrans") > 0)
                        {
                            aux = new Tuple<float, string>((float)objListLin.Valor("Quantidade") - (float)objListStatus.Valor("QuantTrans"), objListLin.Valor("Descricao"));
                            missArtigosAux.Add(aux);
                        }

                        lindv.ratingEntrega = Convert.ToString((float)objListStatus.Valor("QuantTrans")) + '/' + Convert.ToString((float)objListLin.Valor("Quantidade"));

                        listlindv.Add(lindv);
                        objListLin.Seguinte();
                    }

                    dv.totalMerc = objListCab.Valor("TotalMerc") + objListCab.Valor("TotalIva") - objListCab.Valor("TotalDesc");
                    dv.ListaArtigos = listlindv;
                    dv.ListaGR = new List<int>();

                    if (dv.anulado == true)
                    {
                        dv.estadoEntrega = "anulada";
                        dv.DataFim = Convert.ToDateTime("1900-01-01");
                        dv.DataVencimento = Convert.ToDateTime("1900-01-01");
                    }
                    else if (findP == false && findT == true)
                    {
                        dv.estadoEntrega = "nao facturada";
                        dv.DataFim = Convert.ToDateTime("1900-01-01");
                        dv.DataVencimento = Convert.ToDateTime("1900-01-01");

                        StdBELista objIdCabecDocGR = PriEngine.Engine.Consulta("SELECT Descricao, idCabecDoc FROM LinhasDoc");
                        List<string> auxList = new List<string>();

                        while (!objIdCabecDocGR.NoFim())
                        {
                            string aux = objIdCabecDocGR.Valor("Descricao");

                            if (aux.Contains("ECL "))
                            {
                                String[] aux_query = Regex.Split(aux, "/");
                                String[] aux_query2 = Regex.Split(aux_query[0], "º");

                                if (dv.codEncomenda == Convert.ToInt16(aux_query2[1]))
                                {
                                    if (!auxList.Contains(objIdCabecDocGR.Valor("idCabecDoc")))
                                    {
                                        auxList.Add(objIdCabecDocGR.Valor("idCabecDoc"));
                                    }
                                }
                            }

                            objIdCabecDocGR.Seguinte();
                        }

                        bool gr = false;
                        bool fac = false;

                        for (int j = 0; j < auxList.Count(); j++)
                        {
                            string auxStr = auxList[j];

                            StdBELista objGR = PriEngine.Engine.Consulta("SELECT NumDoc, TipoDoc FROM CabecDoc WHERE id='" + auxStr + "'");
                            dv.ListaGR.Add(objGR.Valor("NumDoc"));

                            if (objGR.Valor("TipoDoc").Equals("GR") || objGR.Valor("TipoDoc").Equals("GT"))
                            {
                                gr = true;
                            }
                            else if (objGR.Valor("TipoDoc").Equals("FA"))
                            {
                                fac = true;
                            }
                        }

                        // Ate aqui estao encontrados os id's das guias de remessa referentes a cada encomenda
                        // Para agora comecar a identificar as facturas e ver se batem certo

                        if (fac == false)
                        {
                            auxList.Clear();
                        }

                        DateTime dF = Convert.ToDateTime("1900-01-01");
                        DateTime dV = Convert.ToDateTime("1900-01-01");

                        for (int i = 0; i < dv.ListaGR.Count; i++)
                        {
                            StdBELista objIdCabecDocFA = PriEngine.Engine.Consulta("SELECT Descricao, idCabecDoc FROM LinhasDoc");

                            while (!objIdCabecDocFA.NoFim())
                            {
                                string auxFA = objIdCabecDocFA.Valor("Descricao");

                                if (auxFA.Contains("GR ") || auxFA.Contains("GT "))
                                {
                                    String[] aux_query = Regex.Split(auxFA, "/");
                                    String[] aux_query2 = Regex.Split(aux_query[0], "º");

                                    if (dv.ListaGR[i] == Convert.ToInt16(aux_query2[1]))
                                    {
                                        if (!auxList.Contains(objIdCabecDocFA.Valor("idCabecDoc")))
                                        {
                                            auxList.Add(objIdCabecDocFA.Valor("idCabecDoc"));
                                        }
                                    }
                                }

                                objIdCabecDocFA.Seguinte();
                            }
                        }

                        List<Model.Artigo> auxArt = new List<Model.Artigo>();
                        List<double> quantidades = new List<double>();
                        List<int> codEncomendas = new List<int>();

                        for (int i = 0; i < dv.ListaArtigos.Count; i++)
                        {
                            Model.Artigo auxArtigo = new Model.Artigo();
                            auxArtigo = dv.ListaArtigos[i];
                            
                            auxArt.Add(auxArtigo);
                            quantidades.Add(auxArtigo.quantidade);
                        }

                        for (int i = 0; i < auxList.Count; i++)
                        {
                            StdBELista objFA = PriEngine.Engine.Consulta("SELECT NumDoc, Data, DataVencimento FROM CabecDoc WHERE id='"+ auxList[i] +"'");
                            StdBELista objLinFA = PriEngine.Engine.Consulta("SELECT Descricao, Quantidade FROM LinhasDoc WHERE idCabecDoc='" + auxList[i] + "'");

                            while (!objLinFA.NoFim())
                            {
                                string desAux = objLinFA.Valor("Descricao");

                                if (objLinFA.Valor("Quantidade") > 0)
                                {
                                    for (int k = 0; k < auxArt.Count; k++)
                                    {
                                        if (auxArt[k].descricaoArtigo.Equals(desAux))
                                        {
                                            if (quantidades[k] - objLinFA.Valor("Quantidade") == 0)
                                            {
                                                auxArt.RemoveAt(k);
                                                k--;
                                            }
                                            else if (quantidades[k] - objLinFA.Valor("Quantidade") > 0)
                                            {
                                                quantidades[k] = quantidades[k] - objLinFA.Valor("Quantidade");
                                            }
                                        }
                                    }
                                }

                                objLinFA.Seguinte();
                            }


                            if (dV < objFA.Valor("DataVencimento"))
                            {
                                dV = objFA.Valor("DataVencimento");
                            }

                            if (dF < objFA.Valor("Data"))
                            {
                                dF = objFA.Valor("Data");
                            }

                            codEncomendas.Add(objFA.Valor("NumDoc"));
                        }

                        dv.DataFim = dF;

                        if (auxArt.Count() == 0)
                        {
                            dv.DataVencimento = dV;

                            if (dV <= DateTime.Today)
                            {
                                dv.estadoEntrega = "paga";
                            }
                            else
                            {
                                dv.estadoEntrega = "nao paga";
                            }
                        }
                        else
                        {
                            dv.estadoEntrega = "nao facturada";
                        }
                    }
                    else if (findP == true && findT == true)
                    {
                        dv.estadoEntrega = "a entregar";
                        dv.DataFim = Convert.ToDateTime("1900-01-01");
                    }
                    else if (findP = true && findT == false)
                    {
                        dv.estadoEntrega = "a processar";
                        dv.DataFim = Convert.ToDateTime("1900-01-01");
                    }

                    if (entregues == 0 || totais == 0)
                    {
                        dv.perEntrega = 0;
                    }
                    else
                    {
                        dv.perEntrega = (entregues / totais) * 100;
                    }
                    dv.serie = objListCab.Valor("Serie");
                    dv.missArtigos = missArtigosAux;
                    listdv.Add(dv);
                    objListCab.Seguinte();
                    entregues = 0;
                    totais = 0;

                }

                return listdv;
            }
            else
            {
                return null;
            }
        }

        public static List<Model.Encomenda> getEncomendasDevolvidas()
        {
            List<Model.Encomenda> encomendas = Encomendas_Lista();
            List<Model.Encomenda> retorno = new List<Model.Encomenda>();

            for (int k = 0; k < encomendas.Count; k++)
            {
                StdBELista objIdCabecDocGR = PriEngine.Engine.Consulta("SELECT Descricao, idCabecDoc FROM LinhasDoc");
                List<string> auxList = new List<string>();

                while (!objIdCabecDocGR.NoFim())
                {
                    string aux = objIdCabecDocGR.Valor("Descricao");

                    if (aux.Contains("ECL "))
                    {
                        String[] aux_query = Regex.Split(aux, "/");
                        String[] aux_query2 = Regex.Split(aux_query[0], "º");

                        if (encomendas[k].codEncomenda == Convert.ToInt16(aux_query2[1]))
                        {
                            if (!auxList.Contains(objIdCabecDocGR.Valor("idCabecDoc")))
                            {
                                auxList.Add(objIdCabecDocGR.Valor("idCabecDoc"));
                            }
                        }
                    }

                    objIdCabecDocGR.Seguinte();
                }

                bool gr = false;
                bool fac = false;
                encomendas[k].ListaGR.Clear();

                for (int j = 0; j < auxList.Count(); j++)
                {
                    string auxStr = auxList[j];

                    StdBELista objGR = PriEngine.Engine.Consulta("SELECT NumDoc, TipoDoc FROM CabecDoc WHERE id='" + auxStr + "'");
                    encomendas[k].ListaGR.Add(objGR.Valor("NumDoc"));

                    if (objGR.Valor("TipoDoc").Equals("GR") || objGR.Valor("TipoDoc").Equals("GT"))
                    {
                        gr = true;
                    }
                    else if (objGR.Valor("TipoDoc").Equals("FA"))
                    {
                        fac = true;
                    }
                }

                // Ate aqui estao encontrados os id's das guias de remessa referentes a cada encomenda
                // Para agora comecar a identificar as facturas e ver se batem certo

                if (fac == false)
                {
                    auxList.Clear();
                }

                DateTime dF = Convert.ToDateTime("1900-01-01");
                DateTime dV = Convert.ToDateTime("1900-01-01");

                for (int i = 0; i < encomendas[k].ListaGR.Count; i++)
                {
                    StdBELista objIdCabecDocFA = PriEngine.Engine.Consulta("SELECT Descricao, idCabecDoc FROM LinhasDoc");

                    while (!objIdCabecDocFA.NoFim())
                    {
                        string auxFA = objIdCabecDocFA.Valor("Descricao");

                        if (auxFA.Contains("GR ") || auxFA.Contains("GT "))
                        {
                            String[] aux_query = Regex.Split(auxFA, "/");
                            String[] aux_query2 = Regex.Split(aux_query[0], "º");

                            if (encomendas[k].ListaGR[i] == Convert.ToInt16(aux_query2[1]))
                            {
                                if (!auxList.Contains(objIdCabecDocFA.Valor("idCabecDoc")))
                                {
                                    auxList.Add(objIdCabecDocFA.Valor("idCabecDoc"));
                                }
                            }
                        }

                        objIdCabecDocFA.Seguinte();
                    }
                }

                List<int> codFacturas = new List<int>();

                for (int i = 0; i < auxList.Count; i++)
                {
                    StdBELista objFA = PriEngine.Engine.Consulta("SELECT NumDoc, Data, DataVencimento FROM CabecDoc WHERE id='" + auxList[i] + "'");
                    codFacturas.Add(objFA.Valor("NumDoc"));
                }

                // Ver se existem devolucoes com base nas facturas
                StdBELista objIdCabecDocFAux = PriEngine.Engine.Consulta("SELECT Descricao, idCabecDoc FROM LinhasDoc");
                Model.Encomenda enc = encomendas[k];

                // Colocar as quantidades dos artigos a 0
                List<double> quantidades = new List<double>();
                for (int m = 0; m < enc.ListaArtigos.Count; m++)
                {
                    quantidades.Add(enc.ListaArtigos[m].quantidade);
                    enc.ListaArtigos[m].quantidade = 0;
                }

                bool exist = false;

                while (!objIdCabecDocFAux.NoFim())
                {
                    string aux = objIdCabecDocFAux.Valor("Descricao");

                    if (aux.Contains("FA "))
                    {
                        StdBELista objFAuxNC = PriEngine.Engine.Consulta("SELECT id, TipoDoc FROM CabecDoc where id='" + objIdCabecDocFAux.Valor("idCabecDoc") + "'");

                        if (objFAuxNC.Valor("TipoDoc") == "NC")
                        {
                            String[] aux_query = Regex.Split(aux, "/");
                            String[] aux_query2 = Regex.Split(aux_query[0], "º");

                            for (int l = 0; l < codFacturas.Count; l++)
                            {
                                if (codFacturas[l] == Convert.ToInt16(aux_query2[1]))
                                {
                                    string id = objIdCabecDocFAux.Valor("idCabecDoc");

                                    exist = true;

                                    StdBELista objFAux = PriEngine.Engine.Consulta("SELECT Descricao, Quantidade, idCabecDoc FROM LinhasDoc where idCabecDoc='" + id + "'");


                                    while (!objFAux.NoFim())
                                    {
                                        for (int g = 0; g < enc.ListaArtigos.Count; g++)
                                        {
                                            if (enc.ListaArtigos[g].descricaoArtigo.Equals(objFAux.Valor("Descricao")))
                                            {
                                                enc.estadoEntrega = "devolução de artigos";
                                                enc.ListaArtigos[g].quantidade = enc.ListaArtigos[g].quantidade + Math.Abs(objFAux.Valor("Quantidade"));
                                                enc.ListaArtigos[g].ratingEntrega = (quantidades[g] - enc.ListaArtigos[g].quantidade).ToString() + '/' + quantidades[g].ToString();
                                            }
                                        }

                                        objFAux.Seguinte();
                                    }
                                }
                            }
                        }
                    }

                    objIdCabecDocFAux.Seguinte();
                }

                if (exist == true)
                {
                    retorno.Add(enc);
                }
            }

            return retorno;
        }

        public static List<Model.Encomenda> getEncomendasAtivasDevolvidas()
        {
            List<Model.Encomenda> encomendasD = getEncomendasDevolvidas();
            List<Model.Encomenda> encomendas = Encomendas_Lista();
            List<Model.Encomenda> retorno = new List<Model.Encomenda>();

            for (int i = 0; i < encomendasD.Count; i++)
            {
                for (int j = 0; j < encomendas.Count; j++)
                {
                    if (encomendasD[i].codEncomenda.Equals(encomendas[j].codEncomenda) && (encomendas[j].estadoEntrega.Equals("a processar") || encomendas[j].estadoEntrega.Equals("a entregar")))
                    {
                        retorno.Add(encomendasD[i]);
                    }
                }
            }

            return retorno;
        }

        public static List<Model.Encomenda> getEncomendasDevolvidas(string codCliente)
        {
            List<Model.Encomenda> encomendas = getEncomendas(codCliente, 10000);
            List<Model.Encomenda> retorno = new List<Model.Encomenda>();

            for (int k = 0; k < encomendas.Count; k++)
            {
                StdBELista objIdCabecDocGR = PriEngine.Engine.Consulta("SELECT Descricao, idCabecDoc FROM LinhasDoc");
                List<string> auxList = new List<string>();

                while (!objIdCabecDocGR.NoFim())
                {
                    string aux = objIdCabecDocGR.Valor("Descricao");

                    if (aux.Contains("ECL "))
                    {
                        String[] aux_query = Regex.Split(aux, "/");
                        String[] aux_query2 = Regex.Split(aux_query[0], "º");

                        if (encomendas[k].codEncomenda == Convert.ToInt16(aux_query2[1]))
                        {
                            if (!auxList.Contains(objIdCabecDocGR.Valor("idCabecDoc")))
                            {
                                auxList.Add(objIdCabecDocGR.Valor("idCabecDoc"));
                            }
                        }
                    }

                    objIdCabecDocGR.Seguinte();
                }

                bool gr = false;
                bool fac = false;
                encomendas[k].ListaGR.Clear();

                for (int j = 0; j < auxList.Count(); j++)
                {
                    string auxStr = auxList[j];

                    StdBELista objGR = PriEngine.Engine.Consulta("SELECT NumDoc, TipoDoc FROM CabecDoc WHERE id='" + auxStr + "'");
                    encomendas[k].ListaGR.Add(objGR.Valor("NumDoc"));

                    if (objGR.Valor("TipoDoc").Equals("GR") || objGR.Valor("TipoDoc").Equals("GT"))
                    {
                        gr = true;
                    }
                    else if (objGR.Valor("TipoDoc").Equals("FA"))
                    {
                        fac = true;
                    }
                }

                // Ate aqui estao encontrados os id's das guias de remessa referentes a cada encomenda
                // Para agora comecar a identificar as facturas e ver se batem certo

                if (fac == false)
                {
                    auxList.Clear();
                }

                DateTime dF = Convert.ToDateTime("1900-01-01");
                DateTime dV = Convert.ToDateTime("1900-01-01");

                for (int i = 0; i < encomendas[k].ListaGR.Count; i++)
                {
                    StdBELista objIdCabecDocFA = PriEngine.Engine.Consulta("SELECT Descricao, idCabecDoc FROM LinhasDoc");

                    while (!objIdCabecDocFA.NoFim())
                    {
                        string auxFA = objIdCabecDocFA.Valor("Descricao");

                        if (auxFA.Contains("GR ") || auxFA.Contains("GT "))
                        {
                            String[] aux_query = Regex.Split(auxFA, "/");
                            String[] aux_query2 = Regex.Split(aux_query[0], "º");

                            if (encomendas[k].ListaGR[i] == Convert.ToInt16(aux_query2[1]))
                            {
                                if (!auxList.Contains(objIdCabecDocFA.Valor("idCabecDoc")))
                                {
                                    auxList.Add(objIdCabecDocFA.Valor("idCabecDoc"));
                                }
                            }
                        }

                        objIdCabecDocFA.Seguinte();
                    }
                }


                List<int> codFacturas = new List<int>();


                for (int i = 0; i < auxList.Count; i++)
                {
                    StdBELista objFA = PriEngine.Engine.Consulta("SELECT NumDoc, Data, DataVencimento FROM CabecDoc WHERE id='" + auxList[i] + "'");
                    codFacturas.Add(objFA.Valor("NumDoc"));
                }

                // Ver se existem devolucoes com base nas facturas
                StdBELista objIdCabecDocFAux = PriEngine.Engine.Consulta("SELECT Descricao, idCabecDoc FROM LinhasDoc");
                Model.Encomenda enc = encomendas[k];

                // Colocar as quantidades dos artigos a 0
                List<double> quantidades = new List<double>();
                for (int m = 0; m < enc.ListaArtigos.Count; m++)
                {
                    quantidades.Add(enc.ListaArtigos[m].quantidade);
                    enc.ListaArtigos[m].quantidade = 0;
                }

                bool exist = false;

                while (!objIdCabecDocFAux.NoFim())
                {
                    string aux = objIdCabecDocFAux.Valor("Descricao");

                    if (aux.Contains("FA "))
                    {
                        StdBELista objFAuxNC = PriEngine.Engine.Consulta("SELECT id, TipoDoc FROM CabecDoc where id='" + objIdCabecDocFAux.Valor("idCabecDoc") + "'");

                        if (objFAuxNC.Valor("TipoDoc") == "NC")
                        {
                            String[] aux_query = Regex.Split(aux, "/");
                            String[] aux_query2 = Regex.Split(aux_query[0], "º");

                            for (int l = 0; l < codFacturas.Count; l++)
                            {
                                if (codFacturas[l] == Convert.ToInt16(aux_query2[1]))
                                {
                                    string id = objIdCabecDocFAux.Valor("idCabecDoc");
                                    exist = true;

                                    StdBELista objFAux = PriEngine.Engine.Consulta("SELECT Descricao, Quantidade, idCabecDoc FROM LinhasDoc where idCabecDoc='" + id + "'");

                                    while (!objFAux.NoFim())
                                    {
                                        for (int g = 0; g < enc.ListaArtigos.Count; g++)
                                        {
                                            if (enc.ListaArtigos[g].descricaoArtigo.Equals(objFAux.Valor("Descricao")))
                                            {
                                                enc.estadoEntrega = "devolução de artigos";
                                                enc.ListaArtigos[g].quantidade = enc.ListaArtigos[g].quantidade + Math.Abs(objFAux.Valor("Quantidade"));
                                                enc.ListaArtigos[g].ratingEntrega = (quantidades[g] - enc.ListaArtigos[g].quantidade).ToString() + '/' + quantidades[g].ToString();
                                            }
                                        }

                                        objFAux.Seguinte();
                                    }
                                }
                            }
                        }
                    }

                    objIdCabecDocFAux.Seguinte();
                }

                if (exist == true)
                {
                    retorno.Add(enc);
                }
            }

            return retorno;
        }

        public static List<Model.Encomenda> getEncomendasAtivasDevolvidas(string codCliente)
        {
            List<Model.Encomenda> encomendasD = getEncomendasDevolvidas(codCliente);
            List<Model.Encomenda> encomendas = getEncomendas(codCliente, 10000);
            List<Model.Encomenda> retorno = new List<Model.Encomenda>();

            for (int i = 0; i < encomendasD.Count; i++)
            {
                for (int j = 0; j < encomendas.Count; j++)
                {
                    if (encomendasD[i].codEncomenda.Equals(encomendas[j].codEncomenda) && (encomendas[j].estadoEntrega.Equals("a processar") || encomendas[j].estadoEntrega.Equals("a entregar")))
                    {
                        retorno.Add(encomendasD[i]);
                    }
                }
            }

            return retorno;
        }

        public static Model.Encomenda getEncomendaDevolvida(string codEncomenda)
        {
            List<Model.Encomenda> encomendas = getEncomendasDevolvidas();
            Model.Encomenda retorno = new Model.Encomenda();

            for (int i = 0; i < encomendas.Count; i++)
            {
                if(encomendas[i].codEncomenda.Equals(int.Parse(codEncomenda)))
                {
                    retorno = encomendas[i];
                    break;
                }
            }

            return retorno;
        }

        public static List<Model.Encomenda> getEncomendas(string codCliente, int quantidade)
        {
            List<Model.Encomenda> encomendas = Encomendas_Lista();
            List<Model.Encomenda> retorno = new List<Model.Encomenda>();
            int j = 0;

            for (int i = 0; i < encomendas.Count; i++)
            {
                if (j < quantidade)
                {
                    if (encomendas[i].codCliente.Equals(codCliente))
                    {
                        retorno.Add(encomendas[i]);
                    }
                }
                else
                {
                    break;
                }
            }

            return retorno;

        }

        public static Lib_Primavera.Model.Encomenda getEncomenda(string codEncomenda)
        {
            List<Model.Encomenda> encomendas = Encomendas_Lista();
            Model.Encomenda retorno = new Model.Encomenda();

            for (int i = 0; i < encomendas.Count; i++)
            {
                if (encomendas[i].codEncomenda.Equals(int.Parse(codEncomenda)))
                {
                    retorno = encomendas[i];
                    break;
                }
            }

            return retorno;
        }

        public static Model.RespostaErro insertEncomenda(Model.Encomenda dv)
        {
            Lib_Primavera.Model.RespostaErro erro = new Model.RespostaErro();
            GcpBEDocumentoVenda myEnc = new GcpBEDocumentoVenda();
            GcpBELinhaDocumentoVenda myLin = new GcpBELinhaDocumentoVenda();
            GcpBELinhasDocumentoVenda myLinhas = new
            GcpBELinhasDocumentoVenda();
            PreencheRelacaoVendas rl = new PreencheRelacaoVendas();
            List<Model.Artigo> lstlindv = new List<Model.Artigo>();
            
            try
            {
                if (PriEngine.InitializeCompany("BELAFLOR", "", "") == true)
                {
                    // Atribui valores ao cabecalho do doc
                    //myEnc.set_DataDoc(dv.Data);
                    myEnc.set_Entidade(dv.codCliente);
                    myEnc.set_Serie(dv.serie);
                    myEnc.set_Tipodoc("ECL");
                    myEnc.set_TipoEntidade("C");
                    // Linhas do documento para a lista de linhas
                    lstlindv = dv.ListaArtigos;
                    PriEngine.Engine.Comercial.Vendas.PreencheDadosRelacionados(myEnc, rl);

                    foreach (Model.Artigo lin in lstlindv)
                    {
                        {
                            PriEngine.Engine.Comercial.Vendas.AdicionaLinha(myEnc, lin.codArtigo, lin.quantidade, "","",lin.precoUnitario, lin.desconto);
                        }
                    }

                    PriEngine.Engine.IniciaTransaccao();
                    PriEngine.Engine.Comercial.Vendas.Actualiza(myEnc,"Teste");
                    PriEngine.Engine.TerminaTransaccao();
                    erro.Erro = 0;
                    erro.Descricao = "Sucesso";
                    return erro;
                }
                else
                {
                    erro.Erro = 1;
                    erro.Descricao = "Erro ao abrir empresa";
                    return erro;
                }
            }
            catch (Exception ex)
            {
                PriEngine.Engine.DesfazTransaccao();
                erro.Erro = 1;
                erro.Descricao = ex.Message;
                return erro;
            }
        }

        public static List<Model.Artigo> getHistorico(string codCliente)
        {
            Dictionary<string, int> retornoAux = new Dictionary<string,int>();
            List<Model.Artigo> retorno = new List<Model.Artigo>();
            List<Model.Encomenda> encomendas = getEncomendas(codCliente, 10000);

            for (int i = 0; i < encomendas.Count; i++)
            {
                for (int j = 0; j < encomendas[i].ListaArtigos.Count; j++)
                {
                    if (retornoAux.ContainsKey(encomendas[i].ListaArtigos[j].descricaoArtigo))
                    {
                        int value = 0;
                        value = retornoAux[encomendas[i].ListaArtigos[j].descricaoArtigo];

                        retornoAux.Remove(encomendas[i].ListaArtigos[j].descricaoArtigo);
                        retornoAux.Add(encomendas[i].ListaArtigos[j].descricaoArtigo, value + 1);
                    }
                    else
                    {
                        retornoAux.Add(encomendas[i].ListaArtigos[j].descricaoArtigo, 1);
                    }
                }
            }

            // Reverse sort.
            // ... Can be looped over in the same way as above.
            var items = from pair in retornoAux
                    orderby pair.Value descending
                    select pair;

            // Display results.
            List<Model.Artigo> auxList = new List<Model.Artigo>();
            auxList.AddRange(ArtigosGes.Artigo_Lista());
            int cont = 0;
            int max = 14;

            if (retornoAux.Count < max)
            {
                max = retornoAux.Count;
            }

            foreach (KeyValuePair<string, int> pair in items)
            {
                if (cont < max)
                {
                    for (int i = 0; i < auxList.Count; i++)
                    {
                        if (pair.Key.Equals(auxList[i].descricaoArtigo))
                        {
                            retorno.Add(auxList[i]);
                        }
                    }
                }

                cont++;
            }

            return retorno;
        }

        public static List<Model.Encomenda> getEncomendasEntregues()
        {
            List<Model.Encomenda> encomendas = Encomendas_Lista();
            List<Model.Encomenda> retorno = new List<Model.Encomenda>();

            for (int i = 0; i < encomendas.Count; i++)
            {
                if (encomendas[i].estadoEntrega.Equals("nao facturada") || encomendas[i].estadoEntrega.Equals("nao paga") || encomendas[i].estadoEntrega.Equals("paga"))
                {
                    retorno.Add(encomendas[i]);
                }
            }

            return retorno;
        }

        public static List<Model.Encomenda> getEncomendasEntregues(string codCliente)
        {
            List<Model.Encomenda> encomendas = getEncomendas(codCliente, 100000);
            List<Model.Encomenda> retorno = new List<Model.Encomenda>();

            for (int i = 0; i < encomendas.Count; i++)
            {
                if (encomendas[i].estadoEntrega.Equals("nao facturada") || encomendas[i].estadoEntrega.Equals("nao paga") || encomendas[i].estadoEntrega.Equals("paga"))
                {
                    retorno.Add(encomendas[i]);
                }
            }

            return retorno;
        }

        public static List<Model.Encomenda> getEncomendasAnuladas()
        {
            List<Model.Encomenda> encomendas = Encomendas_Lista();
            List<Model.Encomenda> retorno = new List<Model.Encomenda>();

            for (int i = 0; i < encomendas.Count; i++)
            {
                if (encomendas[i].estadoEntrega.Equals("anulada"))
                {
                    retorno.Add(encomendas[i]);
                }
            }

            return retorno;
        }

        public static List<Model.Encomenda> getEncomendasAnuladas(string codCliente)
        {
            List<Model.Encomenda> encomendas = getEncomendas(codCliente, 100000);
            List<Model.Encomenda> retorno = new List<Model.Encomenda>();

            for (int i = 0; i < encomendas.Count; i++)
            {
                if (encomendas[i].estadoEntrega.Equals("anulada"))
                {
                    retorno.Add(encomendas[i]);
                }
            }

            return retorno;
        }

        public static List<Model.Encomenda> getEncomendasProcessamento()
        {
            List<Model.Encomenda> encomendas = Encomendas_Lista();
            List<Model.Encomenda> retorno = new List<Model.Encomenda>();

            for (int i = 0; i < encomendas.Count; i++)
            {
                if (encomendas[i].estadoEntrega.Equals("a processar"))
                {
                    retorno.Add(encomendas[i]);
                }
            }

            return retorno;
        }

        public static List<Model.Encomenda> getEncomendasProcessamento(string codCliente)
        {
            List<Model.Encomenda> encomendas = getEncomendas(codCliente, 100000);
            List<Model.Encomenda> retorno = new List<Model.Encomenda>();

            for (int i = 0; i < encomendas.Count; i++)
            {
                if (encomendas[i].estadoEntrega.Equals("a processar"))
                {
                    retorno.Add(encomendas[i]);
                }
            }

            return retorno;
        }

        public static List<Model.Encomenda> getEncomendasAEntregar()
        {
            List<Model.Encomenda> encomendas = Encomendas_Lista();
            List<Model.Encomenda> retorno = new List<Model.Encomenda>();

            for (int i = 0; i < encomendas.Count; i++)
            {
                if (encomendas[i].estadoEntrega.Equals("a entregar"))
                {
                    retorno.Add(encomendas[i]);
                }
            }

            return retorno;
        }

        public static List<Model.Encomenda> getEncomendasAEntregar(string codCliente)
        {
            List<Model.Encomenda> encomendas = getEncomendas(codCliente, 100000);
            List<Model.Encomenda> retorno = new List<Model.Encomenda>();

            for (int i = 0; i < encomendas.Count; i++)
            {
                if (encomendas[i].estadoEntrega.Equals("a entregar"))
                {
                    retorno.Add(encomendas[i]);
                }
            }

            return retorno;
        }

        public static List<Model.Encomenda> getEncomendasPagas()
        {
            List<Model.Encomenda> encomendas = Encomendas_Lista();
            List<Model.Encomenda> retorno = new List<Model.Encomenda>();

            for (int i = 0; i < encomendas.Count; i++)
            {
                if (encomendas[i].estadoEntrega.Equals("paga"))
                {
                    retorno.Add(encomendas[i]);
                }
            }

            return retorno;
        }

        public static List<Model.Encomenda> getEncomendasPagas(string codCliente)
        {
            List<Model.Encomenda> encomendas = getEncomendas(codCliente, 100000);
            List<Model.Encomenda> retorno = new List<Model.Encomenda>();

            for (int i = 0; i < encomendas.Count; i++)
            {
                if (encomendas[i].estadoEntrega.Equals("paga"))
                {
                    retorno.Add(encomendas[i]);
                }
            }

            return retorno;
        }

        public static List<Model.Encomenda> getEncomendasNaoPagas()
        {
            List<Model.Encomenda> encomendas = Encomendas_Lista();
            List<Model.Encomenda> retorno = new List<Model.Encomenda>();

            for (int i = 0; i < encomendas.Count; i++)
            {
                if (encomendas[i].estadoEntrega.Equals("nao paga"))
                {
                    retorno.Add(encomendas[i]);
                }
            }

            return retorno;
        }

        public static List<Model.Encomenda> getEncomendasNaoPagas(string codCliente)
        {
            List<Model.Encomenda> encomendas = getEncomendas(codCliente, 100000);
            List<Model.Encomenda> retorno = new List<Model.Encomenda>();

            for (int i = 0; i < encomendas.Count; i++)
            {
                if (encomendas[i].estadoEntrega.Equals("nao paga"))
                {
                    retorno.Add(encomendas[i]);
                }
            }

            return retorno;
        }

        public static List<Model.Encomenda> getEncomendasFacturadas()
        {
            List<Model.Encomenda> encomendas = Encomendas_Lista();
            List<Model.Encomenda> retorno = new List<Model.Encomenda>();

            for (int i = 0; i < encomendas.Count; i++)
            {
                if (encomendas[i].estadoEntrega.Equals("paga") || encomendas[i].estadoEntrega.Equals("nao paga"))
                {
                    retorno.Add(encomendas[i]);
                }
            }

            return retorno;
        }

        public static List<Model.Encomenda> getEncomendasFacturadas(string codCliente)
        {
            List<Model.Encomenda> encomendas = getEncomendas(codCliente, 100000);
            List<Model.Encomenda> retorno = new List<Model.Encomenda>();

            for (int i = 0; i < encomendas.Count; i++)
            {
                if (encomendas[i].estadoEntrega.Equals("paga") || encomendas[i].estadoEntrega.Equals("nao paga"))
                {
                    retorno.Add(encomendas[i]);
                }
            }

            return retorno;
        }

        public static List<Model.Encomenda> getEncomendasNaoFacturadas()
        {
            List<Model.Encomenda> encomendas = Encomendas_Lista();
            List<Model.Encomenda> retorno = new List<Model.Encomenda>();

            for (int i = 0; i < encomendas.Count; i++)
            {
                if (encomendas[i].estadoEntrega.Equals("nao facturada"))
                {
                    retorno.Add(encomendas[i]);
                }
            }

            return retorno;
        }

        public static List<Model.Encomenda> getEncomendasNaoFacturadas(string codCliente)
        {
            List<Model.Encomenda> encomendas = getEncomendas(codCliente, 100000);
            List<Model.Encomenda> retorno = new List<Model.Encomenda>();

            for (int i = 0; i < encomendas.Count; i++)
            {
                if (encomendas[i].estadoEntrega.Equals("nao facturada"))
                {
                    retorno.Add(encomendas[i]);
                }
            }

            return retorno;
        }

        public static List<Model.Encomenda> getEncomendasNoIntervalo(string codCliente, string dataIni, string dataFim)
        {
            List<Model.Encomenda> encomendas = getEncomendasPagas(codCliente);
            List<Model.Encomenda> retorno = new List<Model.Encomenda>();

            DateTime dI = Convert.ToDateTime(dataIni);
            DateTime dF = Convert.ToDateTime(dataFim);

            for (int i = 0; i < encomendas.Count(); i++)
            {
                if (encomendas.ElementAt(i).DataFim <= dF && encomendas.ElementAt(i).DataInicio >= dI)
                {
                    retorno.Add(encomendas.ElementAt(i));
                }
            }

            return retorno;
        }

        public static List<Model.Encomenda> getEncomendasNoIntervalo(string dataIni, string dataFim)
        {
            List<Model.Encomenda> encomendas = getEncomendasPagas();
            List<Model.Encomenda> retorno = new List<Model.Encomenda>();

            DateTime dI = Convert.ToDateTime(dataIni);
            DateTime dF = Convert.ToDateTime(dataFim);

            for (int i = 0; i < encomendas.Count(); i++)
            {
                if (encomendas.ElementAt(i).DataFim <= dF && encomendas.ElementAt(i).DataInicio >= dI)
                {
                    retorno.Add(encomendas.ElementAt(i));
                }
            }

            return retorno;
        }

        public static Model.Encomenda getPesquisaEncomenda(string codEncomenda)
        {
            return getEncomenda(codEncomenda);
        }

        public static Model.Encomenda getPesquisaEncomenda(string codCliente, string codEncomenda)
        {
            List<Model.Encomenda> encomendas = getEncomendas(codCliente, 1000);
            Model.Encomenda retorno = null;

            for (int i = 0; i < encomendas.Count(); i++)
            {
                if (encomendas[i].codEncomenda.Equals(int.Parse(codEncomenda)))
                {
                    retorno = encomendas[i];
                    break;
                }
            }

            return retorno;
        }

        public static List<Model.Encomenda> getPesquisaAvanEncomenda(string codCliente, string queryAux)
        {
            List<Model.Encomenda> encomendas = getEncomendas(codCliente, 1000);
            List<Model.Encomenda> retorno = new List<Model.Encomenda>();

            String query = nameProduto(queryAux);
            String[] aux_query = Regex.Split(query, "___");

            String id_encomenda = aux_query[0];
            String dInit = aux_query[1];
            String dFinal = aux_query[2];
            String nome_produto = aux_query[3];
            String preco = aux_query[4];
            String morada = aux_query[5];
            int quantidade = 0;
            String state = aux_query[7];

            if (aux_query[6].Equals("none"))
            {
                quantidade = 9999;
            }
            else
            {
                quantidade = int.Parse(aux_query[6]);
            }

            List<Model.Encomenda> aux_enc = selectIdEncomenda(encomendas, id_encomenda);
            List<Model.Encomenda> aux_dateI = selectDataInicio(dInit, aux_enc);
            List<Model.Encomenda> aux_dateF = selectDataFim(dFinal, aux_dateI);
            List<Model.Encomenda> aux_nameP = selectNomeProduto(nome_produto, aux_dateF);
            List<Model.Encomenda> aux_preco = selectPreco(preco, aux_nameP);
            List<Model.Encomenda> aux_state = selectState(state, aux_preco);

            for (int i = 0; i < aux_state.Count; i++)
            {
                if (i == quantidade)
                {
                    break;
                }
                else if (i < quantidade)
                {
                    retorno.Add(aux_state[i]);
                }
            }

            return retorno;
        }


        public static List<Model.Encomenda> getPesquisaAvanEncomenda(string queryAux)
        {
            List<Model.Encomenda> encomendas = Encomendas_Lista();
            List<Model.Encomenda> retorno = new List<Model.Encomenda>();

            String query = nameProduto(queryAux);
            String[] aux_query = Regex.Split(query, "___");

            String id_encomenda = aux_query[0];
            String dInit = aux_query[1];
            String dFinal = aux_query[2];
            String nomes_produtos = aux_query[3];
            String preco = aux_query[4];
            String morada = aux_query[5];
            int quantidade = 0;
            String state = aux_query[7];

            if (aux_query[6].Equals("none"))
            {
                quantidade = 9999;
            }
            else
            {
                quantidade = int.Parse(aux_query[6]);
            }

            List<Model.Encomenda> aux_enc = selectIdEncomenda(encomendas, id_encomenda);
            List<Model.Encomenda> aux_dateI = selectDataInicio(dInit, aux_enc);
            List<Model.Encomenda> aux_dateF = selectDataFim(dFinal, aux_dateI);
            List<Model.Encomenda> aux_nameP = selectNomeProduto(nomes_produtos, aux_dateF);
            List<Model.Encomenda> aux_preco = selectPreco(preco, aux_nameP);
            List<Model.Encomenda> aux_morada = selectMorada(morada, aux_preco);
            List<Model.Encomenda> aux_state = selectState(state, aux_morada);

            for (int i = 0; i < aux_state.Count; i++)
            {
                if (i == quantidade)
                {
                    break;
                }
                else if (i < quantidade)
                {
                    retorno.Add(aux_state[i]);
                }
            }

            return retorno;
        }

        private static List<Model.Encomenda> selectState(string state, List<Model.Encomenda> aux_preco_morada)
        {
            List<Model.Encomenda> retorno = new List<Model.Encomenda>();

            if (state.Equals("none"))
            {
                for (int i = 0; i < aux_preco_morada.Count; i++)
                {
                    retorno.Add(aux_preco_morada[i]);
                }
            }
            else
            {
                for (int i = 0; i < aux_preco_morada.Count; i++)
                {
                    if (state.Equals("entregue"))
                    {
                        if (aux_preco_morada[i].estadoEntrega.Equals("paga") || aux_preco_morada[i].estadoEntrega.Equals("nao paga") || aux_preco_morada[i].estadoEntrega.Equals("nao facturada"))
                        {
                            retorno.Add(aux_preco_morada[i]);
                        }
                    }
                    else
                    {
                        if (aux_preco_morada[i].estadoEntrega.Equals(state))
                        {
                            retorno.Add(aux_preco_morada[i]);
                        }
                    }
                }
            }

            return retorno;
        }

        private static String nameProduto(String prod)
        {
            String aux = Regex.Replace(prod, "________", " ");

            return aux;
        }

        private static List<Model.Encomenda> selectMorada(String morada, List<Model.Encomenda> aux_preco)
        {
            // Selecionar por morada
            List<Model.Encomenda> aux_morada = new List<Model.Encomenda>();

            if (morada.Equals("none"))
            {
                for (int i = 0; i < aux_preco.Count; i++)
                {
                    aux_morada.Add(aux_preco[i]);
                }
            }
            else if (!morada.Equals("none"))
            {
                for (int i = 0; i < aux_preco.Count; i++)
                {
                    Model.Cliente aux_cli = Lib_Primavera.ClienteGes.getInfoUtilizador(aux_preco[i].codCliente);

                    if (aux_cli.Morada.Equals(morada))
                    {
                        aux_morada.Add(aux_preco[i]);
                    }
                }
            }
            return aux_morada;
        }

        private static List<Model.Encomenda> selectPreco(String preco, List<Model.Encomenda> aux_nameP)
        {
            // Selecionar por preco
            List<Model.Encomenda> aux_preco = new List<Model.Encomenda>();

            if (preco.Equals("none"))
            {
                for (int i = 0; i < aux_nameP.Count; i++)
                {
                    aux_preco.Add(aux_nameP[i]);
                }
            }
            else if (!preco.Equals("none"))
            {
                for (int i = 0; i < aux_nameP.Count; i++)
                {
                    if (aux_nameP[i].totalMerc.Equals(double.Parse(preco)))
                    {
                        aux_preco.Add(aux_nameP[i]);
                    }
                }
            }
            return aux_preco;
        }

        private static List<Model.Encomenda> selectNomeProduto(String nomes_produtos, List<Model.Encomenda> aux_dateF)
        {
            // Selecionar por nome do produto
            List<Model.Encomenda> aux_nameP = new List<Model.Encomenda>();
            String[] aux_produto = Regex.Split(nomes_produtos, "_");

            if (nomes_produtos.Equals("none"))
            {
                for (int i = 0; i < aux_dateF.Count; i++)
                {
                    aux_nameP.Add(aux_dateF[i]);
                }
            }
            else if (!nomes_produtos.Equals("none"))
            {
                for (int i = 0; i < aux_dateF.Count; i++)
                {
                    int match = 0;

                    for (int j = 0; j < aux_dateF[i].ListaArtigos.Count; j++)
                    {
                        if (aux_dateF[i].ListaArtigos.Count >= aux_produto.Count())
                        {
                            for (int l = 0; l < aux_produto.Count(); l++)
                            {
                                if (aux_dateF[i].ListaArtigos[j].descricaoArtigo.Equals(aux_produto[l]))
                                {
                                    match++;
                                }
                            }

                            if (match == aux_produto.Count())
                            {
                                aux_nameP.Add(aux_dateF[i]);
                                break;
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }
            return aux_nameP;
        }

        private static List<Model.Encomenda> selectDataFim(String dFinal, List<Model.Encomenda> aux_dateI)
        {
            // Selecionar por data de fim
            List<Model.Encomenda> aux_dateF = new List<Model.Encomenda>();

            if (dFinal.Equals("none"))
            {
                for (int i = 0; i < aux_dateI.Count; i++)
                {
                    aux_dateF.Add(aux_dateI[i]);
                }
            }
            else if (!dFinal.Equals("none"))
            {
                DateTime dF = Convert.ToDateTime(dFinal);

                for (int i = 0; i < aux_dateI.Count; i++)
                {
                    if (aux_dateI[i].DataFim <= dF)
                    {
                        aux_dateF.Add(aux_dateI[i]);
                    }
                }
            }
            return aux_dateF;
        }

        private static List<Model.Encomenda> selectDataInicio(String dInit, List<Model.Encomenda> aux_enc)
        {
            // Selecionar por data de inicio
            List<Model.Encomenda> aux_dateI = new List<Model.Encomenda>();

            if (dInit.Equals("none"))
            {
                for (int i = 0; i < aux_enc.Count; i++)
                {
                    aux_dateI.Add(aux_enc[i]);
                }
            }
            else if (!dInit.Equals("none"))
            {
                DateTime dI = Convert.ToDateTime(dInit);

                for (int i = 0; i < aux_enc.Count; i++)
                {
                    if (aux_enc[i].DataInicio >= dI)
                    {
                        aux_dateI.Add(aux_enc[i]);
                    }
                }
            }
            return aux_dateI;
        }

        private static List<Model.Encomenda> selectIdEncomenda(List<Model.Encomenda> encomendas, String id_encomenda)
        {
            // Selecionar as encomendas
            List<Model.Encomenda> aux_enc = new List<Model.Encomenda>();

            if (id_encomenda.Equals("none"))
            {
                for (int i = 0; i < encomendas.Count; i++)
                {
                    aux_enc.Add(encomendas[i]);
                }
            }
            else if (!id_encomenda.Equals("none"))
            {
                for (int i = 0; i < encomendas.Count; i++)
                {
                    if (encomendas[i].codEncomenda.Equals(int.Parse(id_encomenda)))
                    {
                        aux_enc.Add(encomendas[i]);
                    }
                }
            }
            return aux_enc;
        }

        # endregion Encomendas;
    }
}