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
                        writer.WriteLine(JsonSerializer.Serialize(
                            new MensajesIO("ERROR", false, null, "JSON inválido", "Servidor")
                        ));
                        continue;
                    }

                    if (solicitud == null || string.IsNullOrWhiteSpace(solicitud.Comando))
                    {
                        writer.WriteLine(JsonSerializer.Serialize(
                            new MensajesIO("ERROR", false, null, "Comando vacío", "Servidor")
                        ));
                        continue;
                    }

                    MensajesIO respuesta;

                    try
                    {
                        var datos = ProcesarComando(solicitud.Comando, solicitud.Datos);
                        respuesta = new MensajesIO(solicitud.Comando, true, datos, "OK", "Servidor");
                    }
                    catch (Exception ex)
                    {
                        respuesta = new MensajesIO(solicitud.Comando, false, null, ex.Message, "Servidor");
                    }

                    writer.WriteLine(JsonSerializer.Serialize(respuesta));
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error Handler: " + e.Message);
            }
            finally
            {
                cliente.Close();
            }
        }

        private object ProcesarComando(string comando, object datos)
        {
            switch (comando.ToUpper())
            {
                // INFORMACIÓN DEL SISTEMA
                case "GET_OS_INFO": return SystemInfo.GetOSInfo();
                case "GET_MACHINE_NAME": return SystemInfo.GetMachineName();
                case "GET_USER": return SystemInfo.GetUserInfo();
                case "GET_PROCESSOR": return SystemInfo.GetProcessorInfo();
                case "GET_RAM": return SystemInfo.GetRAM();
                case "GET_DISKS": return SystemInfo.GetDisks();
                case "GET_RESOLUTION": return SystemInfo.GetResolution();
                case "GET_TIME": return SystemInfo.GetTime();
                case "GET_PROCESSES": return SystemInfo.GetProcesses();

                // FUNCIONES DE VOLUMEN
                case "VOL_UP": AccionesVolumen.SubirVolumen(); return "OK";
                case "VOL_DOWN": AccionesVolumen.BajarVolumen(); return "OK";
                case "MUTE": AccionesVolumen.Silenciar(); return "OK";

                // FUNCIONES DE SISTEMA
                case "SHUTDOWN": AccionesSistema.Apagar(); return "OK";
                case "REBOOT": AccionesSistema.Reiniciar(); return "OK";
                case "LOGOUT": AccionesSistema.CerrarSesion(); return "OK";

                // MOUSE
                case "MOVE_MOUSE":
                    JsonElement e = (JsonElement)datos;
                    AccionesControl.Mover(e.GetProperty("x").GetInt32(), e.GetProperty("y").GetInt32());
                    return "OK";

                case "MOUSE_LEFT": AccionesControl.ClickIzquierdo(); return "OK";
                case "MOUSE_RIGHT": AccionesControl.ClickDerecho(); return "OK";
                case "MOUSE_DOUBLE": AccionesControl.DobleClick(); return "OK";

                // MENSAJE
                case "SHOW_MESSAGE":
                    string msg = datos?.ToString() ?? "";
                    AccionesRemotas.MostrarMensaje(msg);
                    return "OK";

                // SCREENSHOT 
                case "GET_SCREENSHOT":
                    return new { imagen = AccionesRemotas.TomarScreenshot() };

                default:
                    throw new Exception("Comando no reconocido");
            }
        }
    }
}
