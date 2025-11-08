using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.IO;

namespace AppServidor
{
    public class Handler
    {
        private TcpClient cliente;

        public Handler(TcpClient cliente)
        {
            this.cliente = cliente;
        }


        public void Run()
        {
            try
            {
                using NetworkStream stream = cliente.GetStream();
                using StreamReader reader = new StreamReader(stream);
                using StreamWriter writer = new StreamWriter(stream);

                String linea;

                while((linea = reader.ReadLine()) != null)
                {
                    Console.WriteLine("Cliente {0} : {1}", cliente.Client.RemoteEndPoint, linea);

                    writer.WriteLine("Recibido");
                }
            }catch(Exception e)
            {
                Console.WriteLine("Error " + e);
            }
            finally
            {
                cliente.Close();
                Console.WriteLine("La conexiòn ha finalizado");
            }
        }
    }
}
