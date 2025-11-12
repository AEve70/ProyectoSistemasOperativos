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
            //El ComboBox va a llenarse con las "Llaves" del diccionario
            cbx_datos.Items.Clear();
            cbx_datos.Items.AddRange(comandos.Keys.ToArray());
            cbx_datos.SelectedIndex = 0; // Seleccionar la primera opcion por defecto
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
        //Diccionario que vincula la accion que quiere el usuario con el comando que se ejecuta de forma interna
        private readonly Dictionary<String, String> comandos = new Dictionary<string, string>()
        {
            {"Sistema Operativo", "GET_OS_INFO" },
            {"Nombre del Equipo", "GET_MACHINE_NAME" },
            {"Usuario Actual", "GET_USER" },
            {"Procesador", "GET_PROCESSOR" },
            {"Memoria RAM", "GET_RAM" },
            {"Unidades de Disco", "GET_DISKS" },
            {"Resolucion Pantalla", "GET_RESOLUTION" },
            {"Zona Horaria y Hora del Equipo", "GET_TIME" },
            {"Procesos en Ejecución", "GET_PROCESSES" }

        };

        private void btn_consultar_Click(object sender, EventArgs e)
        {
            //El cliente va pedirle informacion al servidor
            if (!client.Conectado)
            {
                //Validar que haya conexion con el servidor antes de hacer cualquier consulta
                MessageBox.Show("No hay conexion con el servidor", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if(cbx_datos.SelectedItem == null)
            {
                MessageBox.Show("Seleccione una opción antes de consultar.");
                return;
            }

            String eleccion = cbx_datos.SelectedItem.ToString();
            String comando = comandos[eleccion];

            var solicitud = new MensajesIO()
        }
    }
}
