using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
using Compartido;

namespace AppClienteControlador
{
    public partial class Form1 : Form
    {
        private Cliente client;
        bool ControlRemoto;
        private FormScrenshot ventanaCaptura;

        public Form1()
        {
            InitializeComponent();
            client = new Cliente();
            llenarComboBox();

            label4.Text = "Desconectado";
            label4.ForeColor = Color.Coral;

            ControlRemoto = false;
        }

        // ============================================================
        // COMANDOS DISPONIBLES
        // ============================================================
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

        // ============================================================
        // CONEXIÓN
        // ============================================================
        private async void btn_conectar_Click(object sender, EventArgs e)
        {
            string ip = txt_ip.Text.Trim();
            if (!int.TryParse(txt_puerto.Text, out int puerto))
            {
                MessageBox.Show("Ingrese un número de puerto válido.");
                return;
            }

            label4.Text = "Conectando...";
            label4.ForeColor = Color.Goldenrod;
            btn_conectar.Enabled = false;

            bool ok = await client.Conectar(ip, puerto);

            label4.Text = ok ? "Conectado" : "Error de conexión";
            label4.ForeColor = ok ? Color.ForestGreen : Color.Red;

            btn_conectar.Enabled = true;
        }

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

        // ============================================================
        // CONSULTAR INFORMACIÓN DEL SISTEMA
        // ============================================================
        private async void btn_consultar_Click(object sender, EventArgs e)
        {
            if (!client.Conectado)
            {
                MessageBox.Show("No hay conexión.");
                return;
            }

            string eleccion = cbx_datos.SelectedItem.ToString();
            string comando = comandos[eleccion];

            var respuesta = await client.EnviarSimple(comando);

            if (respuesta != null)
                richTextBox1.Text = TraducirRespuesta(respuesta);
            else
                richTextBox1.Text = "No se recibió respuesta.";
        }

        // ============================================================
        // ACCIONES DE AUDIO
        // ============================================================
        private async void btn_subirVolumen_Click(object sender, EventArgs e)
        {
            var res = await client.EnviarSimple("VOL_UP");
            richTextBox1.Text = res?.Mensaje ?? "OK";
        }

        private async void btn_bajarVolumen_Click(object sender, EventArgs e)
        {
            var res = await client.EnviarSimple("VOL_DOWN");
            richTextBox1.Text = res?.Mensaje ?? "OK";
        }

        private async void btn_silenciar_Click(object sender, EventArgs e)
        {
            var res = await client.EnviarSimple("MUTE");
            richTextBox1.Text = res?.Mensaje ?? "OK";
        }

        // ============================================================
        // ACCIONES DE SISTEMA
        // ============================================================
        private async void btn_apagar_Click(object sender, EventArgs e)
        {
            await client.EnviarSimple("SHUTDOWN");
        }

        private async void btn_reiniciar_Click(object sender, EventArgs e)
        {
            await client.EnviarSimple("REBOOT");
        }

        private async void btn_cerrarSesion_Click(object sender, EventArgs e)
        {
            await client.EnviarSimple("LOGOUT");
        }

        // ============================================================
        // MENSAJES AL SERVIDOR
        // ============================================================
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
                MessageBox.Show("Escribe algo primero.");
                return;
            }

            var respuesta = await client.Enviar("SHOW_MESSAGE", texto);
            richTextBox1.Text = respuesta?.Mensaje ?? "Mensaje enviado.";
        }

        // ============================================================
        // CAPTURA MANUAL
        // ============================================================
        private async void btn_screenshot_Click(object sender, EventArgs e)
        {
            var respuesta = await client.EnviarSimple("GET_SCREENSHOT");

            if (respuesta?.Datos == null)
            {
                MessageBox.Show("No se recibió imagen.");
                return;
            }

            JsonElement elem = JsonSerializer.Deserialize<JsonElement>(JsonSerializer.Serialize(respuesta.Datos));
            string base64 = elem.GetProperty("imagen").GetString();
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

        // ============================================================
        // CONTROL REMOTO (MOUSE + STREAMING)
        // ============================================================
        private void button1_Click(object sender, EventArgs e)
        {
            if (!client.Conectado)
            {
                MessageBox.Show("No hay conexión activa.");
                return;
            }

            ControlRemoto = true;

            timer_mouse.Interval = 40;
            timer_mouse.Start();

            timer_pantalla.Interval = 200; // 5 FPS aprox
            timer_pantalla.Start();

            if (ventanaCaptura == null || ventanaCaptura.IsDisposed)
                ventanaCaptura = new FormScrenshot();

            ventanaCaptura.Show();
            ventanaCaptura.BringToFront();

            richTextBox1.Text = "Control remoto ACTIVADO.";
        }

        private void button2_Click(object sender, EventArgs e)
        {
            ControlRemoto = false;

            timer_mouse.Stop();
            timer_pantalla.Stop();

            richTextBox1.Text = "Control remoto DETENIDO.";
        }

        private async void timer_mouse_Tick(object sender, EventArgs e)
        {
            if (!ControlRemoto || !client.Conectado)
                return;

            Point pos = Cursor.Position;

            await client.Enviar("MOVE_MOUSE", new { x = pos.X, y = pos.Y });
        }

        // clics reales del usuario
        protected override void WndProc(ref Message m)
        {
            const int WM_LBUTTONDOWN = 0x0201;
            const int WM_RBUTTONDOWN = 0x0204;
            const int WM_LBUTTONDBLCLK = 0x0203;

            base.WndProc(ref m);

            if (!ControlRemoto || !client.Conectado)
                return;

            if (m.Msg == WM_LBUTTONDOWN)
                _ = client.EnviarSimple("MOUSE_LEFT");

            else if (m.Msg == WM_RBUTTONDOWN)
                _ = client.EnviarSimple("MOUSE_RIGHT");

            else if (m.Msg == WM_LBUTTONDBLCLK)
                _ = client.EnviarSimple("MOUSE_DOUBLE");
        }

        private async void timer_pantalla_Tick(object sender, EventArgs e)
        {
            if (!ControlRemoto || !client.Conectado)
                return;

            var respuesta = await client.EnviarSimple("GET_SCREENSHOT");
            if (respuesta?.Datos == null) return;

            JsonElement elem = JsonSerializer.Deserialize<JsonElement>(JsonSerializer.Serialize(respuesta.Datos));
            string base64 = elem.GetProperty("imagen").GetString();

            byte[] bytes = Convert.FromBase64String(base64);

            using (var ms = new MemoryStream(bytes))
            {
                Image img = Image.FromStream(ms);

                if (ventanaCaptura != null && !ventanaCaptura.IsDisposed)
                    ventanaCaptura.mostrarCaptura(img);
            }
        }

        // ============================================================
        // TRADUCTOR DE RESPUESTAS
        // ============================================================
        private string TraducirRespuesta(MensajesIO respuesta)
        {
            if (respuesta == null || respuesta.Datos == null)
                return "No se recibió información.";

            string comando = respuesta.Comando.ToUpper();
            string texto = "";

            try
            {
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
                        texto = $"RAM total: {elemento.GetProperty("ram_total_gb").GetDouble()} GB";
                        break;

                    case "GET_DISKS":
                        texto = "Unidades de disco:\n";
                        foreach (var disco in elemento.EnumerateArray())
                        {
                            string name = disco.GetProperty("unidad").GetString();
                            string formato = disco.GetProperty("formato").GetString();
                            double total = disco.GetProperty("tamano_total_gb").GetDouble();
                            double libre = disco.GetProperty("disponible_gb").GetDouble();

                            texto += $"\n{name} ({formato}) → Total: {total} GB | Libre: {libre} GB";
                        }
                        break;

                    case "GET_RESOLUTION":
                        texto = $"Resolución: {elemento.GetProperty("resolution").GetString()}";
                        break;

                    case "GET_TIME":
                        texto = $"{elemento.GetProperty("datetime").GetString()}\n" +
                                $"Zona horaria: {elemento.GetProperty("timezone").GetString()}";
                        break;

                    case "GET_PROCESSES":
                        texto = "Procesos activos:\n";
                        foreach (var proc in elemento.EnumerateArray())
                        {
                            texto += $"- {proc.GetProperty("ProcessName").GetString()} " +
                                     $"(PID {proc.GetProperty("Id").GetInt32()})\n";
                        }
                        break;

                    default:
                        texto = "Respuesta no formateada.";
                        break;
                }
            }
            catch (Exception ex)
            {
                texto = $"Error procesando JSON: {ex.Message}";
            }

            return texto;
        }
    }
}
