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

        bool ControlRemoto;
        public Form1()
        {
            InitializeComponent();
            client = new Cliente();
            llenarComboBox();
            //Etiqueta que indica estado
            label4.Text = "Desconectado";
            label4.ForeColor = Color.Coral;
            ControlRemoto = false;
            
        }

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

            //Validar que haya seleccionado una opcion del combo box
            if (cbx_datos.SelectedItem == null)
            {
                MessageBox.Show("Seleccione una opción antes de consultar.");
                return;
            }

            //El usuario elije y luego se manda la solicitud al servidor, si el servidor responde se recibe una repsuesta y se muestra en el text box
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

        private async void btn_subirVolumen_Click(object sender, EventArgs e)
        {
            if (!client.Conectado)
            {
                MessageBox.Show("No hay conexion activa", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var solicitud = new MensajesIO("VOL_UP", true, null, "", "Cliente");
            await client.Enviar(solicitud);

            var respuesta = await client.Recibir();
            richTextBox1.Text = respuesta?.Mensaje ?? "OK"; 
        }

        private async void btn_bajarVolumen_Click(object sender, EventArgs e)
        {
            if (!client.Conectado)
            {
                MessageBox.Show("No hay conexion activa", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var solicitud = new MensajesIO("VOL_DOWN", true, null, "", "Cliente");
            await client.Enviar(solicitud);

            var respuesta = await client.Recibir();
            richTextBox1.Text = respuesta?.Mensaje ?? "OK";
        }

        private async void btn_silenciar_Click(object sender, EventArgs e)
        {
            if (!client.Conectado)
            {
                MessageBox.Show("No hay conexion activa", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var solicitud = new MensajesIO("MUTE", true, null, "", "Cliente");
            await client.Enviar(solicitud);

            var respuesta = await client.Recibir();
            richTextBox1.Text = respuesta?.Mensaje ?? "OK";
        }



        //Se usa para traducir respuestas complejas es decir multilinea
        private string TraducirRespuesta(MensajesIO respuesta)
        {
            if (respuesta == null || respuesta.Datos == null)
                return "No se recibió información del servidor.";

            string comando = respuesta.Comando.ToUpper();
            string texto = "";

            try
            {
                // Convertimos a JSON string para leerlo
                var json = JsonSerializer.Serialize(respuesta.Datos);
                JsonElement elemento = JsonSerializer.Deserialize<JsonElement>(json);

                switch (comando)
                {
                    case "GET_OS_INFO":
                        texto = $"{elemento.GetProperty("os_name").GetString()} " +
                                $"({elemento.GetProperty("platform").GetString()})\n" +
                                $"Versión: {elemento.GetProperty("version").GetString()}";
                        break;

                    case "GET_MACHINE_NAME":
                        texto = $"Nombre del equipo: {elemento.GetProperty("machine_name").GetString()}";
                        break;

                    case "GET_USER":
                        texto = $"Usuario activo: {elemento.GetProperty("user").GetString()}";
                        break;

                    case "GET_PROCESSOR":
                        texto = $"{elemento.GetProperty("processor").GetString()}\n" +
                                $"Núcleos lógicos: {elemento.GetProperty("logical_processors").GetInt32()}";
                        break;

                    case "GET_RAM":
                        texto = $"Memoria RAM total: {elemento.GetProperty("ram_total_gb").GetDouble()} GB";
                        break;

                    case "GET_DISKS":
                        texto = "Unidades de disco detectadas:\n";
                        foreach (var disco in elemento.EnumerateArray())
                        {
                            string name = disco.GetProperty("unidad").GetString();
                            string formato = disco.GetProperty("formato").GetString();
                            double total = disco.GetProperty("tamano_total_gb").GetDouble();
                            double usado = disco.GetProperty("usado_gb").GetDouble();
                            double libre = disco.GetProperty("disponible_gb").GetDouble();

                            texto += $"\n{name} ({formato}) → Total: {total} GB | Usado: {usado} GB | Libre: {libre} GB";
                        }
                        break;

                    case "GET_RESOLUTION":
                        texto = $"Resolución de pantalla: {elemento.GetProperty("resolution").GetString()}";
                        break;

                    case "GET_TIME":
                        texto = $"{elemento.GetProperty("datetime").GetString()}\n" +
                                $"Zona horaria: {elemento.GetProperty("timezone").GetString()}";
                        break;

                    case "GET_PROCESSES":
                        texto = "Procesos activos (máx. 200):\n";
                        foreach (var proc in elemento.EnumerateArray())
                        {
                            string nombre = proc.GetProperty("ProcessName").GetString();
                            int id = proc.GetProperty("Id").GetInt32();
                            texto += $"- {nombre} (PID {id})\n";
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

        private void btn_desconectar_Click(object sender, EventArgs e)
        {
            if (!client.Conectado)
            {
                MessageBox.Show("No hay conexion activa", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            client.Desconectar();

            label4.Text = "Desconectado";
            label4.ForeColor = Color.Red;

            richTextBox1.Clear();
        }

        private async void btn_apagar_Click(object sender, EventArgs e)
        {
            if (!client.Conectado)
            {
                MessageBox.Show("No hay conexion activa", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var solicitud = new MensajesIO("SHUTDOWN", true, null, "", "Cliente");
            await client.Enviar(solicitud);
        }

        private async void btn_reiniciar_Click(object sender, EventArgs e)
        {
            if (!client.Conectado)
            {
                MessageBox.Show("No hay conexion activa", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var solicitud = new MensajesIO("REBOOT", true, null, "", "Cliente");
            await client.Enviar(solicitud);

        }

        private async void btn_cerrarSesion_Click(object sender, EventArgs e)
        {
            if (!client.Conectado)
            {
                MessageBox.Show("No hay conexion activa", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var solicitud = new MensajesIO("LOGOUT", true, null, "", "Cliente");
            await client.Enviar(solicitud);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!client.Conectado)
            {
                MessageBox.Show("No hay conexión activa.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            ControlRemoto = true;

            // empezar a enviar coordenadas
            timer_mouse.Interval = 40;
            timer_mouse.Start();

            // empezar a capturar clics
            this.MouseDown += Form_MouseDown;

            richTextBox1.Text = "Control remoto ACTIVADO.";
        }

        private void button2_Click(object sender, EventArgs e)
        {
            ControlRemoto = false;

            timer_mouse.Stop();
            this.MouseDown -= Form_MouseDown;

            richTextBox1.Text = "Control remoto DETENIDO.";
        }

        private async void timer_mouse_Tick(object sender, EventArgs e)
        {

            if (!ControlRemoto || !client.Conectado)
                return;

            // posición actual del mouse en el cliente
            Point pos = Cursor.Position;

            var solicitud = new MensajesIO(
                "MOVE_MOUSE",
                true,
                new { x = pos.X, y = pos.Y },
                "",
                "Cliente"
            );

            await client.Enviar(solicitud);
        }

        //Metodo para capturar los clics naturales del usuario 
        private async void Form_MouseDown(object sender, MouseEventArgs e)
        {
            if (!ControlRemoto || !client.Conectado)
                return;

            MensajesIO solicitud;

            if (e.Button == MouseButtons.Left)
            {
                solicitud = new MensajesIO("MOUSE_LEFT", true, null, "", "Cliente");
                await client.Enviar(solicitud);
            }
            else if (e.Button == MouseButtons.Right)
            {
                solicitud = new MensajesIO("MOUSE_RIGHT", true, null, "", "Cliente");
                await client.Enviar(solicitud);
            }
        }

    }
}
