using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Timers;

namespace RedSunServerCheck
{
    public partial class Form1 : Form
    {

        public class ServerInfo
        {
            public string name { get; set; }
            public string gamemode { get; set; }
            public string map { get; set; }
            public int players { get; set; }
            public int maxplayers { get; set; }

            public static implicit operator ServerInfo(A2S_INFO server)
            {
                return new ServerInfo { name = server.Name, gamemode = server.Game, map = server.Map, maxplayers = server.MaxPlayers, players = server.Players };
            }
        }

        private System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();

        IPAddress raidenAddr = IPAddress.Parse("95.172.92.5");
        IPEndPoint raidenEndpoint;

        IPAddress armstrongAddr = IPAddress.Parse("192.223.24.11");
        IPEndPoint armstrongEndpoint;

        public Form1()
        {
            InitializeComponent();
            raidenEndpoint = new IPEndPoint(raidenAddr, 27015);
            armstrongEndpoint = new IPEndPoint(armstrongAddr, 27015);

            try
            {
                timer.Tick += new EventHandler(RequestInfoTimer);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
            }
            timer.Interval = 5000;
            timer.Start();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            RequestInfo();
        }

        public void RequestInfo()
        {
            A2S_INFO raidenInfo = new A2S_INFO(raidenEndpoint);
            A2S_INFO armstrongInfo = new A2S_INFO(armstrongEndpoint);

            textBox1.Text = "";
            FormatTextbox((ServerInfo)raidenInfo);
            FormatTextbox((ServerInfo)armstrongInfo);
        }

        void RequestInfoTimer(Object source, EventArgs e)
        {
            RequestInfo();
        }
            
        void FormatTextbox(ServerInfo info)
        {
            NewlineAppend(info.name + " - " + info.gamemode);
            NewlineAppend("    " + info.players.ToString() + "/" + info.maxplayers.ToString());
            NewlineAppend("    " + info.map);
            NewlineAppend("");
            NewlineAppend("");
        }

        void NewlineAppend(string line)
        {
            textBox1.AppendText(line);
            textBox1.AppendText(Environment.NewLine);
        }
    }
}
