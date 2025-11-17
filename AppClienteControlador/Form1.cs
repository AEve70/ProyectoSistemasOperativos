using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
using Compartido;

/* Proyecto Sistemas Operativos
 * Sebastián Alfaro Arias C4C212
 * Alejandro Córdoba Solís C4E463 
 * Evelyn Martinez Hernández C34617
 */

//Interfaz grafica donde el cliente le hara solicitudes al servidor y se mostraran las respuestas del mismo
namespace AppClienteControlador
{
    public partial class Form1 : Form
    {
        private Cliente client;
        private bool ControlRemoto;
        private FormScrenshot ventanaCaptura;

        // Estados internos para detección de clics
        private bool prevLeft = false;
        private bool prevRight = false;
        private DateTime lastLeftClick = DateTime.MinValue;

        // Diccionario visible al usuario → comando interno real
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

            ControlRemoto = false;
            label4.Text = "Desconectado";
            label4.ForeColor = Color.Coral;

            llenarComboBox();

            client.ConexionCerrada += ServidorDesconectado; //Detectar conexion cerrada
        }

        //Llenar el combo box los nombres relacionados a los comandos mas que todo para entendimiento del usuario
        private void llenarComboBox()
        {
            cbx_datos.Items.Clear();
            cbx_datos.Items.AddRange(comandos.Keys.ToArray());
            cbx_datos.SelectedIndex = 0;
        }

        //Metodos para realizar la conexion - por defecto tendra la local, el usuario puede cambiar la Ip
        //Tambien por defecto se usara el puerto 8000 pero se puede usar otro de preferencia siempre y cuando no este ocupado
        private async void btn_conectar_Click(object sender, EventArgs e)
        {
            string ip = txt_ip.Text.Trim();

            if (!int.TryParse(txt_puerto.Text, out int puerto))
            {
                MessageBox.Show("Ingrese un puerto válido.", "Error");
                return;
            }

            btn_conectar.Enabled = false;
            label4.Text = "Conectando...";
            label4.ForeColor = Color.Goldenrod;

            bool ok = await client.Conectar(ip, puerto);

            btn_conectar.Enabled = true;

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
        }

        //Metodo para desconectarse del servidor
        private void btn_desconectar_Click(object sender, EventArgs e)
        {
            if (!client.Conectado)
            {
                MessageBox.Show("No hay conexión activa.");
                return;
            }

            client.Desconectar();

            label4.Text = "Desconectado";
            label4.ForeColor = Color.Red;
            richTextBox1.Clear();
        }

        //Metodos para obtener informacion del equipo remoto
        private async void btn_consultar_Click(object sender, EventArgs e)
        {
            if (!client.Conectado)
            {
                MessageBox.Show("No hay conexión.");
                return;
            }

            string eleccion = cbx_datos.SelectedItem?.ToString() ?? "";
            string comando = comandos[eleccion];

            var solicitud = new MensajesIO(comando, true, null, "", "Cliente");
            await client.Enviar(solicitud);

            var respuesta = await client.Recibir();
            richTextBox1.Text = respuesta != null ?
                TraducirRespuesta(respuesta) :
                "No se recibió respuesta.";
        }

        // Controles de volumen
        private async void btn_subirVolumen_Click(object sender, EventArgs e)
        {
            await EnviarSimple("VOL_UP");
        }

        private async void btn_bajarVolumen_Click(object sender, EventArgs e)
        {
            await EnviarSimple("VOL_DOWN");
        }

        private async void btn_silenciar_Click(object sender, EventArgs e)
        {
            await EnviarSimple("MUTE");
        }

        //Metodo para evitar repetir codigo al hacer solicitudes al servidor
        private async Task EnviarSimple(string cmd)
        {
            if (!client.Conectado)
            {
                MessageBox.Show("No hay conexión.");
                return;
            }

            var solicitud = new MensajesIO(cmd, true, null, "", "Cliente");
            await client.Enviar(solicitud);

            var respuesta = await client.Recibir();
            richTextBox1.Text = respuesta?.Mensaje ?? "OK";
        }

        // Metodos del sistema apagar-reiniciar-cerrar sesion
        private async void btn_apagar_Click(object sender, EventArgs e)
        {
            await EnviarSimple("SHUTDOWN");
        }

        private async void btn_reiniciar_Click(object sender, EventArgs e)
        {
            await EnviarSimple("REBOOT");
        }

        private async void btn_cerrarSesion_Click(object sender, EventArgs e)
        {
            await EnviarSimple("LOGOUT");
        }

        // ===== CONTROL REMOTO =====
        private void button1_Click(object sender, EventArgs e) // Activar
        {
            if (!client.Conectado)
            {
                MessageBox.Show("No hay conexión activa.");
                return;
            }

            ControlRemoto = true;
            timer_mouse.Interval = 40;
            timer_mouse.Start();

            richTextBox1.Text = "Control remoto ACTIVADO.";
        }

        // Detener control remoto
        private void button2_Click(object sender, EventArgs e)
        {
            ControlRemoto = false;
            timer_mouse.Stop();

            richTextBox1.Text = "Control remoto DETENIDO.";
        }

        // Movimiento y clics del mouse
        private async void timer_mouse_Tick(object sender, EventArgs e)
        {
            if (!ControlRemoto || !client.Conectado)
                return;

            // ===== Movimiento =====
            Point pos = Cursor.Position;
            await client.Enviar("MOVE_MOUSE", new { x = pos.X, y = pos.Y });

            // ===== Clic izquierdo =====
            bool leftNow = (Control.MouseButtons & MouseButtons.Left) != 0;

            if (leftNow && !prevLeft)
            {
                // doble clic
                if ((DateTime.Now - lastLeftClick).TotalMilliseconds < 250)
                    _ = client.EnviarSimple("MOUSE_DOUBLE");
                else
                    _ = client.EnviarSimple("MOUSE_LEFT");

                lastLeftClick = DateTime.Now;
            }

            prevLeft = leftNow;

            // ===== Clic derecho =====
            bool rightNow = (Control.MouseButtons & MouseButtons.Right) != 0;

            if (rightNow && !prevRight)
            {
                _ = client.EnviarSimple("MOUSE_RIGHT");
            }

            prevRight = rightNow;
        }

        // ===== Captura de pantalla =====
        private async void btn_screenshot_Click(object sender, EventArgs e)
        {
            if (!client.Conectado)
            {
                MessageBox.Show("No hay conexión.");
                return;
            }

            var solicitud = new MensajesIO("GET_SCREENSHOT", true, null, "", "Cliente");
            await client.Enviar(solicitud);

            var respuesta = await client.Recibir();
            MostrarCaptura(respuesta);
        }

        private void MostrarCaptura(MensajesIO respuesta)
        {
            try
            {
                if (respuesta?.Datos == null)
                {
                    MessageBox.Show("No se recibió imagen del servidor.");
                    return;
                }

                JsonElement root = JsonSerializer.Deserialize<JsonElement>(
                    JsonSerializer.Serialize(respuesta.Datos));

                if (!root.TryGetProperty("imagen", out JsonElement imgElem))
                {
                    MessageBox.Show("La respuesta no contiene la imagen.");
                    return;
                }

                string base64 = imgElem.GetString();
                byte[] bytes = Convert.FromBase64String(base64);

                using (var ms = new MemoryStream(bytes))
                {
                    Image img = Image.FromStream(ms);

                    if (ventanaCaptura == null || ventanaCaptura.IsDisposed)
                        ventanaCaptura = new FormScrenshot();

                    ventanaCaptura.mostrarCaptura(img);
                    ventanaCaptura.Show();
                    ventanaCaptura.BringToFront();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error mostrando la captura: {ex.Message}");
            }
        }

        // ===== Mostrar mensaje en el servidor =====
        private async void btn_message_Click(object sender, EventArgs e)
        {
            if (!client.Conectado)
            {
                MessageBox.Show("No hay conexión.");
                return;
            }

            string texto = txt_mensaje.Text.Trim();
            if (string.IsNullOrWhiteSpace(texto))
            {
                MessageBox.Show("Escribe un mensaje.");
                return;
            }

            var solicitud = new MensajesIO("SHOW_MESSAGE", true, texto, "", "Cliente");
            await client.Enviar(solicitud);

            var respuesta = await client.Recibir();
            richTextBox1.Text = respuesta?.Mensaje ?? "Mensaje enviado.";
        }

        //Cuando se hagan acciones de apagar-reiniciar-cerrar sesion el equipo remoto cierra las aplicaciones por default
        //El equipo cliente debe detectar esa desconexion y detener las acciones que estaba realizando en el equipo remoto
        private void ServidorDesconectado()
        {
            if (InvokeRequired)
            {
                Invoke((Action)ServidorDesconectado);
                return;
            }

            ControlRemoto = false;
            timer_mouse.Stop();

            label4.Text = "Desconectado";
            label4.ForeColor = Color.Red;

            richTextBox1.Text = "El servidor se desconectó (apagado/reinicio/cierre de sesión).";

            if (ventanaCaptura != null && !ventanaCaptura.IsDisposed)
                ventanaCaptura.Close();
        }
    }
}
