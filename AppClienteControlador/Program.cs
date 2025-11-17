using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

/* Proyecto Sistemas Operativos
 * Sebastián Alfaro Arias C4C212
 * Alejandro Córdoba Solís C4E463 
 * Evelyn Martinez Hernández C34617
 */

namespace AppClienteControlador
{
    static class Program
    {
        /// <summary>
        /// Punto de entrada principal para la aplicación.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}
