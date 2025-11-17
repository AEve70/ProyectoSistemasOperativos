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
        private HookMouse hook;


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
            //Obtenemos la ip y el puerto (se hace un cast a Int)
            string ip = txt_ip.Text.Trim();

            if (!int.TryParse(txt_puerto.Text, out int puerto))
            {
                MessageBox.Show("Ingrese un puerto válido.", "Error");
                return;
            }

            btn_conectar.Enabled = false;
            label4.Text = "Conectando...";
            label4.ForeColor = Color.Goldenrod; //Para diferenciar la accion 

            bool ok = await client.Conectar(ip, puerto); //Espera la respuesta del servidor

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

        //Metodos de control remoto
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

        //Metodo para detener el control remoto a traves del mouse
        private void button2_Click(object sender, EventArgs e) // Detener
        {

            ControlRemoto = false;
            timer_mouse.Stop();

            richTextBox1.Text = "Control remoto DETENIDO.";
        }

        // Movimiento del mouse
        private async void timer_mouse_Tick(object sender, EventArgs e)
        {
            if (!ControlRemoto || !client.Conectado) return;

            Point pos = Cursor.Position;

            await client.Enviar("MOVE_MOUSE", new { x = pos.X, y = pos.Y });
        }

        // Deteccion de los clicls
        protected override void WndProc(ref Message m)
        {
            const int WM_LBUTTONDOWN = 0x0201;
            const int WM_RBUTTONDOWN = 0x0204;
            const int WM_LBUTTONDBLCLK = 0x0203;

            base.WndProc(ref m);

            if (!ControlRemoto || !client.Conectado)
                return;

            switch (m.Msg)
            {
                case WM_LBUTTONDOWN:
                    _ = client.EnviarSimple("MOUSE_LEFT");
                    break;

                case WM_RBUTTONDOWN:
                    _ = client.EnviarSimple("MOUSE_RIGHT");
                    break;

                case WM_LBUTTONDBLCLK:
                    _ = client.EnviarSimple("MOUSE_DOUBLE");
                    break;
            }
        }


        // Captura de pantalla

        private async void btn_screenshot_Click(object sender, EventArgs e)
        {
            if (!client.Conectado)
            {
                MessageBox.Show("No hay conexión.");
                return;
            }

            // pedir la captura al servidor
            var solicitud = new MensajesIO("GET_SCREENSHOT", true, null, "", "Cliente");
            await client.Enviar(solicitud);

            var respuesta = await client.Recibir();

            // usar el helper para mostrarla
            MostrarCaptura(respuesta);
        }

        //Metodo que recibe los bytes que forman la imagen y abre un form con un picture box para mostrar la captura
        private void MostrarCaptura(MensajesIO respuesta)
        {
            try
            {
                if (respuesta?.Datos == null)
                {
                    MessageBox.Show("No se recibió imagen del servidor.");
                    return;
                }

                // Datos → JsonElement
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

                    // si la ventana no existe o está cerrada, la creamos
                    if (ventanaCaptura == null || ventanaCaptura.IsDisposed)
                        ventanaCaptura = new FormScrenshot();

                    ventanaCaptura.mostrarCaptura(img); // aquí adentro usás el PictureBox
                    ventanaCaptura.Show();
                    ventanaCaptura.BringToFront();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error mostrando la captura: {ex.Message}");
            }
        }

        // metodo basico para mostrar un mensaje

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

        // Como las solicitudes y respuestas estan en formato JSON, se necesita convertir esa estructura en un String común para ser legible al usuario

        private string TraducirRespuesta(MensajesIO respuesta)
        {
            if (respuesta == null || respuesta.Datos == null)
                return "Sin datos.";

            try
            {
                // Usar JSON para convertir un Json en el objeto original enviado por el servidor
                JsonElement elem = JsonSerializer.Deserialize<JsonElement>(
                    JsonSerializer.Serialize(respuesta.Datos)
                );

                string cmd = respuesta.Comando.ToUpper();

                string texto;

                switch (cmd)
                {
                    case "GET_OS_INFO":
                        texto =
                            $"{elem.GetProperty("os_name")}\n" +
                            $"{elem.GetProperty("platform")}\n" +
                            $"Versión: {elem.GetProperty("version")}";
                        break;

                    case "GET_MACHINE_NAME":
                        texto = $"Nombre del equipo: {elem.GetProperty("machine_name")}";
                        break;

                    case "GET_USER":
                        texto = $"Usuario: {elem.GetProperty("user")}";
                        break;

                    case "GET_PROCESSOR":
                        texto =
                            $"{elem.GetProperty("processor")}\n" +
                            $"Lógicos: {elem.GetProperty("logical_processors")}";
                        break;

                    case "GET_RAM":
                        texto = $"RAM total: {elem.GetProperty("ram_total_gb")} GB";
                        break;

                    case "GET_RESOLUTION":
                        texto = $"Resolución: {elem.GetProperty("resolution")}";
                        break;

                    case "GET_TIME":
                        texto =
                            $"{elem.GetProperty("datetime")}\n" +
                            $"{elem.GetProperty("timezone")}";
                        break;

                    case "GET_DISKS":
                        texto = TraducirDiscos(elem);
                        break;

                    case "GET_PROCESSES":
                        texto = TraducirProcesos(elem);
                        break;

                    default:
                        texto = "Comando no implementado.";
                        break;
                }

                return texto;
            }
            catch (Exception ex)
            {
                return $"Error al traducir: {ex.Message}";
            }
        }


        private static string TraducirDiscos(JsonElement elem)
        {
            string t = "Discos detectados:\n";
            foreach (var d in elem.EnumerateArray())
            {
                t += $"- {d.GetProperty("unidad")} ({d.GetProperty("formato")})\n" +
                     $"  Total: {d.GetProperty("tamano_total_gb")} | " +
                     $"Usado: {d.GetProperty("usado_gb")} | " +
                     $"Libre: {d.GetProperty("disponible_gb")}\n";
            }
            return t;
        }

        private static string TraducirProcesos(JsonElement elem)
        {
            string t = "Procesos (máx. 200):\n";
            foreach (var p in elem.EnumerateArray())
            {
                t += $"- {p.GetProperty("ProcessName")} (PID {p.GetProperty("Id")})\n";
            }
            return t;
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

        private void toolTip1_Popup(object sender, PopupEventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }
    }
}
