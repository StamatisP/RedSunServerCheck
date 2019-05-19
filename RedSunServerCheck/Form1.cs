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
using Newtonsoft.Json;
using System.IO;

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

        /*public class JsonGamemodes
        {
            public string gamemode { get; set; }
            public bool enabled { get; set; }
        }*/

        private System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();

        IPAddress raidenAddr = IPAddress.Parse("95.172.92.5");
        IPEndPoint raidenEndpoint;
        A2S_INFO raidenInfo;
        
        IPAddress armstrongAddr = IPAddress.Parse("192.223.24.11");
        IPEndPoint armstrongEndpoint;
        A2S_INFO armstrongInfo;

        string[] preferredGamemodes = { "Versus Saxton Hale", "Dodgeball", "Randomizer", "Team Fortress", "Stop That Tank", "Tank Race", "Smash Fortress", "Freeze Tag", "Glass Attack", "Team Battles" };
        int preferredPlayers = 2;

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
            timer.Interval = (int) numericUpDown1.Value * 1000;
            timer.Start();
            if (File.Exists(Directory.GetCurrentDirectory() + @"\prefs.json"))
            {
                LoadPreferences();
            }
        }

        void Form1_Load(object sender, System.EventArgs e)
        {

        }

        bool CheckIfFilterMatch(ServerInfo info)
        {
            /*if (info.gamemode == filter.gamemode && info.players >= filter.players) {
                return true;
            }*/
            
            if (checkedListBox1.CheckedItems.Contains(info.gamemode) && info.players >= preferredPlayers)
            {
                Console.WriteLine("server passed filter");
                return true;
            }
            Console.WriteLine("server failed filter, " + info.gamemode + " " + info.players + " >= " + preferredPlayers);
            return false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            RequestInfo();
        }

        public void RequestInfo()
        {
            raidenInfo = new A2S_INFO(raidenEndpoint);
            armstrongInfo = new A2S_INFO(armstrongEndpoint);

            textBox1.Text = "";
            FormatTextbox((ServerInfo)raidenInfo);
            FormatTextbox((ServerInfo)armstrongInfo);
            if (CheckIfFilterMatch(raidenInfo))
            {
                System.Media.SoundPlayer player = new System.Media.SoundPlayer(@"C:\Users\Stamos\source\repos\RedSunServerCheck\RedSunServerCheck\no.wav");
                player.Play();
                Console.WriteLine("oh hell yea server available");
            }
            if (CheckIfFilterMatch(armstrongInfo))
            {
                System.Media.SoundPlayer player = new System.Media.SoundPlayer(@"C:\Users\Stamos\source\repos\RedSunServerCheck\RedSunServerCheck\no.wav");
                player.Play();
                Console.WriteLine("oh hell yea server available");
            }
        }

        void RequestInfoTimer(Object source, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                RequestInfo();
            }
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

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            timer.Interval = (int) numericUpDown1.Value * 1000;
        }

        void SavePreferences()
        {
            string output = JsonConvert.SerializeObject(checkedListBox1.CheckedItems);
            File.WriteAllText(Directory.GetCurrentDirectory() + @"\prefs.json", output);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            SavePreferences();
            Console.WriteLine("closing");
        }

        void LoadPreferences()
        {
            var input = JsonConvert.DeserializeObject<string[]>(File.ReadAllText(Directory.GetCurrentDirectory() + @"\prefs.json"));
            
            for (int i = 0; i < input.Count(); i++)
            {
                //checkedListBox1.SetItemChecked(i, true);
                checkedListBox1.SetItemChecked(checkedListBox1.FindString(input[i]), true);
            }
            
        }
    }
}
