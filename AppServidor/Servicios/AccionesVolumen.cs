using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AppServidor.Servicios
{   // Clase que maneja las acciones de volumen
    public static class AccionesVolumen
    {
        //Se usa la libreria user32 que permite ejecutar acciones de Windows
        [DllImport("user32.dll", SetLastError = true)]

        //Modificador extern para poder usar librerias fuera de C#
        static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, UIntPtr dwExtraInfo); //Metodo para simular que se presiono alguna tecla (vol up-down-silence)
        const uint KETEVENTF_EXTENDEDKEY = 0x1;
        const uint KEYEVENTF_KEYUP = 0x2;

        //Metodos estaticos para subir, bajar volumen y silenciar usando el metodo keyevent
        public static void SubirVolumen()
        {
            keybd_event((byte)Keys.VolumeUp, 0, KETEVENTF_EXTENDEDKEY, UIntPtr.Zero);
        }

        public static void BajarVolumen()
        {
            keybd_event((byte)Keys.VolumeDown, 0, KETEVENTF_EXTENDEDKEY, UIntPtr.Zero);
        }

        public static void Silenciar()
        {
            keybd_event((byte)Keys.VolumeMute, 0, KETEVENTF_EXTENDEDKEY, UIntPtr.Zero);
        }



        

    }
}
