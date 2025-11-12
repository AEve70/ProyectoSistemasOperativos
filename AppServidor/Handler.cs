using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using Compartido;
using AppServidor.Servicios;

namespace AppServidor
{
    public class Handler
    {
        private readonly TcpClient cliente;

        public Handler(TcpClient cliente)
        {
            this.cliente = cliente;
        }

        public void Run()
        {
            try
            {
                using NetworkStream stream = cliente.GetStream();
                using StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                using StreamWriter writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true };

                string linea;

                while ((linea = reader.ReadLine()) != null)
                {
                    Console.WriteLine($"Cliente {cliente.Client.RemoteEndPoint}: {linea}");

                    MensajesIO solicitud;

                    try
                    {
                        solicitud = JsonSerializer.Deserialize<MensajesIO>(linea);
                    }
                    catch
                    {
                        var error = new MensajesIO("ERROR", false, null, "JSON inválido o malformado", "Servidor");
                        writer.WriteLine(JsonSerializer.Serialize(error));
                        continue;
                    }

                    // validar el comando
                    if (solicitud == null || string.IsNullOrWhiteSpace(solicitud.Comando))
                    {
                        var error = new MensajesIO("ERROR", false, null, "Solicitud vacía o sin comando", "Servidor");
                        writer.WriteLine(JsonSerializer.Serialize(error));
                        continue;
                    }

                    // procesar comando
                    MensajesIO respuesta;
                    try
                    {
                        object datos = ProcesarComando(solicitud.Comando);
                        respuesta = new MensajesIO(solicitud.Comando, true, datos, "OK", "Servidor");
                    }
                    catch (Exception ex)
                    {
                        respuesta = new MensajesIO(solicitud.Comando, false, null, ex.Message, "Servidor");
                    }

                    // enviar respuesta
                    string json = JsonSerializer.Serialize(respuesta);
                    writer.WriteLine(json);
                    Console.WriteLine($"→ Enviado a {cliente.Client.RemoteEndPoint}: {json}");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error en Handler: " + e.Message);
            }
            finally
            {
                cliente.Close();
                Console.WriteLine("La conexión ha finalizado");
            }
        }

        // este método ejecuta los comandos recibidos
        private object ProcesarComando(string comando)
        {
            switch (comando.ToUpper())
            {
                case "GET_OS_INFO":
                    return SystemInfo.GetOSInfo();

                case "GET_MACHINE_NAME":
                    return SystemInfo.GetMachineName();

                case "GET_USER":
                    return SystemInfo.GetUserInfo();

                case "GET_PROCESSOR":
                    return SystemInfo.GetProcessorInfo();

                case "GET_RAM":
                    return SystemInfo.GetRAM();

                case "GET_DISKS":
                    return SystemInfo.GetDisks();

                case "GET_RESOLUTION":
                    return SystemInfo.GetResolution();

                case "GET_TIME":
                    return SystemInfo.GetTime();

                case "GET_PROCESSES":
                    return SystemInfo.GetProcesses();

                case "PING":
                    return new { Pong = true, Time = DateTime.Now.ToString("T") };

                default:
                    throw new InvalidOperationException($"Comando no reconocido: {comando}");
            }
        }
    }
}
