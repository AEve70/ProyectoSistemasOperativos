using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
using Compartido; // para MensajesIO

namespace AppClienteControlador
{
    public partial class Form1 : Form
    {
        private Cliente client;

        // Diccionario que vincula la acción del usuario con el comando real
        private readonly Dictionary<string, string> comandos = new Dictionary<string, string>()
        {
            {"Sistema Operativo", "GET_OS_INFO" },
            {"Nombre del Equipo", "GET_MACHINE_NAME" },
            {"Usuario Actual", "GET_USER" },
            {"Procesador", "GET_PROCESSOR" },
            {"Memoria RAM", "GET_RAM" },
            {"Unidades de Disco", "GET_DISKS" },
            {"Resolución de Pantalla", "GET_RESOLUTION" },
            {"Zona Horaria y Hora del Equipo", "GET_TIME" },
            {"Procesos en Ejecución", "GET_PROCESSES" }
        };

        public Form1()
        {
            InitializeComponent();
            client = new Cliente();
            llenarComboBox();
           
        }

       private void llenarComboBox()
        {
            cbx_datos.Items.Clear();
            cbx_datos.Items.AddRange(comandos.Keys.ToArray());
            cbx_datos.SelectedIndex = 0;
        }

        private async void btn_conectar_Click(object sender, EventArgs e)
        {
            string ip = txt_ip.Text.Trim();
            if (!int.TryParse(txt_puerto.Text, out int puerto))
            {
                MessageBox.Show("Ingrese un número de puerto válido.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

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

        private async void btn_consultar_Click(object sender, EventArgs e)
        {
            // Verificar conexión
            if (!client.Conectado)
            {
                MessageBox.Show("No hay conexión con el servidor.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (cbx_datos.SelectedItem == null)
            {
                MessageBox.Show("Seleccione una opción antes de consultar.");
                return;
            }

            string eleccion = cbx_datos.SelectedItem.ToString();
            string comando = comandos[eleccion];

            var solicitud = new MensajesIO(comando, true, null, "", "Cliente");
            await client.Enviar(solicitud);

            var respuesta = await client.Recibir();

            if (respuesta != null)
            {
                richTextBox1.Text = TraducirRespuesta(respuesta);
            }
            else
            {
                richTextBox1.Text = "No se recibió respuesta del servidor.";
            }
        }

        private string TraducirRespuesta(MensajesIO respuesta)
        {
            if (respuesta == null || respuesta.Datos == null)
                return "No se recibió información del servidor.";

            string comando = respuesta.Comando.ToUpper();
            string texto = "";

            try
            {
                var json = JsonSerializer.Serialize(respuesta.Datos);
                var datos = JsonSerializer.Deserialize<Dictionary<string, object>>(json);

                switch (comando)
                {
                    case "GET_OS_INFO":
                        texto = $"{datos["os_name"]} ({datos["platform"]})\nVersión: {datos["version"]}";
                        break;

                    case "GET_MACHINE_NAME":
                        texto = $"Nombre del equipo: {datos["machine_name"]}";
                        break;

                    case "GET_USER":
                        texto = $"Usuario activo: {datos["user"]}";
                        break;

                    case "GET_PROCESSOR":
                        texto = $"{datos["processor"]}\nNúcleos lógicos: {datos["logical_processors"]}";
                        break;

                    case "GET_RAM":
                        texto = $"Memoria RAM total: {datos["ram_total_gb"]} GB";
                        break;

                    case "GET_DISKS":
                        texto = "Unidades de disco detectadas:\n";

                        var jsonDiscos = JsonSerializer.Serialize(respuesta.Datos);
                        using (JsonDocument doc = JsonDocument.Parse(jsonDiscos))
                        {
                            foreach (var disco in doc.RootElement.EnumerateArray())
                            {
                                string name = disco.GetProperty("unidad").GetString();
                                string formato = disco.GetProperty("formato").GetString();
                                double total = disco.GetProperty("tamano_total_gb").GetDouble();
                                double usado = disco.GetProperty("usado_gb").GetDouble();
                                double libre = disco.GetProperty("disponible_gb").GetDouble();

                                texto += $"\n{name} ({formato}) → Total: {total} GB | Usado: {usado} GB | Libre: {libre} GB";
                            }
                        }
                        break;

                    case "GET_RESOLUTION":
                        texto = $"Resolución de pantalla: {datos["resolution"]}";
                        break;

                    case "GET_TIME":
                        texto = $"{datos["datetime"]}\nZona horaria: {datos["timezone"]}";
                        break;

                    case "GET_PROCESSES":
                        texto = "Procesos activos (máx. 200):\n";

                        // Convertimos el objeto Datos a JSON y lo parseamos
                        var jsonProcesos = JsonSerializer.Serialize(respuesta.Datos);
                        using (JsonDocument doc = JsonDocument.Parse(jsonProcesos))
                        {
                            foreach (var proc in doc.RootElement.EnumerateArray())
                            {
                                string nombre = proc.GetProperty("ProcessName").GetString();
                                int pid = proc.GetProperty("Id").GetInt32();
                                texto += $"- {nombre} (PID {pid})\n";
                            }
                        }
                        break;

                    default:
                        texto = "Comando no reconocido o sin formato definido.";
                        break;
                }
            }
            catch (Exception ex)
            {
                texto = $"Error al procesar datos: {ex.Message}";
            }

            return texto;
        }
    }
}
