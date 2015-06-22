using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net.Cache;
using System.Net;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.IO;
using System.Threading;
using System.Management;
using System.Diagnostics;
using ArrumaRede.Classes;
using System.Media;

namespace ArrumaRede
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            carregacads();
            this.MouseDown += new MouseEventHandler(Form3_MouseDown);
            this.MouseMove += new MouseEventHandler(Form3_MouseMove);

        }
        int contlog = 0;
        Thread t;
        Thread tp;
        Thread tv;
        int X = 0;  
        int Y = 0;
        string ipmanu = "";
        Detalhes detalhe;
        About about;
        Boolean ativa = false;
        NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
        int ativacampo = 0;
        private void Form3_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;
            X = this.Left - MousePosition.X;
            Y = this.Top - MousePosition.Y;
        }
        //move o form 
        private void Form3_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;
            this.Left = X + MousePosition.X;
            this.Top = Y + MousePosition.Y;
        }
        public void carregacads()
        {
            for (int i = 0; i < nics.Length; i++)
            {
                if (nics[i].OperationalStatus.ToString() == "Down")
                {
                }
                else
                {
                    listBox1.Items.Add(nics[i].Name.ToString() + " / "  + nics[i].Description.ToString());
                }
            }
            Refresh();
        }

        public void ativaGeral()
        {
            nu_seg.Enabled = true;
            nd_saltos.Enabled = true;
            if (cb_testaping.Checked)
            {
                cb_testaping.Enabled = true;
                tb_ipmanu.Enabled = true;
            }
            else
            {
                cb_testaping.Enabled = true;
            }
            cb_renovarIP.Enabled = true;
            cb_cache.Enabled = true;
        }

        public void desativaGeral()
        {
            if (cb_renovarIP.Enabled)
            {
                cb_renovarIP.Enabled = false;
            }
            if(cb_testaping.Enabled)
            {
                cb_testaping.Enabled = false;
            }
            if (cb_cache.Enabled)
            {
                cb_cache.Enabled = false;
            }
            if(nd_saltos.Enabled)
            {
                nd_saltos.Enabled = false;
            }
            if(nu_seg.Enabled)
            {
                nu_seg.Enabled = false;
            }
            if(tb_ipmanu.Enabled)
            {
                tb_ipmanu.Enabled = false;
            }

        }
        private void Form1_Load(object sender, EventArgs e)
        {
            PegarInfor();
        }

        public void novaThreadProgm()
        {
            tp = new Thread(new ThreadStart(manutencaoProgramda));
        }

        public void novaThreadVerifica()
        {
            tv = new Thread(new ThreadStart(verificaSite));
        }

        public void verificaSite()
        {
            if (tb_site.Text == "" || tb_site.Text.Length < 6)
            {
                MessageBox.Show("Por Favor preencha o site corretamente. EXEMPLO \n google.com.br","ERRO!!",MessageBoxButtons.OK,MessageBoxIcon.Exclamation);
                tv.Interrupt();
            }
            else
            {
                using (Ping p = new Ping())
                {
                    tb_log2.Invoke((MethodInvoker)delegate { Deltxt(" > VERIFICANDO SITE...  ",Color.White); });
                    try
                    {
                        System.Threading.Thread.Sleep(TimeSpan.FromSeconds(System.Convert.ToDouble(nu_seg.Value)));
                        p.Send(tb_site.Text).Status.ToString();
                        tb_log2.Invoke((MethodInvoker)delegate { Deltxt(" > SITE "+tb_site.Text + " ESTA ONLINE!!    ",Color.Green); });
                        tv.Interrupt();
                    }
                    catch
                    {
                        tb_log2.Invoke((MethodInvoker)delegate { Deltxt(" > ERRO! SITE " + tb_site.Text + " ESTA OFFLINE OU NÃO EXISTE!! ",Color.Red); });
                        tv.Interrupt();
                    }
                }
            }
        }

        //ativa ou desativa o programa.
        private void label15_Click(object sender, EventArgs e)
        {
            if (lb_ativar.Text == "ATIVAR")
            {
                lb_ativar.Text = "DESATIVAR";
                lb_ativar.ForeColor = System.Drawing.Color.Red;
                Deltxt(" > INICIANDO PROGRAMAÇÃO DE MONITORAMENTO DA REDE.  ",Color.White);
                desativaProgm(1);
                novaThreadProgm();
                tp.Start();
                
            }
            else
            {
                tp.Interrupt();
                lb_ativar.Text = "ATIVAR";
                desativaProgm(0);
                this.lb_ativar.ForeColor = System.Drawing.Color.Green;
                contlog++;
                Deltxt(" > DESATIVADO MONITORAMENTO.    ", Color.White);
                tb_log2.SelectionStart = tb_log2.Text.Length;
                tb_log2.ScrollToCaret();
            }

        }

        public void desativaProgm(int v)
        {
            if (v == 1)
            {
                cb_renovaiph.Enabled = false;
                tb_ipmonitor.Enabled = false;
                nu_min_program.Enabled = false;
                cb_manuml.Enabled = false;
                nu_mileprogam.Enabled = false;
                cb_lost.Enabled = false;
                cb_limpacacheh.Enabled = false;
                nu_alertaping.Enabled = false;
                nu_minchace.Enabled = false;
                label12.Enabled = false;
                label18.Enabled = false;
                label14.Enabled = false;
                label11.Enabled = false;
            }
            else
            {
                cb_renovaiph.Enabled = true;
                tb_ipmonitor.Enabled = true;
                nu_min_program.Enabled = true;
                cb_manuml.Enabled = true;
                nu_mileprogam.Enabled = true;
                cb_lost.Enabled = true;
                cb_limpacacheh.Enabled = true;
                nu_alertaping.Enabled = true;
                label14.Enabled = true;
                nu_minchace.Enabled = true;
                label12.Enabled = true;
                label18.Enabled = true;
                label11.Enabled = true;
            }

        }
        //botões
        //limpa log
        private void label10_Click(object sender, EventArgs e) { tb_log2.Text = ""; contlog = 0; }
        //chebox testa ip 
        private void cb_testaping_CheckedChanged(object sender, EventArgs e)
        {
            if (cb_testaping.Checked)
                tb_ipmanu.Enabled = true;
            else
                tb_ipmanu.Enabled = false;
        }
        //carrega as informações do pc
        public void PegarInfor()
        {
            try
            {
                //ip interno.
                IPHostEntry IPHost = Dns.GetHostByName(Dns.GetHostName());
                lb_ipatual.Invoke((MethodInvoker)(delegate { lb_ipatual.Text = "IP ATUAL: " + IPHost.AddressList[0].ToString(); }));
                //ip externo.
                lb_ipinternet.Invoke((MethodInvoker)(delegate { lb_ipinternet.Text = "IP DE INTERNET: " + GetPublicIP(); }));
                if (lb_ipinternet.Text == "IP DE INTERNET: FALHOU")
                {
                    lb_ipinternet.Invoke((MethodInvoker)(delegate { lb_ipinternet.ForeColor = System.Drawing.Color.Red; }));
                    lb_status.Invoke((MethodInvoker)(delegate { lb_status.ForeColor = System.Drawing.Color.Red; }));
                    lb_status.Invoke((MethodInvoker)(delegate { lb_status.Text = "STATUS: ERRO!!"; }));
                }
                else
                {
                    lb_ipinternet.Invoke((MethodInvoker)(delegate { lb_ipinternet.ForeColor = System.Drawing.Color.WhiteSmoke; }));
                    lb_status.Invoke((MethodInvoker)(delegate { lb_status.ForeColor = System.Drawing.Color.Green; }));
                    lb_status.Invoke((MethodInvoker)(delegate { lb_status.Text = "STATUS: OK!!"; }));
                }
                tb_log2.Invoke((MethodInvoker)(delegate { Deltxt(" > CONEXÕES EXAMINADAS.", Color.White); }));
                tb_log2.Invoke((MethodInvoker)(delegate{tb_log2.SelectionStart = tb_log2.Text.Length;}));
                tb_log2.ScrollToCaret();

            }
            catch
            {
                lb_status.Text = "STATUS: ERRO!!";
                Deltxt(" > ERRO NA VERIFICAÇÃO DA CONEXÃO.    ", Color.Red);
                tb_log2.SelectionStart = tb_log2.Text.Length;
                tb_log2.ScrollToCaret();
                lb_status.ForeColor = System.Drawing.Color.Red;
            }

        }
        //pega o ip publico.
        public string GetPublicIP()
        {
            try
            {
                String direction = "";
                WebRequest request = WebRequest.Create("http://checkip.dyndns.org/");
                using (WebResponse response = request.GetResponse())
                using (StreamReader stream = new StreamReader(response.GetResponseStream()))
                {
                    direction = stream.ReadToEnd();
                }

                //Search for the ip in the html
                int first = direction.IndexOf("Address: ") + 9;
                int last = direction.LastIndexOf("</body>");
                direction = direction.Substring(first, last - first);
                return direction;
            }
            catch
            {
                lb_ipinternet.ForeColor = Color.Red;
                return "FALHOU";
            }
        }

        public void Deltxt(String txt,Color cor)
        {
            contlog++;
            txt = contlog + txt + DateTime.Now.ToString();
            int length = tb_log2.TextLength;  // at end of text
            tb_log2.AppendText(Environment.NewLine + txt);
            tb_log2.SelectionStart = length;
            tb_log2.SelectionLength = txt.Length+1;
            tb_log2.SelectionColor = cor;
            tb_log2.ScrollToCaret();

        }


        //eventos executar manutenção simples.
        public void desativaexecuta(int qual)
        {
            if (qual == 1)
            {
                lb_executar.Text = "DESATIVAR";
                this.lb_executar.ForeColor = System.Drawing.Color.Red;
            }
            else
            {
                lb_executar.Text = "EXECUTAR";
                this.lb_executar.ForeColor = System.Drawing.Color.Green;
                ativaGeral();
                
            }
        }

        public void criaThread()
        {
            t = new Thread(new ThreadStart(manutencao));
        }

        private void lb_executar_Click(object sender, EventArgs e)
        {
            try
            {
                if (ativa == false)
                {
                    criaThread();
                    desativaGeral();
                    desativaexecuta(1);
                    ativa = true;
                    t.Start();

                }
                else
                {
                    t.Abort();
                    ativaGeral();
                    desativaexecuta(0);
                    ativa = false;
                    Deltxt(" > VERIFICAÇÃO CANCELADA.", Color.White);
                }
            }
            catch (Exception ea)
            {
                MessageBox.Show("Algo deu errado: " + ea, "ERRO!!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        public void manutencao()
        {
            try
            {
                if (!tb_log2.InvokeRequired & lb_executar.InvokeRequired)
                {
                    MessageBox.Show("O sistema esta ocupado. Um instante!");
                }
                else
                {
                    lb_executar.Invoke((MethodInvoker)delegate { desativaexecuta(1); });
                    tb_log2.Invoke((MethodInvoker)delegate { Deltxt(" > MANUTENÇÃO INICIADA.    ",Color.White); });
                    tb_log2.Invoke((MethodInvoker)delegate { Deltxt(" > AGUARDE ALGUNS INSTANTES...    ",Color.White); });
                    if (cb_renovarIP.Checked)
                    {
                        ManagementClass objMC = new ManagementClass("Win32_NetworkAdapterConfiguration");
                        ManagementObjectCollection objMOC = objMC.GetInstances();
                        foreach (ManagementObject objMO in objMOC)
                        {
                            Thread.Sleep(99);
                            objMO.InvokeMethod("ReleaseDHCPLease", null, null);
                        }
                        tb_log2.Invoke((MethodInvoker)delegate { Deltxt(" > CONEXÃO REVOGADA.    ",Color.Green); });
                        Thread.Sleep(4500);
                        ManagementClass objMC2 = new ManagementClass("Win32_NetworkAdapterConfiguration");
                        ManagementObjectCollection objMOC2 = objMC2.GetInstances();
                        foreach (ManagementObject objMO2 in objMOC2)
                        {
                            Thread.Sleep(99);
                            objMO2.InvokeMethod("RenewDHCPLease", null, null);
                        }
                        tb_log2.Invoke((MethodInvoker)delegate { Deltxt(" > RENOVAÇÃO DA CONEXÃO FEITA COM SUCESSO.    ",Color.Green); });
                    }
                    if (cb_cache.Checked)
                    {
                        FlushMyCache();
                        System.Threading.Thread.Sleep(TimeSpan.FromSeconds(1));
                        tb_log2.Invoke((MethodInvoker)delegate { Deltxt(" > LIMPEZA DE CACHE FEITA COM SUCESSO.    ",Color.Green); });
                    }
                    if (cb_testaping.Checked)
                    {
                        int i = 0;
                        tb_log2.Invoke((MethodInvoker)delegate { Deltxt(" > INICIANDO TESTE DE PING.    ",Color.White); });
                        while (i < nd_saltos.Value)
                        {
                            try
                            {
                                using (Ping p = new Ping())
                                {
                                    string ipando = "";
                                    tb_ipmanu.Invoke((MethodInvoker)delegate { ipando = ipping(); });
                                    System.Threading.Thread.Sleep(TimeSpan.FromSeconds(System.Convert.ToDouble(nu_seg.Value)));
                                    p.Send(ipando).Status.ToString();
                                    tb_log2.Invoke((MethodInvoker)delegate { Deltxt(" > " + p.Send(ipando).Status.ToString().ToUpper() + ": " + p.Send(ipando).RoundtripTime.ToString() + " MS/s    ",Color.Green); });
                                    i++;
                                }
                            }
                            catch (Exception ee)
                            {
                                i++;
                                System.Threading.Thread.Sleep(TimeSpan.FromSeconds(System.Convert.ToDouble(nu_seg.Value)));
                                tb_log2.Invoke((MethodInvoker)delegate { Deltxt(" > FALHA: PACOTE PERDIDO!! :( ",Color.Red); });
                            }
                        }
                    }
                }
                lb_executar.Invoke((MethodInvoker)delegate { desativaexecuta(0); });
                ativa = false;
            }
            catch (Exception eae)
            {
            }
        }
        //dll de flush dns
        [DllImport("dnsapi.dll", EntryPoint = "DnsFlushResolverCache")]
        private static extern UInt32 DnsFlushResolverCache();
        public static void FlushMyCache() //This can be named whatever name you want and is the function you will call
        {
            UInt32 result = DnsFlushResolverCache();
        }
        //fim dos eventos da manutneçãos simples.

        private void cb_renovaiph_CheckedChanged(object sender, EventArgs e)
        {
            if (cb_renovaiph.Checked)
            {
                nu_min_program.Enabled = true;
            }
            else
            {
                nu_min_program.Enabled = false;
            }
        }

        private void cb_manuml_CheckedChanged(object sender, EventArgs e)
        {
            if (tb_ipmonitor.Text == "" || tb_ipmonitor.Text == "DIGITE OU ESCOLHA")
            {
                cb_manuml.Checked = false;
                MessageBox.Show("Escolha um IP para comparação", "ERRO!!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else
            {
                if (cb_manuml.Checked)
                {
                    nu_mileprogam.Enabled = true;
                    nu_alertaping.Enabled = true;
                }
                else
                {
                    nu_mileprogam.Enabled = false;
                    nu_alertaping.Enabled = false;
                }
            }
        }

        private void cb_limpacacheh_CheckedChanged(object sender, EventArgs e)
        {
            if (cb_limpacacheh.Checked)
            {
                nu_minchace.Enabled = true;
            }
            else
            {
                nu_minchace.Enabled = false;
            }
        }

        private void label4_Click(object sender, EventArgs e)
        {
            PegarInfor();
        }

        public void manutencaoProgramda()
        {
            try
            {
                int tempo1 = 0;
                int tempo2 = 0;
                int tempo3 = 0;
                int delay = 0;
                ippingdois();
                decimal minrenovacon = nu_min_program.Value;
                decimal minmanuping = nu_mileprogam.Value;
                decimal mincacherenov = nu_minchace.Value;

                if (cb_renovaiph.Checked) { delay = 55302;}else { delay = 60000; }
                while (tempo1 > -1)
                {
                    Thread.Sleep(delay);
                    tempo1++;
                    tempo2++;
                    tempo3++;
                    //programação de renovar conexão.
                    if (cb_renovaiph.Checked & tempo1 == Math.Truncate(nu_min_program.Value))
                    {
                        ManagementClass objMC = new ManagementClass("Win32_NetworkAdapterConfiguration");
                        ManagementObjectCollection objMOC = objMC.GetInstances();
                        foreach (ManagementObject objMO in objMOC)
                        {
                            Thread.Sleep(99);
                            objMO.InvokeMethod("ReleaseDHCPLease", null, null);
                        }
                        tb_log2.Invoke((MethodInvoker)delegate { Deltxt(" > CONEXÃO REVOGADA.    ",Color.Green); });
                        Thread.Sleep(4500);
                        ManagementClass objMC2 = new ManagementClass("Win32_NetworkAdapterConfiguration");
                        ManagementObjectCollection objMOC2 = objMC2.GetInstances();
                        foreach (ManagementObject objMO2 in objMOC2)
                        {
                            Thread.Sleep(99);
                            objMO2.InvokeMethod("RenewDHCPLease", null, null);
                        }
                        tb_log2.Invoke((MethodInvoker)delegate { Deltxt(" > RENOVAÇÃO DA CONEXÃO FEITA COM SUCESSO.    ",Color.Green); });
                        tempo1 = 0;
                    }
                    //programação de cache.
                    if (cb_limpacacheh.Checked & tempo2 == Math.Truncate(nu_minchace.Value))
                    {
                        FlushMyCache();
                        tb_log2.Invoke((MethodInvoker)delegate { Deltxt(" > CACHE LIMPO COM SUCESSO.    ",Color.Green); });
                        tempo2 = 0;
                    }
                    //verifica se ele perder a conexão.
                    if (cb_lost.Checked)
                    {
                        try
                        {
                            using (Ping p = new Ping())
                            {
                                p.Send(ipmanu).Status.ToString();
                                tb_log2.Invoke((MethodInvoker)delegate { Deltxt(" > CONEXÃO COM O SITE/IP INFORMADO OK.",Color.Green); });
                            }
                        }
                        catch (Exception ee)
                        {
                            MessageBox.Show(ee + "");
                            SoundPlayer simpleSound = new SoundPlayer(@"C:\Users\lucas\Downloads\alert.wav");
                            simpleSound.Play();
                            tb_log2.Invoke((MethodInvoker)delegate { Deltxt(" > FALHA: CONEXÃO COM O SITE/IP INFORMADO. FORA DO AR OU NÃO EXISTE. (ALERTA SONORO ATIVADO).   ",Color.Red); });
                        }
                    }
                    if (cb_manuml.Checked & tempo3 == Math.Truncate(nu_alertaping.Value))
                    {
                        tempo3 = 0;
                        try
                        {
                            using (Ping p = new Ping())
                            {
                                p.Send(ipmanu).Status.ToString();
                                if (p.Send(ipmanu).RoundtripTime >= Math.Truncate(nu_alertaping.Value))
                                {
                                    SoundPlayer simpleSound = new SoundPlayer(@"C:\Users\lucas\Downloads\alert.wav");
                                    simpleSound.Play();
                                    tb_log2.Invoke((MethodInvoker)delegate { Deltxt(" > ATENÇÃO! DELAY DE: " + p.Send(ipmanu).RoundtripTime.ToString() + " / "+  p.Send(ipmanu).Status.ToString() + "(ALERTA SONORO)    ",Color.Red); });
                                }
                                else
                                    tb_log2.Invoke((MethodInvoker)delegate { Deltxt(" > DELAY DE APENAS: " + p.Send(ipmanu).RoundtripTime.ToString() + " / " + p.Send(ipmanu).Status.ToString() + " - DENTRO DO LIMITE.   ", Color.Green); });
                            }
                        }
                        catch
                        {
                            tb_log2.Invoke((MethodInvoker)delegate { Deltxt(" > NÃO FOI POSSIVEL VERIFICAR O DELAY DO IP/SITE INFORMADO./ "+ ipmanu,Color.Red); });
                        }

                    }
                }
            }
            catch (Exception errop)
            {
                MessageBox.Show(errop + "");
            }

            
        }

        public string ipping()
        {
            if (tb_ipmanu.Text == "DNS GOOGLE")
            {
                return "8.8.8.8";
            }
            else
            {
                if (tb_ipmanu.Text == "FACEBOOK")
                {
                    return "173.252.120.6";
                }
                else
                {
                    if (tb_ipmanu.Text == "UOL BRASIL")
                    {
                        return "200.221.2.45";
                    }
                    else
                    {
                        if (tb_ipmanu.Text == "SERVIDOR GVT EM DF")
                        {
                            return "200.175.182.139";
                        }
                        else
                        {
                            if (tb_ipmanu.Text == "SERVIDOR EM SP")
                            {
                                return "217.150.156.33";
                            }
                            else
                            {
                                if (tb_ipmanu.Text == "SERVIDOR EM PR")
                                {
                                    return "189.76.176.4";
                                }
                                else
                                {
                                    return tb_ipmanu.Text;
                                }
                            }
                        }
                    }
                }
            }

        }

        public void ippingdois()
        {
            tb_ipmanu.Invoke((MethodInvoker)delegate {  
            if (tb_ipmonitor.Text == "DNS GOOGLE")
            {
                ipmanu = "8.8.8.8";
            }
            else
            {
                if (tb_ipmonitor.Text == "FACEBOOK")
                {
                    ipmanu = "173.252.120.6";
                }
                else
                {
                    if (tb_ipmonitor.Text == "UOL BRASIL")
                    {
                        ipmanu = "200.221.2.45";
                    }
                    else
                    {
                        if (tb_ipmonitor.Text == "SERVIDOR GVT EM DF")
                        {
                            ipmanu = "200.175.182.139";
                        }
                        else
                        {
                            if (tb_ipmonitor.Text == "SERVIDOR EM SP")
                            {
                                ipmanu = "217.150.156.33";
                            }
                            else
                            {
                                if (tb_ipmonitor.Text == "SERVIDOR EM PR")
                                {
                                    ipmanu = "189.76.176.4";
                                }
                                else
                                {
                                    ipmanu = tb_ipmonitor.Text;
                                }
                            }
                        }
                    }
                }
            }
            });

        }
        private void label13_Click(object sender, EventArgs e)
        {
            novaThreadVerifica();
            tv.Start();

        }

        private void tb_ipmanu_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        private void label8_Click(object sender, EventArgs e)
        {
            detalhe = new Detalhes();
            detalhe.Show();
        }

        private void groupBox2_Enter(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {
            about = new About();
            about.Show();
        }
    }
}