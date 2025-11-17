using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

/* Proyecto Sistemas Operativos
 * Sebastián Alfaro Arias C4C212
 * Alejandro Córdoba Solís C4E463 
 * Evelyn Martinez Hernández C34617
 */

namespace AppClienteControlador
{
    //Ayuda a imitar el control del mouse en la computadora servidor como si fuera el cliente
    public class HookMouse
    {
        //Importar librerias de user32
        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelMouseProc lpfn,
            IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll")]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll")]
        private static extern IntPtr GetModuleHandle(string lpModuleName);
        // Delegado para enviar los comandos al servidor
        private readonly Action<string> enviarComando;

        public HookMouse(Action<string> enviarComandoCallback)
        {
            enviarComando = enviarComandoCallback;
        }

        //Codigos de los clicks segun System32
        private const int WH_MOUSE_LL = 14;
        private const int WM_LBUTTONDOWN = 0x0201;
        private const int WM_RBUTTONDOWN = 0x0204;
        private const int WM_LBUTTONDBLCLK = 0x0203;

        private static IntPtr hookId = IntPtr.Zero;

        private delegate IntPtr LowLevelMouseProc(int nCode, IntPtr wParam, IntPtr lParam);
        private static LowLevelMouseProc proc;

        // 

        public void Instalar()
        {
            proc = HookCallback;
            hookId = SetHook(proc);
        }

        public void Desinstalar()
        {
            if(hookId != IntPtr.Zero) UnhookWindowsHookEx(hookId);
        }

        private IntPtr SetHook(LowLevelMouseProc proc)
        {
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule)
            {
                return SetWindowsHookEx(
                    WH_MOUSE_LL, proc,
                    GetModuleHandle(curModule.ModuleName),
                    0
                );
            }
        }

        // Sirve para detectar los clicks del cliente sin importar si esta afuera o dentro del forms
        private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0) // Evento válido
            {
                int msg = wParam.ToInt32();

                switch (msg)
                {
                    case WM_LBUTTONDOWN:
                        enviarComando("MOUSE_LEFT");
                        break;

                    case WM_RBUTTONDOWN:
                        enviarComando("MOUSE_RIGHT");
                        break;

                    case WM_LBUTTONDBLCLK:
                        enviarComando("MOUSE_DOUBLE");
                        break;
                }
            }

            return CallNextHookEx(hookId, nCode, wParam, lParam);
        }

      
    }
}
