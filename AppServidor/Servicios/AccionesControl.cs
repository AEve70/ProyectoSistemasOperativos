using System.Runtime.InteropServices;

namespace AppServidor.Servicios
{
    //Clase que contiene un metodo para controlar el mouse
    //Se usa una libreria externa de windows
    //Se usan los codigos asignados cuando se quiere hacer alguna cosa ya sea mover el mouse o hacer clicks
    public static class AccionesControl
    {
        [DllImport("user32.dll")]
        private static extern bool SetCursorPos(int X, int Y);

        [DllImport("user32.dll")]
        private static extern void mouse_event(uint flags, uint dx, uint dy, uint data, int extra);

        private const uint LEFTDOWN = 0x0002;
        private const uint LEFTUP   = 0x0004;
        private const uint RIGHTDOWN = 0x0008;
        private const uint RIGHTUP   = 0x0010;

        public static void Mover(int x, int y) => SetCursorPos(x, y);

        public static void ClickIzquierdo()
        {
            mouse_event(LEFTDOWN, 0, 0, 0, 0);
            mouse_event(LEFTUP, 0, 0, 0, 0);
        }

        public static void ClickDerecho()
        {
            mouse_event(RIGHTDOWN, 0, 0, 0, 0);
            mouse_event(RIGHTUP, 0, 0, 0, 0);
        }

        public static void DobleClick()
        {
            ClickIzquierdo();
            System.Threading.Thread.Sleep(80);
            ClickIzquierdo();
        }
    }
}
