using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SharpAis;

namespace AIS_Testbed
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        public Boolean stop = false;
        static private string getData(string Address, Int32 Port)
        {
            Application.DoEvents();
            System.Net.IPAddress serverAddress = System.Net.IPAddress.Parse(Address);
            TcpClient tcpClient = new TcpClient();
            tcpClient.Connect(serverAddress, Port);
            NetworkStream ourStream = tcpClient.GetStream();
            byte[] data = new byte[tcpClient.ReceiveBufferSize];         
            // Read() is a blocking call
            int bytesRead = ourStream.Read(data, 0, System.Convert.ToInt32(tcpClient.ReceiveBufferSize));
           string strData = Encoding.ASCII.GetString(data, 0, bytesRead);
            return strData;
        }
        private void Delay(long delay)
        {
            DateTime tthen = DateTime.Now;
            do
            {
                Application.DoEvents();
            } while (tthen.AddSeconds(delay) > DateTime.Now);
        }
        private void button1_Click(object sender, EventArgs e)
        {
            string line;
            Int32 port=2233;
            string target="127.0.0.1";
            Int64 numMessages = 0;

            if (tbIPAddress.Text == "")
            {
                MessageBox.Show("Please enter a valid IP address");
            }
            else
            {
                target = tbIPAddress.Text;
            }
            if (tbPort.Text == "")
            {
                MessageBox.Show("Please enter a valid port number");
            }
            else
            {
                port = Convert.ToInt32(tbPort.Text);
            }
            while ((line = getData(target, port)) != null)
            {
                if (stop)
                {
                    stop = false;
                    return;
                }
                Delay(Convert.ToInt64(tbDelay.Text));
                var ht = new Hashtable();
                var AisParser = new Parser();
                ht = AisParser.Parse(line);
                if (ht!=null)
                { 
                foreach (DictionaryEntry entry in ht)
                {
                        if (entry.Key.ToString()=="MMSI")
                        { tbMessage.Text += Environment.NewLine;
                          tbMessage.Text += Environment.NewLine;
                          lblNumMessages.Text = Convert.ToString(numMessages++);
                        }
                        tbMessage.Text+= entry.Key + "  :  " + entry.Value.ToString() + Environment.NewLine;

                    }
                }
                
               
            }
        }

        private void btnStopCapture_Click(object sender, EventArgs e)
        {
            stop = true;
        }
    }
    
}
