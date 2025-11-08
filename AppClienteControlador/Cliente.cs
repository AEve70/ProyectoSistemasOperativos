using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace AppClienteControlador
{
    public class Cliente
    {
        private TcpClient cliente;
        private StreamReader reader;
        private StreamWriter writer;


        public bool Conectado => cliente?.Connected ?? false; //Expresion remplaza if

        public async Task<bool> Conectar(String ip, int puerto)
        {
            try
            {
                cliente = new TcpClient();
                await cliente.ConnectAsync(ip, puerto);

                NetworkStream stream = cliente.GetStream();
                writer = new StreamWriter(stream) {AutoFlush = true };
                reader = new StreamReader(stream);

                Console.WriteLine("Conectado al servidor");
                return true;
            }catch(Exception e)
            {
                Console.WriteLine("Error: " + e);
                return false;
            }
        } 
    }
}
