using System;
using System.Windows.Forms;
using Compartido;


namespace AppClienteControlador
{
    public class ClickFilter : IMessageFilter
    {
        private Cliente client;
        private Func<bool> controlActivo;

        public ClickFilter(Cliente c, Func<bool> activo)
        {
            client = c;
            controlActivo = activo;
        }

        public bool PreFilterMessage(ref Message m)
        {
            const int WM_LBUTTONDOWN = 0x0201;
            const int WM_RBUTTONDOWN = 0x0204;
            const int WM_LBUTTONDBLCLK = 0x0203;

            if (!controlActivo() || !client.Conectado)
                return false;

            switch (m.Msg)
            {
                case WM_LBUTTONDOWN:
                    _ = client.Enviar(new MensajesIO("MOUSE_LEFT", true, null, "", "Cliente"));
                    break;

                case WM_RBUTTONDOWN:
                    _ = client.Enviar(new MensajesIO("MOUSE_RIGHT", true, null, "", "Cliente"));
                    break;

                case WM_LBUTTONDBLCLK:
                    _ = client.Enviar(new MensajesIO("MOUSE_DOUBLE", true, null, "", "Cliente"));
                    break;
            }

            return false;
        }
    }
}