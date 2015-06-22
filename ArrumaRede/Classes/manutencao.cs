using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Management;
using System.Drawing;
using System.Net.NetworkInformation;

namespace ArrumaRede.Classes
{
    class manutencao 
    {
        //função para realizar a manutenção.
        //recebe 3 bool para verificar os checks, 2 int dos saltos e segundos definidos pelo usuario

        public void manutRotina(bool ip, bool cache, bool renew, int saltos, int tempo,string endereco)
        {
            try
            {
                if (ip == true)
                {
                    ManagementClass objMC = new ManagementClass("Win32_NetworkAdapterConfiguration");
                    ManagementObjectCollection objMOC = objMC.GetInstances();
                    foreach (ManagementObject objMO in objMOC)
                    {
                        //pausa necessaria para dar tempo entre as execuções da thread. evita bugs. (necessario achar outra solução).
                        System.Threading.Thread.Sleep(TimeSpan.FromSeconds(1));
                        objMO.InvokeMethod("ReleaseDHCPLease", null, null);
                    }
                    outstring(" > CONEXÃO REVOGADA.    ", Color.Green);
                    //pausa necessaria para dar tempo entre as execuções da thread. evita bugs. (necessario achar outra solução).
                    System.Threading.Thread.Sleep(TimeSpan.FromSeconds(4));
                    ManagementClass objMC2 = new ManagementClass("Win32_NetworkAdapterConfiguration");
                    ManagementObjectCollection objMOC2 = objMC2.GetInstances();
                    foreach (ManagementObject objMO2 in objMOC2)
                    {
                        //pausa necessaria para dar tempo entre as execuções da thread. evita bugs. (necessario achar outra solução).
                        System.Threading.Thread.Sleep(TimeSpan.FromSeconds(1));
                        objMO2.InvokeMethod("RenewDHCPLease", null, null);
                    }
                    outstring(" > RENOVAÇÃO DA CONEXÃO FEITA COM SUCESSO.    ", Color.Green);
                }
                if (cache == true)
                {

                    System.Threading.Thread.Sleep(TimeSpan.FromSeconds(1));
                    outstring(" > LIMPEZA DE CACHE FEITA COM SUCESSO.    ", Color.Green);
                }
                if (renew == true)
                {
                    int i = 0;
                    outstring(" > INICIANDO TESTE DE PING.    ", Color.White);
                    while (i < saltos)
                    {
                        try
                        {
                            using (Ping p = new Ping())
                            {
                                System.Threading.Thread.Sleep(TimeSpan.FromSeconds(System.Convert.ToDouble(tempo)));
                                p.Send(endereco).Status.ToString();
                                outstring(" > " + p.Send(endereco).Status.ToString().ToUpper() + ": " + p.Send(endereco).RoundtripTime.ToString() + " MS/s    ", Color.Green);
                                i++;
                            }
                        }
                        catch
                        {
                            i++;
                            System.Threading.Thread.Sleep(TimeSpan.FromSeconds(System.Convert.ToDouble(tempo)));
                            outstring(" > FALHA: PACOTE PERDIDO!! :( ", Color.Red);;
                        }
                    }
                }
            }
            //lb_executar.Invoke((MethodInvoker)delegate { desativaexecuta(0); });
            //ativa = false;
            catch (Exception ex)
            {
                outstring(ex.Message, Color.Red);
            }
            throw new NotImplementedException();
        }

        //saida das informações para o usuario/log.
        public string outstring(string texto,Color cor)
        {
            return texto;
            throw new NotImplementedException();
        }
    }
}
