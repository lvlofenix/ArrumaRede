using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net.NetworkInformation;
using System.Threading;

namespace ArrumaRede
{
    public partial class Detalhes : Form
    {
        public Detalhes()
        {
            InitializeComponent();
        }
        NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
        Thread at;
        

        private void Detalhes_Load(object sender, EventArgs e)
        {
            at = new Thread(new ThreadStart(atualizasa));
            at.Start();
        }

        public void atualizasa()
        {
            int a =1;                
                try
                {
                    while (a > 0)
                    {
                        Thread.Sleep(1000);
                        listBox1.Invoke((MethodInvoker)delegate { listBox1.Items.Clear(); });
                        listBox1.Invoke((MethodInvoker)delegate { listBox1.Items.Add("    NOME     /       DESCRIÇÃO      /   VELOCIDADE  /   STATUS /   BYTES RECEBIDOS  /   BYTES ENVIADOS"); });
                        for (int i = 0; i < nics.Length; i++)
                        {
                            long endValue = nics[i].GetIPv4Statistics().BytesReceived;
                            long iniValue = nics[i].GetIPv4Statistics().BytesSent;
                            listBox1.Invoke((MethodInvoker)delegate { listBox1.Items.Add(nics[i].Name.ToString() + " / " + nics[i].Description.ToString() + " / " + nics[i].Speed + " / " + nics[i].OperationalStatus + " / " + endValue + " / " + iniValue); });
                        }
                    }
                }
                catch
                {
                    at.Interrupt();
                    Close();                    
                }
            
        }
    }
}
