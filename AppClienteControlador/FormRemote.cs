using System;
using System.Drawing;
using System.Windows.Forms;
using Compartido;

/* Proyecto Sistemas Operativos
 * Sebastián Alfaro Arias C4C212
 * Alejandro Córdoba Solís C4E463 
 * Evelyn Martinez Hernández C34617
 */

//Ventana especial usada para el control remoto del mouse.
//Todo clic y movimiento dentro del panelRemote se enviará al servidor.

namespace AppClienteControlador
{
    public partial class FormRemote : Form
    {
        private readonly Cliente client;
        private bool activo;
        private Point ultimoEnvio = Point.Empty; // Para evitar saturación

        public FormRemote(Cliente cliente)
        {
            InitializeComponent();
            this.client = cliente;
            activo = false;
        }

        private void FormRemote_Load(object sender, EventArgs e)
        {
            // Eventos del panel donde se controla el mouse remoto
            panelRemote.MouseMove += PanelRemote_MouseMove;
            panelRemote.MouseDown += PanelRemote_MouseDown;
            panelRemote.MouseDoubleClick += PanelRemote_MouseDoubleClick;
        }

        //Activar control remoto
        public void Activar()
        {
            activo = true;
        }

        //Desactivar control remoto
        public void Desactivar()
        {
            activo = false;
        }

        // Movimiento del mouse
        private async void PanelRemote_MouseMove(object sender, MouseEventArgs e)
        {
            if (!activo || !client.Conectado)
                return;

            // Convertir posición del panel → pantalla
            Point screenPos = panelRemote.PointToScreen(e.Location);

            // Evitar enviar posiciones casi idénticas
            if (Math.Abs(screenPos.X - ultimoEnvio.X) < 2 &&
                Math.Abs(screenPos.Y - ultimoEnvio.Y) < 2)
                return;

            ultimoEnvio = screenPos;

            await client.Enviar(new MensajesIO(
                "MOVE_MOUSE",
                true,
                new { x = screenPos.X, y = screenPos.Y },
                "",
                "Cliente"));
        }

        // Clic simple
        private async void PanelRemote_MouseDown(object sender, MouseEventArgs e)
        {
            if (!activo || !client.Conectado)
                return;

            string cmd = e.Button switch
            {
                MouseButtons.Left => "MOUSE_LEFT",
                MouseButtons.Right => "MOUSE_RIGHT",
                _ => null
            };

            if (cmd != null)
            {
                _ = client.Enviar(new MensajesIO(cmd, true, null, "", "Cliente"));
            }
        }

        // Doble clic
        private async void PanelRemote_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (!activo || !client.Conectado)
                return;

            _ = client.Enviar(new MensajesIO("MOUSE_DOUBLE", true, null, "", "Cliente"));
        }
    }
}
