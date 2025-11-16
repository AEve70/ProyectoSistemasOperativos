using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace AppServidor.Servicios
{
    public static class AccionesControl
    {
        // Usar libreria externa para controlar funciones de windows
        [DllImport("user32.dll")]
        private static extern bool SetCursorPos(int X, int Y); //Metodo que mueve el cursor del mouse del sistema 
        [DllImport("user32.dll")]
        private static extern void mouse_event(uint flags, uint dx, uint dy, uint data, int extraInfo);

        //Flags para que Windows identifique que accion se quiere hacer con respecto al mouse
        private const uint LEFTDOWN = 0x0002;
        private const uint LEFTUP = 0x0004;
        private const uint RIGHTDOWN = 0x0008;
        private const uint RIGHTUP = 0x0010;

        //Metodo para mover el cursor
        public static void Mover(int x, int y)
        {
            SetCursorPos(x, y);
        }

        //Metodos de Click
        public static void ClickIzquierdo()
        {
            //Mouse event necesita como parametro la flag para saber que hacer
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
            ClickDerecho();
        }
    }
}
