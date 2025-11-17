using System;
using System.Threading.Tasks;

/* Proyecto Sistemas Operativos
 * Sebastián Alfaro Arias C4C212
 * Alejandro Córdoba Solís C4E463 
 * Evelyn Martinez Hernández C34617
 */
namespace AppServidor
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Console.Title = "Servidor TCP";
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("=== Servidor remoto iniciado ===");

            int puerto = 8000;
            Servidor servidor = new Servidor(puerto);

            try
            {
                await servidor.IniciarServidor();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Error al iniciar el servidor: " + ex.Message);
            }

            Console.ResetColor();
            Console.WriteLine("Presiona cualquier tecla para salir...");
            Console.ReadKey();
        }
    }
}
