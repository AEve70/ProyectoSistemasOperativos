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

        private FormRemote ventanaRemota; // << NUEVO FORM PARA CONTROL REMOTO

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

        private void llenarComboBox()
        {
            cbx_datos.Items.Clear();
            cbx_datos.Items.AddRange(comandos.Keys.ToArray());
            cbx_datos.SelectedIndex = 0;
        }

        // ======================
        // CONECTAR
        // ======================
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

        // ======================
        // DESCONECTAR
        // ======================
        private void btn_desconectar_Click(object sender, EventArgs e)
        {
            if (!client.Conectado)
            {
                MessageBox.Show("No hay conexión activa.");
                return;
            }

            client.Desconectar();

            ControlRemoto = false;
            ventanaRemota?.Desactivar();

            label4.Text = "Desconectado";
            label4.ForeColor = Color.Red;
            richTextBox1.Clear();
        }

        // ======================
        // CONSULTAR INFORMACIÓN
        // ======================
        private async void btn_consultar_Click(object sender, EventArgs e)
        {
            if (!client.Conectado)
            {
                MessageBox.Show("No hay conexión.");
                return;
            }

            string eleccion = cbx_datos.SelectedItem?.ToString() ?? "";
            string comando = comandos[eleccion];

            try
            {
                var solicitud = new MensajesIO(comando, true, null, "", "Cliente");
                await client.Enviar(solicitud);

                var respuesta = await client.Recibir();

                if (respuesta == null)
                {
                    richTextBox1.Text = "El servidor se desconectó.";
                    ServidorDesconectado();
                    return;
                }

                richTextBox1.Text = TraducirRespuesta(respuesta);
            }
            catch
            {
                richTextBox1.Text = "El servidor se desconectó inesperadamente.";
                ServidorDesconectado();
            }
        }

        // ======================
        // CONTROLES DE VOLUMEN
        // ======================
        private async void btn_subirVolumen_Click(object sender, EventArgs e)
        {
            try
            {
                var r = await client.EnviarSimple("VOL_UP");
                if (r == null) { ServidorDesconectado(); return; }
                richTextBox1.Text = r.Mensaje;
            }
            catch { ServidorDesconectado(); }
        }

        private async void btn_bajarVolumen_Click(object sender, EventArgs e)
        {
            try
            {
                var r = await client.EnviarSimple("VOL_DOWN");
                if (r == null) { ServidorDesconectado(); return; }
                richTextBox1.Text = r.Mensaje;
            }
            catch { ServidorDesconectado(); }
        }

        private async void btn_silenciar_Click(object sender, EventArgs e)
        {
            try
            {
                var r = await client.EnviarSimple("MUTE");
                if (r == null) { ServidorDesconectado(); return; }
                richTextBox1.Text = r.Mensaje;
            }
            catch { ServidorDesconectado(); }
        }

        // ======================
        // ACCIONES DE SISTEMA
        // ======================
        private async void btn_apagar_Click(object sender, EventArgs e)
        {
            try
            {
                var r = await client.EnviarSimple("SHUTDOWN");
                if (r == null) { ServidorDesconectado(); return; }
                richTextBox1.Text = r.Mensaje;
            }
            catch { ServidorDesconectado(); }
        }

        private async void btn_reiniciar_Click(object sender, EventArgs e)
        {
            try
            {
                var r = await client.EnviarSimple("REBOOT");
                if (r == null) { ServidorDesconectado(); return; }
                richTextBox1.Text = r.Mensaje;
            }
            catch { ServidorDesconectado(); }
        }

        private async void btn_cerrarSesion_Click(object sender, EventArgs e)
        {
            try
            {
                var r = await client.EnviarSimple("LOGOUT");
                if (r == null) { ServidorDesconectado(); return; }
                richTextBox1.Text = r.Mensaje;
            }
            catch { ServidorDesconectado(); }
        }

        // ======================
        // CONTROL REMOTO
        // ======================
        private void button1_Click(object sender, EventArgs e) // Activar
        {
            if (!client.Conectado)
            {
                MessageBox.Show("No hay conexión activa.");
                return;
            }

            ControlRemoto = true;

            if (ventanaRemota == null || ventanaRemota.IsDisposed)
                ventanaRemota = new FormRemote(client);

            ventanaRemota.Show();
            ventanaRemota.Activar();
            ventanaRemota.BringToFront();

            richTextBox1.Text = "Control remoto ACTIVADO.\nUse la ventana de control remoto para mover/clickear.";
        }

        private void button2_Click(object sender, EventArgs e) // Detener
        {
            ControlRemoto = false;

            ventanaRemota?.Desactivar();

            richTextBox1.Text = "Control remoto DETENIDO.";
        }

        // ======================
        // CAPTURA DE PANTALLA
        // ======================
        private async void btn_screenshot_Click(object sender, EventArgs e)
        {
            if (!client.Conectado)
            {
                MessageBox.Show("No hay conexión.");
                return;
            }

            try
            {
                var solicitud = new MensajesIO("GET_SCREENSHOT", true, null, "", "Cliente");
                await client.Enviar(solicitud);

                var respuesta = await client.Recibir();

                if (respuesta == null)
                {
                    MessageBox.Show("El servidor se desconectó.");
                    ServidorDesconectado();
                    return;
                }

                MostrarCaptura(respuesta);
            }
            catch
            {
                MessageBox.Show("El servidor se desconectó inesperadamente.");
                ServidorDesconectado();
            }
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

        // ======================
        // ENVIAR MENSAJE REMOTO
        // ======================
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

            try
            {
                var respuesta = await client.Enviar("SHOW_MESSAGE", texto);

                if (respuesta == null)
                {
                    richTextBox1.Text = "El servidor se desconectó.";
                    ServidorDesconectado();
                    return;
                }

                richTextBox1.Text = respuesta.Mensaje;
            }
            catch
            {
                richTextBox1.Text = "El servidor se desconectó inesperadamente.";
                ServidorDesconectado();
            }
        }

        // ======================
        // TRADUCIR RESPUESTAS
        // ======================
        private string TraducirRespuesta(MensajesIO respuesta)
        {
            if (respuesta == null || respuesta.Datos == null)
                return "Sin datos.";

            try
            {
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

        // ======================
        // DETECTAR DESCONECCIÓN
        // ======================
        private void ServidorDesconectado()
        {
            if (InvokeRequired)
            {
                Invoke((Action)ServidorDesconectado);
                return;
            }

            ControlRemoto = false;
            ventanaRemota?.Desactivar();

            label4.Text = "Desconectado";
            label4.ForeColor = Color.Red;

            richTextBox1.Text = "El servidor se desconectó (apagado/reinicio/cierre de sesión).";

            if (ventanaCaptura != null && !ventanaCaptura.IsDisposed)
                ventanaCaptura.Close();
        }

        private void toolTip1_Popup(object sender, PopupEventArgs e)
        {
            // método vacío generado por el diseñador
        }
    }
}
