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

namespace SINF_proj.Lib_Primavera
{
    public class ClienteGes
    {

        # region Cliente

        public static Model.Cliente loginUtilizador(string nif, string password)
        {
            ErpBS objMotor = new ErpBS();
            StdBELista objList;

            Model.Cliente cli = null;

            if (PriEngine.InitializeCompany("BELAFLOR", "", "") == true)
            {
                objList = PriEngine.Engine.Consulta("SELECT Cliente, Nome, Moeda, Fac_Mor, Fac_Local, Fac_Cp, Fac_Cploc, Fac_Tel, NumContrib as NumContribuinte, CDU_CampoVar2 as Tipo FROM CLIENTES where (NumContrib = '" + nif + "' and CDU_CampoVar1='" + password + "')");

                while (!objList.NoFim())
                {
                    cli = new Model.Cliente();
                    cli.CodCliente = objList.Valor("Cliente");
                    cli.NomeCliente = objList.Valor("Nome");
                    cli.Moeda = objList.Valor("Moeda");
                    cli.NumContribuinte = objList.Valor("NumContribuinte");
                    cli.Morada = objList.Valor("Fac_Mor");
                    cli.Local = objList.Valor("Fac_Local");
                    cli.Tipo = Convert.ToInt16(objList.Valor("Tipo"));
                    if (objList.Valor("Fac_Cploc") != "")
                    {
                        cli.Codigo_Postal = objList.Valor("Fac_Cp") + "-" + objList.Valor("Fac_Cploc");
                    }
                    else
                    {
                        cli.Codigo_Postal = objList.Valor("Fac_Cp") + "-000";
                    }
                    cli.Telefone = objList.Valor("Fac_Tel");

                    objList.Seguinte();
                }

                return cli;
            }
            else
            {
                return null;
            }
        }


        public static List<Model.Cliente> getUtilizadores()
        {
            ErpBS objMotor = new ErpBS();
            //MotorPrimavera mp = new MotorPrimavera();
            StdBELista objList;

            Model.Cliente cli = new Model.Cliente();
            List<Model.Cliente> listClientes = new List<Model.Cliente>();


            if (PriEngine.InitializeCompany("BELAFLOR", "", "") == true)
            {

                //objList = PriEngine.Engine.Comercial.Clientes.LstClientes();

                objList = PriEngine.Engine.Consulta("SELECT Cliente, Nome, Moeda, Fac_Mor, Fac_Local, Fac_Cp, Fac_Cploc, Fac_Tel, NumContrib as NumContribuinte FROM CLIENTES");

                while (!objList.NoFim())
                {
                    cli = new Model.Cliente();
                    cli.CodCliente = objList.Valor("Cliente");
                    cli.NomeCliente = objList.Valor("Nome");
                    cli.Moeda = objList.Valor("Moeda");
                    cli.NumContribuinte = objList.Valor("NumContribuinte");
                    cli.Morada = objList.Valor("Fac_Mor");
                    cli.Local = objList.Valor("Fac_Local");
                    if (objList.Valor("Fac_Cploc") != "")
                    {
                        cli.Codigo_Postal = objList.Valor("Fac_Cp") + "-" + objList.Valor("Fac_Cploc");
                    }
                    else
                    {
                        cli.Codigo_Postal = objList.Valor("Fac_Cp") + "-000";
                    }
                    cli.Telefone = objList.Valor("Fac_Tel");

                    listClientes.Add(cli);
                    objList.Seguinte();

                }

                return listClientes;
            }
            else
                return null;
        }

        public static Lib_Primavera.Model.Cliente getInfoUtilizador(string codCliente)
        {
            List<Model.Cliente> clientes = getUtilizadores();
            Model.Cliente retorno = null;

            for (int i = 0; i < clientes.Count(); i++)
            {
                if (clientes[i].CodCliente.Equals(codCliente))
                {
                    retorno = clientes[i];
                    break;
                }
            }

            return retorno;
            
        }

        public static Tuple<Model.Cliente, List<Model.Encomenda>> getDetalhesUtilizador(string codCliente)
        {
            Tuple<Model.Cliente, List<Model.Encomenda>> user;
            user = new Tuple<Model.Cliente,List<Model.Encomenda>>(getInfoUtilizador(codCliente), EncomendasGes.getEncomendas(codCliente, 1000));

            return user;
        }

        # endregion Cliente;
    }
}