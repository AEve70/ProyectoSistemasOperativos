using System;
using System.Threading.Tasks;

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
