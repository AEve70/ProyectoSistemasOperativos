using Compartido;
using System;
using System.IO;
using System.Net.Sockets;
using System.Text.Json;
using System.Threading.Tasks;

/* Proyecto Sistemas Operativos
 * Sebastián Alfaro Arias C4C212
 * Alejandro Córdoba Solís C4E463 
 * Evelyn Martinez Hernández C34617
 */

namespace AppClienteControlador
{
    public class Cliente
    {
        private TcpClient cliente;
        private StreamReader reader;
        private StreamWriter writer;

        public bool Conectado => cliente != null && cliente.Connected;

        public event Action ConexionCerrada; //Sirve para modificar el servidor

        //Metodo simple para conectarse al servidor
        public async Task<bool> Conectar(string ip, int puerto)
        {
            try
            {
                cliente = new TcpClient();
                await cliente.ConnectAsync(ip, puerto);

                NetworkStream stream = cliente.GetStream();
                writer = new StreamWriter(stream) { AutoFlush = true };
                reader = new StreamReader(stream);

                Console.WriteLine("Conectado al servidor");
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e);
                return false;
            }
        }

        //Metodo para enviar una solicitud al servidor en formato JSON
        public async Task Enviar(MensajesIO solicitud)
        {
            if (writer != null)
            {
                string json = JsonSerializer.Serialize(solicitud);
                await writer.WriteLineAsync(json);
            }
        }

        //Este metodo se usa cuando el comando no necesita parámetros
        public async Task<MensajesIO> EnviarSimple(string comando)
        {
            var solicitud = new MensajesIO(comando, true, null, "", "Cliente");
            await Enviar(solicitud);

            return await Recibir();
        }

        //Si la accion usa parámetros
        public async Task<MensajesIO> Enviar(string comando, object datos)
        {
            var solicitud = new MensajesIO(comando, true, datos, "", "Cliente");
            await Enviar(solicitud);

            return await Recibir();
        }

        //Recibe lo que envia el servidor en JSON
        public async Task<MensajesIO> Recibir()
        {
            try
            {
                string respuesta = await reader.ReadLineAsync();

                if (respuesta == null)
                {
                    ConexionCerrada?.Invoke();
                    return null;
                }

                if (string.IsNullOrWhiteSpace(respuesta))
                    return null;

                return JsonSerializer.Deserialize<MensajesIO>(respuesta);
            }
            catch
            {
                ConexionCerrada?.Invoke();
                return null;
            }
        }

        //Metodo para desconectarse
        public void Desconectar()
        {
            try
            {
                reader?.Close();
                writer?.Close();
                cliente?.Close();
            }
            catch
            {
            }
            finally
            {
                reader = null;
                writer = null;
                cliente = null;
            }
        }
    }
}
