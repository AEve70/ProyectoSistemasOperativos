using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AppClienteControlador
{
    public partial class Form1 : Form
    {
        Cliente client;
        public Form1()
        {
            InitializeComponent();
            client = new Cliente();

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private async void btn_conectar_Click(object sender, EventArgs e)
        {
            string ip = txt_ip.Text.Trim();
            int puerto = int.Parse(txt_puerto.Text);

            label4.Text = "Conectando...";
            label4.ForeColor = Color.Goldenrod;
            btn_conectar.Enabled = false;

            bool ok = await client.Conectar(ip, puerto);

            if (ok)
            {
                label4.Text = "Conectado";
                label4.ForeColor = Color.ForestGreen;
            }
            else
            {
                label4.Text = "Error de conexión";
                label4.ForeColor = Color.Red;
            }

            btn_conectar.Enabled = true;
        }
    }
}
