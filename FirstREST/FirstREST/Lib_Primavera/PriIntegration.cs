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
//using Interop.StdBESql800;
//using Interop.StdBSSql800;  

namespace FirstREST.Lib_Primavera
{
    public class PriIntegration
    {
        

        # region Cliente

        public static List<Model.Cliente> ListaClientes()
        {
               StdBELista objList;

               List<Model.Cliente> listClientes = new List<Model.Cliente>();

               if (PriEngine.InitializeCompany(FirstREST.Properties.Settings.Default.Company.Trim(), FirstREST.Properties.Settings.Default.User.Trim(), FirstREST.Properties.Settings.Default.Password.Trim()) == true)
               {

                   //objList = PriEngine.Engine.Comercial.Clientes.LstClientes();

                   objList = PriEngine.Engine.Consulta("SELECT Cliente, Nome, NumContrib as NumContribuinte, Fac_Mor AS morada,  ClienteAnulado, Fac_Local, Fac_Tel, CDU_PASSWORD, EnderecoWeb  FROM  CLIENTES");


                   while (!objList.NoFim())
                   {
                       listClientes.Add(new Model.Cliente
                       {
                           ID = objList.Valor("Cliente"),
                           NomeCliente = objList.Valor("Nome"),
                           NumContribuinte = objList.Valor("NumContribuinte"),
                           Morada = objList.Valor("morada"),
                           ClienteBanido = objList.Valor("ClienteAnulado").ToString(),
                           Localidade = objList.Valor("Fac_Local"),
                           Telemóvel = objList.Valor("Fac_Tel"),
                           Email = objList.Valor("EnderecoWeb"),
                           Password = PriEngine.Platform.Criptografia.Descripta(objList.Valor("CDU_PASSWORD"), 50)
                       });
                       objList.Seguinte();

                   }

                   return listClientes;
               }
               else
                   return null;
                  
        }


        public static Lib_Primavera.Model.Cliente GetCliente(string id)
        {


            StdBELista objList;

            //List<Model.Cliente> listClientes = new List<Model.Cliente>();
            Model.Cliente myCli = new Model.Cliente();

            if (PriEngine.InitializeCompany(FirstREST.Properties.Settings.Default.Company.Trim(), FirstREST.Properties.Settings.Default.User.Trim(), FirstREST.Properties.Settings.Default.Password.Trim()) == true)
            {

                //objList = PriEngine.Engine.Comercial.Clientes.LstClientes();

                objList = PriEngine.Engine.Consulta("SELECT Cliente, Nome, NumContrib as NumContribuinte, Fac_Mor AS campo_exemplo,  ClienteAnulado,  Fac_Local, Fac_Tel, EnderecoWeb FROM  CLIENTES WHERE Cliente='" + id + "'"); //supostamente nao se quer listar a password
               
                while (!objList.NoFim())
                {
                    
                        myCli.ID = objList.Valor("Cliente");
                        myCli.NomeCliente = objList.Valor("Nome");
                        myCli.NumContribuinte = objList.Valor("NumContribuinte");
                        myCli.Morada = objList.Valor("campo_exemplo");
                        myCli.ClienteBanido = objList.Valor("ClienteAnulado").ToString();
                        myCli.Localidade = objList.Valor("Fac_Local");
                        myCli.Telemóvel = objList.Valor("Fac_Tel");
                        myCli.Email = objList.Valor("EnderecoWeb");

                   
                    objList.Seguinte();

                }

                return myCli;
            }
            else
                return null;
        }


        public static Lib_Primavera.Model.RespostaErro UpdCliente(Lib_Primavera.Model.Cliente cliente)
        {
            Lib_Primavera.Model.RespostaErro erro = new Model.RespostaErro();
           

            GcpBECliente objCli = new GcpBECliente();

            StdBECampos campos = new StdBECampos();
            StdBECampo campo = new StdBECampo();


            try
            {

                if (PriEngine.InitializeCompany(FirstREST.Properties.Settings.Default.Company.Trim(), FirstREST.Properties.Settings.Default.User.Trim(), FirstREST.Properties.Settings.Default.Password.Trim()) == true)
                {

                    if (PriEngine.Engine.Comercial.Clientes.Existe(cliente.ID) == false)
                    {
                        erro.Erro = 1;
                        erro.Descricao = "O cliente não existe";
                        return erro;
                    }
                    else
                    {

                        objCli = PriEngine.Engine.Comercial.Clientes.Edita(cliente.ID);
                        objCli.set_EmModoEdicao(true);

                        objCli.set_Nome(cliente.NomeCliente);
                        objCli.set_NumContribuinte(cliente.NumContribuinte);
                        objCli.set_EnderecoWeb(cliente.Email);
                        objCli.set_Morada(cliente.Morada);
                        objCli.set_Telefone(cliente.Telemóvel);
                        objCli.set_Localidade(cliente.Localidade);

                        //EDITAR A PASSWORD
                        campo.Nome = "CDU_Password";
                        //FALTA mudar na base de dados.
                        //campo.Valor = PriEngine.Platform.Criptografia.Encripta(cliente.Password, 50);
                        campo.Valor = cliente.Password;
                        campos.Insere(campo);
                       
                        PriEngine.Engine.Comercial.Clientes.Actualiza(objCli);

                        erro.Erro = 0;
                        erro.Descricao = "Sucesso";
                        return erro;
                    }
                }
                else
                {
                    erro.Erro = 1;
                    erro.Descricao = "Erro ao abrir a empresa";
                    return erro;

                }

            }

            catch (Exception ex)
            {
                erro.Erro = 1;
                erro.Descricao = ex.Message;
                return erro;
            }

        }


        public static Lib_Primavera.Model.RespostaErro DelCliente(string id)
        {

            Lib_Primavera.Model.RespostaErro erro = new Model.RespostaErro();
            GcpBECliente objCli = new GcpBECliente();


            try
            {

                if (PriEngine.InitializeCompany(FirstREST.Properties.Settings.Default.Company.Trim(), FirstREST.Properties.Settings.Default.User.Trim(), FirstREST.Properties.Settings.Default.Password.Trim()) == true)
                {
                    if (PriEngine.Engine.Comercial.Clientes.Existe(id) == false)
                    {
                        erro.Erro = 1;
                        erro.Descricao = "O cliente não existe";
                        return erro;
                    }
                    else
                    {

                        PriEngine.Engine.Comercial.Clientes.Remove(id);
                        erro.Erro = 0;
                        erro.Descricao = "Sucesso";
                        return erro;
                    }
                }

                else
                {
                    erro.Erro = 1;
                    erro.Descricao = "Erro ao abrir a empresa";
                    return erro;
                }
            }

            catch (Exception ex)
            {
                erro.Erro = 1;
                erro.Descricao = ex.Message;
                return erro;
            }

        }


        public static bool existeEmail(string email)
        {
            StdBELista objList;
            objList = PriEngine.Engine.Consulta("SELECT Cliente FROM  CLIENTES WHERE EnderecoWeb='" + email + "'");

            if (objList != null)
            {
                if (objList.Vazia())
                    return false;
                return true;
            }
            return false;
        }

        public static bool existeNome(string nome)
        {
            StdBELista objList;
            objList = PriEngine.Engine.Consulta("SELECT Cliente FROM  CLIENTES WHERE Nome='" + nome + "'");

            if (objList != null)
            {
                if (objList.Vazia())
                    return false;
                return true;
            }
            return false;
        }

        public static Lib_Primavera.Model.RespostaErro InsereClienteObj(Model.Cliente cli)
        {

            Lib_Primavera.Model.RespostaErro erro = new Model.RespostaErro();


            GcpBECliente myCli = new GcpBECliente();

            StdBECampos campos = new StdBECampos();
            StdBECampo campo = new StdBECampo();

            try
            {
                if (PriEngine.InitializeCompany(FirstREST.Properties.Settings.Default.Company.Trim(), FirstREST.Properties.Settings.Default.User.Trim(), FirstREST.Properties.Settings.Default.Password.Trim()) == true)
                {
                    myCli.set_NumContribuinte(cli.NumContribuinte);
                    myCli.set_Morada(cli.Morada);
                    myCli.set_Telefone(cli.Telemóvel);
                    myCli.set_Localidade(cli.Localidade);
                    myCli.set_Moeda("EUR");

                    //Dar o id correto, e por acaso isto verifica se tem o mesmo id automaticamente
                    List<Model.Cliente> clientes = ListaClientes();
                    int id;

                    if (clientes.Count >= 2)
                    {
                        id = clientes.Count;
                    }
                    else
                        id = 1;

                    myCli.set_Cliente(id.ToString());
                    
                    //Verificação Email
                    if (cli.Email == null)
                    {
                        erro.Erro = 1;
                        erro.Descricao = "Introduza um email válido, por favor.";
                        return erro;
                    }
                    else if (existeEmail(cli.Email))
                    {
                        erro.Erro = 1;
                        erro.Descricao = "Email já existente.";
                        return erro;
                    }
                    else
                    {
                        myCli.set_EnderecoWeb(cli.Email);
                    }

                    //Verificação nome
                    if (cli.NomeCliente == null)
                    {
                        erro.Erro = 1;
                        erro.Descricao = "Introduza um nome, por favor.";
                        return erro;
                    }
                    else if (existeNome(cli.NomeCliente))
                    {
                        erro.Erro = 1;
                        erro.Descricao = "Introduza outro nome, por favor.";
                        return erro;
                    }
                    else
                    {
                        myCli.set_Nome(cli.NomeCliente);
                    }



                    //inserir a password
                    campo.Nome = "CDU_Password";
                    //É preciso aumentar o espaço da base de dados, 20 nao chega para encripta-la tem que ter 50 para ai ou mais.
                    //campo.Valor = PriEngine.Platform.Criptografia.Encripta(cli.Password, 20);
                    campo.Valor = cli.Password;
                    campos.Insere(campo);
                    myCli.set_CamposUtil(campos);

                    PriEngine.Engine.Comercial.Clientes.Actualiza(myCli);

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
                erro.Erro = 1;
                erro.Descricao = ex.Message;
                return erro;
            }


        }

       

        #endregion Cliente;   // -----------------------------  END   CLIENTE    -----------------------


        #region Artigo

        public static Lib_Primavera.Model.Artigo GetArtigo(string codArtigo)
        {
            
            GcpBEArtigo objArtigo = new GcpBEArtigo();
            Model.Artigo myArt = new Model.Artigo();

            if (PriEngine.InitializeCompany(FirstREST.Properties.Settings.Default.Company.Trim(), FirstREST.Properties.Settings.Default.User.Trim(), FirstREST.Properties.Settings.Default.Password.Trim()) == true)
            {

                if (PriEngine.Engine.Comercial.Artigos.Existe(codArtigo) == false)
                {
                    return null;
                }
                else
                {
                    objArtigo = PriEngine.Engine.Comercial.Artigos.Edita(codArtigo);
                    myArt.CodArtigo = objArtigo.get_Artigo();
                    myArt.DescArtigo = objArtigo.get_Descricao();
                    myArt.ArtigoAnulado = objArtigo.get_Anulado().ToString();
                    myArt.Desconto = objArtigo.get_Desconto().ToString();
                    myArt.STKActual = objArtigo.get_StkActual().ToString();
                    myArt.PCPadrao = objArtigo.get_PCPadrao().ToString();
                    myArt.PrazoEntrega = objArtigo.get_PrazoEntrega().ToString();
                    myArt.Familia = objArtigo.get_Familia();
                    myArt.SubFamilia = objArtigo.get_SubFamilia();
                    myArt.Marca = objArtigo.get_Marca();
                    myArt.Modelo = objArtigo.get_Modelo();
                    myArt.TipoArtigo = objArtigo.get_TipoArtigo();
                    myArt.Iva = objArtigo.get_IVA();

                    return myArt;
                }
                
            }
            else
            {
                return null;
            }

        }

        public static List<Model.Artigo> ListaArtigos()
        {
                        
            StdBELista objList;

            Model.Artigo art = new Model.Artigo();
            List<Model.Artigo> listArts = new List<Model.Artigo>();

            if (PriEngine.InitializeCompany(FirstREST.Properties.Settings.Default.Company.Trim(), FirstREST.Properties.Settings.Default.User.Trim(), FirstREST.Properties.Settings.Default.Password.Trim()) == true)
            {
                objList = PriEngine.Engine.Consulta("SELECT Artigo, Descricao, ArtigoAnulado, Desconto, STKActual, PCPadrao, PrazoEntrega, Familia, SubFamilia, Marca, Modelo, TipoArtigo, Iva FROM  ARTIGO");

                //objList = PriEngine.Engine.Comercial.Artigos.LstArtigos();

                while (!objList.NoFim())
                {
                    art = new Model.Artigo();
                    art.CodArtigo = objList.Valor("artigo");
                    art.DescArtigo = objList.Valor("descricao");
                    art.ArtigoAnulado = objList.Valor("ArtigoAnulado").ToString();
                    art.Desconto = objList.Valor("desconto").ToString();
                    art.STKActual = objList.Valor("stkactual").ToString();
                    art.PCPadrao = objList.Valor("pcpadrao").ToString();
                    art.PrazoEntrega = objList.Valor("prazoentrega").ToString();
                    art.Familia = objList.Valor("familia");
                    art.SubFamilia = objList.Valor("subfamilia");
                    art.Marca = objList.Valor("marca");
                    art.Modelo = objList.Valor("modelo");
                    art.TipoArtigo = objList.Valor("tipoartigo");
                    art.Iva = objList.Valor("iva");


                    listArts.Add(art);
                    objList.Seguinte();
                }

                return listArts;

            }
            else
            {
                return null;

            }

        }

        public static Lib_Primavera.Model.RespostaErro InsereArtigoObj(Model.Artigo art)
        {

            Lib_Primavera.Model.RespostaErro erro = new Model.RespostaErro();


            GcpBEArtigo myArt = new GcpBEArtigo();

            try
            {
                if (PriEngine.InitializeCompany(FirstREST.Properties.Settings.Default.Company.Trim(), FirstREST.Properties.Settings.Default.User.Trim(), FirstREST.Properties.Settings.Default.Password.Trim()) == true)
                {

                    myArt.set_Artigo(art.CodArtigo);
                    myArt.set_Descricao(art.DescArtigo);
                    myArt.set_Anulado(false);
                    myArt.set_Desconto(Convert.ToSingle(art.Desconto));
                    myArt.set_StkActual(Convert.ToSingle(art.STKActual));
                    myArt.set_PCPadrao(Convert.ToSingle(art.PCPadrao));
                    myArt.set_PrazoEntrega(Convert.ToInt16(art.PrazoEntrega));
                    myArt.set_Familia(art.Familia);
                    myArt.set_SubFamilia(art.SubFamilia);
                    myArt.set_Marca(art.Marca);
                    myArt.set_Modelo(art.Modelo);
                    myArt.set_TipoArtigo(art.TipoArtigo);
                    myArt.set_IVA(art.Iva);
                    //PriEngine.Engine.Consulta("INSERT INTO ARTIGO (Artigo, Desconto, Descricao, Familia, IVA, Marca, Modelo, PCPadrao, PrazoEntrega, SubFamilia, stkActual, TipoArtigo) VALUES ('" + art.CodArtigo + "','" + art.Desconto + "','" + art.DescArtigo + "','" + art.Familia + "','" + art.Iva + "','" + art.Marca + "','" + art.Modelo + "','" + art.PCPadrao + "','" + art.PrazoEntrega + "','" + art.SubFamilia + "','" + art.STKActual + "','" + art.TipoArtigo + "')");

                    //FALTA AQUI 3
                    PriEngine.Engine.Comercial.Artigos.Actualiza(myArt);

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
                erro.Erro = 1;
                erro.Descricao = ex.Message;
                return erro;
            }


        }

        #endregion Artigo
  

        #region DocCompra
        

        public static List<Model.DocCompra> VGR_List()
        {
                
            StdBELista objListCab;
            StdBELista objListLin;
            Model.DocCompra dc = new Model.DocCompra();
            List<Model.DocCompra> listdc = new List<Model.DocCompra>();
            Model.LinhaDocCompra lindc = new Model.LinhaDocCompra();
            List<Model.LinhaDocCompra> listlindc = new List<Model.LinhaDocCompra>();

            if (PriEngine.InitializeCompany(FirstREST.Properties.Settings.Default.Company.Trim(), FirstREST.Properties.Settings.Default.User.Trim(), FirstREST.Properties.Settings.Default.Password.Trim()) == true)
            {
                objListCab = PriEngine.Engine.Consulta("SELECT id, NumDocExterno, Entidade, DataDoc, NumDoc, TotalMerc, Serie From CabecCompras where TipoDoc='VGR'");
                while (!objListCab.NoFim())
                {
                    dc = new Model.DocCompra();
                    dc.id = objListCab.Valor("id");
                    dc.NumDocExterno = objListCab.Valor("NumDocExterno");
                    dc.Entidade = objListCab.Valor("Entidade");
                    dc.NumDoc = objListCab.Valor("NumDoc");
                    dc.Data = objListCab.Valor("DataDoc");
                    dc.TotalMerc = objListCab.Valor("TotalMerc");
                    dc.Serie = objListCab.Valor("Serie");
                    objListLin = PriEngine.Engine.Consulta("SELECT idCabecCompras, Artigo, Descricao, Quantidade, Unidade, PrecUnit, Desconto1, TotalILiquido, PrecoLiquido, Armazem, Lote from LinhasCompras where IdCabecCompras='" + dc.id + "' order By NumLinha");
                    listlindc = new List<Model.LinhaDocCompra>();

                    while (!objListLin.NoFim())
                    {
                        lindc = new Model.LinhaDocCompra();
                        lindc.IdCabecDoc = objListLin.Valor("idCabecCompras");
                        lindc.CodArtigo = objListLin.Valor("Artigo");
                        lindc.DescArtigo = objListLin.Valor("Descricao");
                        lindc.Quantidade = objListLin.Valor("Quantidade");
                        lindc.Unidade = objListLin.Valor("Unidade");
                        lindc.Desconto = objListLin.Valor("Desconto1");
                        lindc.PrecoUnitario = objListLin.Valor("PrecUnit");
                        lindc.TotalILiquido = objListLin.Valor("TotalILiquido");
                        lindc.TotalLiquido = objListLin.Valor("PrecoLiquido");
                        lindc.Armazem = objListLin.Valor("Armazem");
                        lindc.Lote = objListLin.Valor("Lote");

                        listlindc.Add(lindc);
                        objListLin.Seguinte();
                    }

                    dc.LinhasDoc = listlindc;
                    
                    listdc.Add(dc);
                    objListCab.Seguinte();
                }
            }
            return listdc;
        }

                
        public static Model.RespostaErro VGR_New(Model.DocCompra dc)
        {
            Lib_Primavera.Model.RespostaErro erro = new Model.RespostaErro();
            

            GcpBEDocumentoCompra myGR = new GcpBEDocumentoCompra();
            GcpBELinhaDocumentoCompra myLin = new GcpBELinhaDocumentoCompra();
            GcpBELinhasDocumentoCompra myLinhas = new GcpBELinhasDocumentoCompra();

            PreencheRelacaoCompras rl = new PreencheRelacaoCompras();
            List<Model.LinhaDocCompra> lstlindv = new List<Model.LinhaDocCompra>();

            try
            {
                if (PriEngine.InitializeCompany(FirstREST.Properties.Settings.Default.Company.Trim(), FirstREST.Properties.Settings.Default.User.Trim(), FirstREST.Properties.Settings.Default.Password.Trim()) == true)
                {
                    // Atribui valores ao cabecalho do doc
                    //myEnc.set_DataDoc(dv.Data);
                    myGR.set_Entidade(dc.Entidade);
                    myGR.set_NumDocExterno(dc.NumDocExterno);
                    myGR.set_Serie(dc.Serie);
                    myGR.set_Tipodoc("VGR");
                    myGR.set_TipoEntidade("F");
                    // Linhas do documento para a lista de linhas
                    lstlindv = dc.LinhasDoc;
                    PriEngine.Engine.Comercial.Compras.PreencheDadosRelacionados(myGR, rl);
                    foreach (Model.LinhaDocCompra lin in lstlindv)
                    {
                        PriEngine.Engine.Comercial.Compras.AdicionaLinha(myGR, lin.CodArtigo, lin.Quantidade, lin.Armazem, "", lin.PrecoUnitario, lin.Desconto);
                    }


                    PriEngine.Engine.IniciaTransaccao();
                    PriEngine.Engine.Comercial.Compras.Actualiza(myGR, "Teste");
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


        #endregion DocCompra


        #region DocsVenda

        public static Model.RespostaErro Encomendas_New(Model.DocVenda dv)
        {
            Lib_Primavera.Model.RespostaErro erro = new Model.RespostaErro();
            GcpBEDocumentoVenda myEnc = new GcpBEDocumentoVenda();
             
            GcpBELinhaDocumentoVenda myLin = new GcpBELinhaDocumentoVenda();

            GcpBELinhasDocumentoVenda myLinhas = new GcpBELinhasDocumentoVenda();
             
            PreencheRelacaoVendas rl = new PreencheRelacaoVendas();
            List<Model.LinhaDocVenda> lstlindv = new List<Model.LinhaDocVenda>();
            
            try
            {
                if (PriEngine.InitializeCompany(FirstREST.Properties.Settings.Default.Company.Trim(), FirstREST.Properties.Settings.Default.User.Trim(), FirstREST.Properties.Settings.Default.Password.Trim()) == true)
                {
                    // Atribui valores ao cabecalho do doc
                    //myEnc.set_DataDoc(dv.Data);
                    myEnc.set_Entidade(dv.Entidade);
                    myEnc.set_Serie(dv.Serie);
                    myEnc.set_Tipodoc("ECL");
                    myEnc.set_TipoEntidade("C");
                    // Linhas do documento para a lista de linhas
                    lstlindv = dv.LinhasDoc;
                    PriEngine.Engine.Comercial.Vendas.PreencheDadosRelacionados(myEnc, rl);
                    foreach (Model.LinhaDocVenda lin in lstlindv)
                    {
                        PriEngine.Engine.Comercial.Vendas.AdicionaLinha(myEnc, lin.CodArtigo, lin.Quantidade, "", "", lin.PrecoUnitario, lin.Desconto);
                    }


                   // PriEngine.Engine.Comercial.Compras.TransformaDocumento(

                    PriEngine.Engine.IniciaTransaccao();
                    PriEngine.Engine.Comercial.Vendas.Actualiza(myEnc, "Teste");
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

     

        public static List<Model.DocVenda> Encomendas_List()
        {
            
            StdBELista objListCab;
            StdBELista objListLin;
            Model.DocVenda dv = new Model.DocVenda();
            List<Model.DocVenda> listdv = new List<Model.DocVenda>();
            Model.LinhaDocVenda lindv = new Model.LinhaDocVenda();
            List<Model.LinhaDocVenda> listlindv = new
            List<Model.LinhaDocVenda>();

            if (PriEngine.InitializeCompany(FirstREST.Properties.Settings.Default.Company.Trim(), FirstREST.Properties.Settings.Default.User.Trim(), FirstREST.Properties.Settings.Default.Password.Trim()) == true)
            {
                objListCab = PriEngine.Engine.Consulta("SELECT id, Entidade, Data, NumDoc, TotalMerc, Serie From CabecDoc where TipoDoc='ECL'");
                while (!objListCab.NoFim())
                {
                    dv = new Model.DocVenda();
                    dv.id = objListCab.Valor("id");
                    dv.Entidade = objListCab.Valor("Entidade");
                    dv.NumDoc = objListCab.Valor("NumDoc");
                    dv.Data = objListCab.Valor("Data");
                    dv.TotalMerc = objListCab.Valor("TotalMerc");
                    dv.Serie = objListCab.Valor("Serie");
                    objListLin = PriEngine.Engine.Consulta("SELECT idCabecDoc, Artigo, Descricao, Quantidade, Unidade, PrecUnit, Desconto1, TotalILiquido, PrecoLiquido from LinhasDoc where IdCabecDoc='" + dv.id + "' order By NumLinha");
                    listlindv = new List<Model.LinhaDocVenda>();

                    while (!objListLin.NoFim())
                    {
                        lindv = new Model.LinhaDocVenda();
                        lindv.IdCabecDoc = objListLin.Valor("idCabecDoc");
                        lindv.CodArtigo = objListLin.Valor("Artigo");
                        lindv.DescArtigo = objListLin.Valor("Descricao");
                        lindv.Quantidade = objListLin.Valor("Quantidade");
                        lindv.Unidade = objListLin.Valor("Unidade");
                        lindv.Desconto = objListLin.Valor("Desconto1");
                        lindv.PrecoUnitario = objListLin.Valor("PrecUnit");
                        lindv.TotalILiquido = objListLin.Valor("TotalILiquido");
                        lindv.TotalLiquido = objListLin.Valor("PrecoLiquido");

                        listlindv.Add(lindv);
                        objListLin.Seguinte();
                    }

                    dv.LinhasDoc = listlindv;
                    listdv.Add(dv);
                    objListCab.Seguinte();
                }
            }
            return listdv;
        }


       

        public static Model.DocVenda Encomenda_Get(string numdoc)
        {
            
            
            StdBELista objListCab;
            StdBELista objListLin;
            Model.DocVenda dv = new Model.DocVenda();
            Model.LinhaDocVenda lindv = new Model.LinhaDocVenda();
            List<Model.LinhaDocVenda> listlindv = new List<Model.LinhaDocVenda>();

            if (PriEngine.InitializeCompany(FirstREST.Properties.Settings.Default.Company.Trim(), FirstREST.Properties.Settings.Default.User.Trim(), FirstREST.Properties.Settings.Default.Password.Trim()) == true)
            {
                

                string st = "SELECT id, Entidade, Data, NumDoc, TotalMerc, Serie From CabecDoc where TipoDoc='ECL' and NumDoc='" + numdoc + "'";
                objListCab = PriEngine.Engine.Consulta(st);
                dv = new Model.DocVenda();
                dv.id = objListCab.Valor("id");
                dv.Entidade = objListCab.Valor("Entidade");
                dv.NumDoc = objListCab.Valor("NumDoc");
                dv.Data = objListCab.Valor("Data");
                dv.TotalMerc = objListCab.Valor("TotalMerc");
                dv.Serie = objListCab.Valor("Serie");
                objListLin = PriEngine.Engine.Consulta("SELECT idCabecDoc, Artigo, Descricao, Quantidade, Unidade, PrecUnit, Desconto1, TotalILiquido, PrecoLiquido from LinhasDoc where IdCabecDoc='" + dv.id + "' order By NumLinha");
                listlindv = new List<Model.LinhaDocVenda>();

                while (!objListLin.NoFim())
                {
                    lindv = new Model.LinhaDocVenda();
                    lindv.IdCabecDoc = objListLin.Valor("idCabecDoc");
                    lindv.CodArtigo = objListLin.Valor("Artigo");
                    lindv.DescArtigo = objListLin.Valor("Descricao");
                    lindv.Quantidade = objListLin.Valor("Quantidade");
                    lindv.Unidade = objListLin.Valor("Unidade");
                    lindv.Desconto = objListLin.Valor("Desconto1");
                    lindv.PrecoUnitario = objListLin.Valor("PrecUnit");
                    lindv.TotalILiquido = objListLin.Valor("TotalILiquido");
                    lindv.TotalLiquido = objListLin.Valor("PrecoLiquido");
                    listlindv.Add(lindv);
                    objListLin.Seguinte();
                }

                dv.LinhasDoc = listlindv;
                return dv;
            }
            return null;
        }

        #endregion DocsVenda
    }
}