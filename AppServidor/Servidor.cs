using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace AppServidor
{
    public class Servidor
    {
        private readonly int puerto; //Seguridad para evitar modificaciones accidentales en el puerto
        private TcpListener listener;
        private bool corriendo;
        
        public Servidor(int puerto) {
            this.puerto = puerto;
        }

        //Tarea asincrona para que el servidor pueda escuchar varias llamadas
        public async Task IniciarServidor()
        {
            listener = new TcpListener(IPAddress.Any, puerto);
            listener.Start();
            corriendo = true; // El servidor esta levantado

            Console.WriteLine("Servidor disponible en el puerto {0} ", puerto);

            while (corriendo)
            {
                TcpClient cliente = await listener.AcceptTcpClientAsync();
                Console.WriteLine("Cliente conectado: {0}", cliente.Client.RemoteEndPoint);

                - = Task.Run(() => {
                    Handler handler = new Handler(cliente)
                  handler.Run();
                });
            }

        }


    }
}
