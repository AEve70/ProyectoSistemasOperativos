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
            if (!controlActivo() || !client.Conectado)
                return false;

            const int WM_LBUTTONDOWN = 0x0201;
            const int WM_RBUTTONDOWN = 0x0204;
            const int WM_LBUTTONDBLCLK = 0x0203;

            // 🔥 Solo procesar si el click fue DIRECTAMENTE sobre el FORM
            //   => No sobre botones, paneles, textbox, etc.
            Control ctrl = Form.ActiveForm?.GetChildAtPoint(Form.ActiveForm.PointToClient(Cursor.Position));

            // Si ctrl NO es null → significa que clicaste un control (botón, textbox)
            // por lo tanto NO mandamos clic remoto
            if (ctrl != null)
                return false;

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

            return false;
        }
    }
}