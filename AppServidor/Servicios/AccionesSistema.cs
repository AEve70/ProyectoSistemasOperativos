using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppServidor.Servicios
{
    //Clase para las acciones de apagar, reiniciar, cerrar sesion, mover el mouse
    public static class AccionesSistema
    {
        public static void Apagar()
        {
            Process.Start("shutdown", "/s /t 0"); // /s = shutdown /t 0 ejecutar de inmediato
        }

        public static void Reiniciar()
        {
            Process.Start("shutdown", "/r /t 0"); // /r = reboot /t 0 ejecutar de inmediato
        }

        public static void CerrarSesion()
        {
            Process.Start("shutdown", "/l"); // /l = logout
        }



        
    }
}
