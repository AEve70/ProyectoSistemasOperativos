using Compartido;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
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

        public async Task Enviar(MensajesIO solicitud)
        {
            if (writer != null)
            {
                string json = JsonSerializer.Serialize(solicitud);
                await writer.WriteLineAsync(json);
            }
        }

        public async Task<MensajesIO> Recibir()
        {
            try
            {
                if (reader == null) return null;

                string respuesta = await reader.ReadLineAsync();
                if (string.IsNullOrWhiteSpace(respuesta)) return null;

                return JsonSerializer.Deserialize<MensajesIO>(respuesta);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error al recibir: " + e.Message);
                return null;
            }
        }

        public void Desconectar()
        {
            try
            {
                if (cliente != null)
                {
                    reader?.Close();
                    writer?.Close();
                    cliente.Close();
                }
            }
            catch(Exception e)
            {
                Console.WriteLine("Error al desconectar {0}", e);
            }
            finally
            {
                cliente = null;
                reader = null;
                writer = null;
            }
        }
    }
}
