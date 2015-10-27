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
                           //Password = PriEngine.Platform.Criptografia.Descripta(objList.Valor("CDU_PASSWORD"), 50)
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

                    //Dar o id correto (ainda nao esta muito bem) , e por acaso isto verifica se tem o mesmo id automaticamente
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

        public static Lib_Primavera.Model.Artigo GetArtigo(string id)
        {
            
            StdBELista objList;
            StdBELista objImage;
            StdBELista objComents;
            StdBELista objRating;
            StdBELista objWishlist;
            
            Model.Artigo myArt = new Model.Artigo();

            if (PriEngine.InitializeCompany(FirstREST.Properties.Settings.Default.Company.Trim(), FirstREST.Properties.Settings.Default.User.Trim(), FirstREST.Properties.Settings.Default.Password.Trim()) == true)
            {

                //objList = PriEngine.Engine.Comercial.Clientes.LstClientes();

                objList = PriEngine.Engine.Consulta("SELECT Artigo, Descricao, Desconto, STKActual, PCPadrao, Familia, SubFamilia, Marca, Modelo FROM  ARTIGO WHERE Artigo='" + id + "'"); 

                while (!objList.NoFim())
                {

                    myArt.ID = objList.Valor("Artigo");
                    float desconto = objList.Valor("Desconto");
                    myArt.Desconto = desconto.ToString();
                    myArt.DescArtigo = objList.Valor("Descricao");
                    double stokeAtual = objList.Valor("STKActual");
                    myArt.STKActual = stokeAtual.ToString();
                    double preco = objList.Valor("PCPadrao");
                    myArt.Preço = preco.ToString();
                    myArt.Familia = objList.Valor("Familia");
                    myArt.SubFamilia = objList.Valor("SubFamilia");
                    myArt.Marca = objList.Valor("Marca");
                    myArt.Modelo = objList.Valor("Modelo");

                    objImage = PriEngine.Engine.Consulta("SELECT CDU_Caminho FROM TDU_Imagem, TDU_ImagemProduto WHERE CDU_idProduto='" + myArt.ID + "' AND TDU_Imagem.CDU_idImagem = TDU_ImagemProduto.CDU_idImagem");
                    myArt.imagens = new List<String>();

                    while (!objImage.NoFim())
                    {
                        
                       myArt.imagens.Add(objImage.Valor("CDU_Caminho"));

                       objImage.Seguinte();
                    }



                    objComents = PriEngine.Engine.Consulta("SELECT Nome, CDU_idComentario, CDU_idProduto, CDU_Conteudo FROM TDU_Comentario, CLIENTES WHERE CDU_idProduto='" + myArt.ID + "' AND CDU_idUtilizador = Cliente");
                    myArt.comentarios = new List<Model.Comentario>();

                    
                    while (!objComents.NoFim())
                    {
                        Model.Comentario temp = new Model.Comentario();
                        temp.idProduto = objComents.Valor("CDU_idProduto");
                        temp.nomeCliente = objComents.Valor("Nome");
                        temp.idComentario = objComents.Valor("CDU_idComentario").ToString();
                        temp.Conteudo = objComents.Valor("CDU_Conteudo");

                        myArt.comentarios.Add(temp);

                        objComents.Seguinte();
                    }

                    objRating = PriEngine.Engine.Consulta("SELECT AVG(CDU_Valor) AS Media FROM TDU_Rating WHERE CDU_idProduto='" + myArt.ID + "'");
                    while (!objRating.NoFim())
                    {
                        myArt.Rating = objRating.Valor("Media").ToString();
                        objRating.Seguinte();
                    }

                    objWishlist = PriEngine.Engine.Consulta("SELECT * FROM TDU_Wishlist, TDU_WishlistProduto WHERE CDU_idUtilizador ='" + "ALCAD" + "' AND CDU_idProduto='" + myArt.ID + "' AND TDU_Wishlist.CDU_idWishlist = TDU_WishlistProduto.CDU_idWishlist");
                    if (!objWishlist.NoFim())
                    {
                        myArt.Wishlist = "True";
                    }
                    else
                        myArt.Wishlist = "False";
                    
                    objList.Seguinte();

                }

                return myArt;
            }
            else
                return null;

        }

        public static Lib_Primavera.Model.Artigo GetArtigoByCategoria(string sub_familia) //Recomendados
        {

            StdBELista objList;

            Model.Artigo myArt = new Model.Artigo();

            if (PriEngine.InitializeCompany(FirstREST.Properties.Settings.Default.Company.Trim(), FirstREST.Properties.Settings.Default.User.Trim(), FirstREST.Properties.Settings.Default.Password.Trim()) == true)
            {

                //objList = PriEngine.Engine.Comercial.Clientes.LstClientes();

                objList = PriEngine.Engine.Consulta("SELECT Artigo, Descricao, Desconto, STKActual, PCPadrao, Familia, SubFamilia, Marca, Modelo FROM  ARTIGO WHERE SubFamilia='" + sub_familia + "'");

                while (!objList.NoFim())
                {

                    myArt.ID = objList.Valor("Artigo");
                    float desconto = objList.Valor("Desconto");
                    myArt.Desconto = desconto.ToString();
                    myArt.DescArtigo = objList.Valor("Descricao");
                    double stokeAtual = objList.Valor("STKActual");
                    myArt.STKActual = stokeAtual.ToString();
                    double preco = objList.Valor("PCPadrao");
                    myArt.Preço = preco.ToString();
                    myArt.Familia = objList.Valor("Familia");
                    myArt.SubFamilia = objList.Valor("SubFamilia");
                    myArt.Marca = objList.Valor("Marca");
                    myArt.Modelo = objList.Valor("Modelo");

                    objList.Seguinte();

                }

                return myArt;
            }
            else
                return null;

        } 

        public static List<Model.Artigo> ListaArtigos()
        {
                        
            StdBELista objList;

            Model.Artigo art = new Model.Artigo();
            List<Model.Artigo> listArts = new List<Model.Artigo>();

            if (PriEngine.InitializeCompany(FirstREST.Properties.Settings.Default.Company.Trim(), FirstREST.Properties.Settings.Default.User.Trim(), FirstREST.Properties.Settings.Default.Password.Trim()) == true)
            {
                objList = PriEngine.Engine.Consulta("SELECT Artigo, Descricao, Desconto, STKActual, PCPadrao, Familia, SubFamilia, Marca, Modelo FROM  ARTIGO");

                //objList = PriEngine.Engine.Comercial.Artigos.LstArtigos();

                while (!objList.NoFim())
                {
                    art = new Model.Artigo();
                    art.ID = objList.Valor("artigo");
                    art.DescArtigo = objList.Valor("descricao");
                    art.Desconto = objList.Valor("desconto").ToString();
                    art.STKActual = objList.Valor("stkactual").ToString();
                    art.Preço = objList.Valor("pcpadrao").ToString();
                    art.Familia = objList.Valor("familia");
                    art.SubFamilia = objList.Valor("subfamilia");
                    art.Marca = objList.Valor("marca");
                    art.Modelo = objList.Valor("modelo");

                    //falta as imagens

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

                    myArt.set_Artigo(art.ID);
                    myArt.set_Descricao(art.DescArtigo);
                    myArt.set_Desconto(Convert.ToSingle(art.Desconto));
                    myArt.set_StkActual(Convert.ToSingle(art.STKActual));
                    myArt.set_PCPadrao(Convert.ToSingle(art.Preço));
                    myArt.set_Familia(art.Familia);
                    myArt.set_SubFamilia(art.SubFamilia);
                    myArt.set_Marca(art.Marca);
                    myArt.set_Modelo(art.Modelo);
                    myArt.set_IVA("23");
                    
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

        public static Lib_Primavera.Model.RespostaErro UpdArtigo(Lib_Primavera.Model.Artigo art)
        {
            Lib_Primavera.Model.RespostaErro erro = new Model.RespostaErro();


            GcpBEArtigo objArt = new GcpBEArtigo();


            try
            {

                if (PriEngine.InitializeCompany(FirstREST.Properties.Settings.Default.Company.Trim(), FirstREST.Properties.Settings.Default.User.Trim(), FirstREST.Properties.Settings.Default.Password.Trim()) == true)
                {

                    if (PriEngine.Engine.Comercial.Artigos.Existe(art.ID) == false)
                    {
                        erro.Erro = 1;
                        erro.Descricao = "O artigo não existe";
                        return erro;
                    }
                    else
                    {

                        objArt = PriEngine.Engine.Comercial.Artigos.Edita(art.ID);
                        objArt.set_EmModoEdicao(true);

                        objArt.set_Artigo(art.ID);
                        objArt.set_Descricao(art.DescArtigo);
                        objArt.set_Desconto(Convert.ToSingle(art.Desconto));
                        objArt.set_StkActual(Convert.ToSingle(art.STKActual));
                        objArt.set_PCPadrao(Convert.ToSingle(art.Preço));
                        objArt.set_Familia(art.Familia);
                        objArt.set_SubFamilia(art.SubFamilia);
                        objArt.set_Marca(art.Marca);
                        objArt.set_Modelo(art.Modelo);


                        PriEngine.Engine.Comercial.Artigos.Actualiza(objArt);

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

        public static Lib_Primavera.Model.RespostaErro DelArtigo(string id)
        {

            Lib_Primavera.Model.RespostaErro erro = new Model.RespostaErro();
            GcpBEArtigo objArt = new GcpBEArtigo();


            try
            {

                if (PriEngine.InitializeCompany(FirstREST.Properties.Settings.Default.Company.Trim(), FirstREST.Properties.Settings.Default.User.Trim(), FirstREST.Properties.Settings.Default.Password.Trim()) == true)
                {
                    if (PriEngine.Engine.Comercial.Artigos.Existe(id) == false)
                    {
                        erro.Erro = 1;
                        erro.Descricao = "O cliente não existe";
                        return erro;
                    }
                    else
                    {

                        PriEngine.Engine.Comercial.Artigos.Remove(id);
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


        # region Carrinho

        public static Lib_Primavera.Model.Carrinho GetCarrinhoUser(string id_user)
        {
     
            StdBELista objListCarrinho;
            StdBELista objList;


            Model.Artigo art = new Model.Artigo();
            Model.Carrinho carr = new Model.Carrinho();

            List<Model.Artigo> listArtigos = new List<Model.Artigo>();

            if (PriEngine.InitializeCompany(FirstREST.Properties.Settings.Default.Company.Trim(), FirstREST.Properties.Settings.Default.User.Trim(), FirstREST.Properties.Settings.Default.Password.Trim()) == true)
            {
                objList = PriEngine.Engine.Consulta("SELECT CDU_idCarrinho, CDU_idCliente, CDU_idProduto FROM  TDU_CarrinhoCompras, TDU_CarrinhoProduto WHERE CDU_idCliente='"+ id_user +"' AND CDU_idCarrinho = CDU_idCarrinhoCompras");

                //objList = PriEngine.Engine.Comercial.Artigos.LstArtigos();

                while (!objList.NoFim())
                {
                    carr = new Model.Carrinho();
                    carr.ID = objList.Valor("CDU_idCarrinho").ToString();
                    carr.ID_Cliente = objList.Valor("CDU_idCliente").ToString();
                    String idTemp = objList.Valor("CDU_idProduto").ToString();

                    objListCarrinho = PriEngine.Engine.Consulta(" SELECT Artigo, Descricao, Desconto, STKActual, PCPadrao, Familia, SubFamilia, Marca, Modelo FROM  ARTIGO WHERE Artigo = '" + idTemp + "'");
                    listArtigos = new List<Model.Artigo>();

                    while (!objListCarrinho.NoFim())
                    {
                        art = new Model.Artigo();
                        art.ID = objListCarrinho.Valor("artigo");
                        art.DescArtigo = objListCarrinho.Valor("descricao");
                        art.Desconto = objListCarrinho.Valor("desconto").ToString();
                        art.STKActual = objListCarrinho.Valor("stkactual").ToString();
                        art.Preço = objListCarrinho.Valor("pcpadrao").ToString();
                        art.Familia = objListCarrinho.Valor("familia");
                        art.SubFamilia = objListCarrinho.Valor("subfamilia");
                        art.Marca = objListCarrinho.Valor("marca");
                        art.Modelo = objListCarrinho.Valor("modelo");

                        listArtigos.Add(art);
                        objListCarrinho.Seguinte();
                    }
                    //falta as imagens
                    carr.ID_Produtos = listArtigos;
                    objList.Seguinte();
                }

                /*
                 Consulta("SELECT CDU_idCarrinho, CDU_idCliente, CDU_idCarrinhoCompras, Artigo, Descricao, Desconto, STKActual, PCPadrao, Familia, SubFamilia, Marca, Modelo FROM  ARTIGO, TDU_idCarrinhoCompras, TDU_CarrinhoProduto WHERE CDU_idProduto = Artigo");

                 * 
                 * */


                return carr;

            }
            else
            {
                return null;

            }

        }

        public static List<Model.Carrinho> ListaCarrinhos()
        {

      
            StdBELista objListCarrinho;
            StdBELista objList;


            Model.Artigo myArt = new Model.Artigo();
            Model.Carrinho carr = new Model.Carrinho();
            List<Model.Carrinho> listCarrinhos = new List<Model.Carrinho>();
            List<Model.Artigo> listArtigos = new List<Model.Artigo>();

            if (PriEngine.InitializeCompany(FirstREST.Properties.Settings.Default.Company.Trim(), FirstREST.Properties.Settings.Default.User.Trim(), FirstREST.Properties.Settings.Default.Password.Trim()) == true)
            {
                objList = PriEngine.Engine.Consulta("SELECT CDU_idCarrinho, CDU_idCliente, CDU_idProduto FROM  TDU_CarrinhoCompras, TDU_CarrinhoProduto");

                //objList = PriEngine.Engine.Comercial.Artigos.LstArtigos();

                while (!objList.NoFim())
                {
                    carr = new Model.Carrinho();
                    carr.ID = objList.Valor("CDU_idCarrinho").ToString();
                    carr.ID_Cliente = objList.Valor("CDU_idCliente").ToString();
                    string idTemp = objList.Valor("CDU_idProduto");

                    objListCarrinho = PriEngine.Engine.Consulta("SELECT Artigo, Descricao, Desconto, STKActual, PCPadrao, Familia, SubFamilia, Marca, Modelo FROM  ARTIGO WHERE Artigo='" + idTemp + "'");
                    listArtigos = new List<Model.Artigo>();

                    while (!objListCarrinho.NoFim())
                    {

                        myArt.ID = objListCarrinho.Valor("Artigo");
                        float desconto = objListCarrinho.Valor("Desconto");
                        myArt.Desconto = desconto.ToString();
                        myArt.DescArtigo = objListCarrinho.Valor("Descricao");
                        double stokeAtual = objListCarrinho.Valor("STKActual");
                        myArt.STKActual = stokeAtual.ToString();
                        double preco = objListCarrinho.Valor("PCPadrao");
                        myArt.Preço = preco.ToString();
                        myArt.Familia = objListCarrinho.Valor("Familia");
                        myArt.SubFamilia = objListCarrinho.Valor("SubFamilia");
                        myArt.Marca = objListCarrinho.Valor("Marca");
                        myArt.Modelo = objListCarrinho.Valor("Modelo");

                        listArtigos.Add(myArt);
                        objListCarrinho.Seguinte();
                    }
                    //falta as imagens
                    carr.ID_Produtos = listArtigos;
                    listCarrinhos.Add(carr);
                    objList.Seguinte();
                }

                /*
                 Consulta("SELECT CDU_idCarrinho, CDU_idCliente, CDU_idCarrinhoCompras, Artigo, Descricao, Desconto, STKActual, PCPadrao, Familia, SubFamilia, Marca, Modelo FROM  ARTIGO, TDU_idCarrinhoCompras, TDU_CarrinhoProduto WHERE CDU_idProduto = Artigo");

                 * 
                 * */


                return listCarrinhos;

            }
            else
            {
                return null;

            }

        }

        #endregion Carrinho


        # region Pagamento

        public static Lib_Primavera.Model.Pagamento GetPagamentoUser(string id_user)
        {

            StdBELista objListCarrinho;
            StdBELista objList;


            Model.Artigo art = new Model.Artigo();
            Model.Pagamento pag = new Model.Pagamento();

            List<Model.Artigo> listArtigos = new List<Model.Artigo>();

            if (PriEngine.InitializeCompany(FirstREST.Properties.Settings.Default.Company.Trim(), FirstREST.Properties.Settings.Default.User.Trim(), FirstREST.Properties.Settings.Default.Password.Trim()) == true)
            {
                objList = PriEngine.Engine.Consulta("SELECT ModoPag, CDU_idCarrinho, CDU_idCliente, CDU_idProduto FROM  CLIENTES, TDU_CarrinhoCompras, TDU_CarrinhoProduto  WHERE CDU_idCliente='" + id_user + "' AND Cliente='" + id_user + "' AND CDU_idCarrinho = CDU_idCarrinhoCompras");

                //objList = PriEngine.Engine.Comercial.Artigos.LstArtigos();

                while (!objList.NoFim())
                {
                    pag = new Model.Pagamento();
                    pag.ID_Carrinho = objList.Valor("CDU_idCarrinho").ToString();
                    pag.ID_Cliente = objList.Valor("CDU_idCliente").ToString();
                    pag.ModoPagamento = objList.Valor("ModoPag").ToString();
                    String idTemp = objList.Valor("CDU_idProduto").ToString();

                    objListCarrinho = PriEngine.Engine.Consulta(" SELECT Artigo, Descricao, Desconto, STKActual, PCPadrao, Familia, SubFamilia, Marca, Modelo FROM  ARTIGO WHERE Artigo = '" + idTemp + "'");
                    listArtigos = new List<Model.Artigo>();

                    while (!objListCarrinho.NoFim())
                    {
                        art = new Model.Artigo();
                        art.ID = objListCarrinho.Valor("artigo");
                        art.DescArtigo = objListCarrinho.Valor("descricao");
                        art.Desconto = objListCarrinho.Valor("desconto").ToString();
                        art.STKActual = objListCarrinho.Valor("stkactual").ToString();
                        art.Preço = objListCarrinho.Valor("pcpadrao").ToString();
                        art.Familia = objListCarrinho.Valor("familia");
                        art.SubFamilia = objListCarrinho.Valor("subfamilia");
                        art.Marca = objListCarrinho.Valor("marca");
                        art.Modelo = objListCarrinho.Valor("modelo");

                        listArtigos.Add(art);
                        objListCarrinho.Seguinte();
                    }
                    //falta as imagens
                    pag.ID_Produtos = listArtigos;
                    objList.Seguinte();
                }

                /*
                 Consulta("SELECT CDU_idCarrinho, CDU_idCliente, CDU_idCarrinhoCompras, Artigo, Descricao, Desconto, STKActual, PCPadrao, Familia, SubFamilia, Marca, Modelo FROM  ARTIGO, TDU_idCarrinhoCompras, TDU_CarrinhoProduto WHERE CDU_idProduto = Artigo");

                 * 
                 * */


                return pag;

            }
            else
            {
                return null;

            }

        }


        #endregion Pagamento
    
    }
}