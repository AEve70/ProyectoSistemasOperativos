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

        //Metodo que ejecuta el servidor por medio de un Thread
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
                            new MensajesIO("ERROR", false, null, "Solicitud sin comando", "Servidor")
                        ));
                        continue;
                    }

                    MensajesIO respuesta;

                    try
                    {
                        object datosRespuesta = ProcesarComando(solicitud.Comando, solicitud.Datos);
                        respuesta = new MensajesIO(solicitud.Comando, true, datosRespuesta, "OK", "Servidor");
                    }
                    catch (Exception ex)
                    {
                        respuesta = new MensajesIO(solicitud.Comando, false, null, ex.Message, "Servidor");
                    }

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

        //Procesa los comandos segun lo que pida el usuario

        private object ProcesarComando(string comando, object datos)
        {
            switch (comando.ToUpper())
            {
                // Acciones de extraccion de datos
                case "GET_OS_INFO": return SystemInfo.GetOSInfo();
                case "GET_MACHINE_NAME": return SystemInfo.GetMachineName(); //No se usan breaks por la existencia del return
                case "GET_USER": return SystemInfo.GetUserInfo();
                case "GET_PROCESSOR": return SystemInfo.GetProcessorInfo();
                case "GET_RAM": return SystemInfo.GetRAM();
                case "GET_DISKS": return SystemInfo.GetDisks();
                case "GET_RESOLUTION": return SystemInfo.GetResolution();
                case "GET_TIME": return SystemInfo.GetTime();
                case "GET_PROCESSES": return SystemInfo.GetProcesses();

                // Acciones para controlar audio
                case "VOL_UP": AccionesVolumen.SubirVolumen(); return "Volumen subido";
                case "VOL_DOWN": AccionesVolumen.BajarVolumen(); return "Volumen bajado";
                case "MUTE": AccionesVolumen.Silenciar(); return "Silenciado";

                // Acciones del sistema
                case "SHUTDOWN": AccionesSistema.Apagar(); return "Apagando...";
                case "REBOOT": AccionesSistema.Reiniciar(); return "Reiniciando...";
                case "LOGOUT": AccionesSistema.CerrarSesion(); return "Cerrando sesión...";

                // Acciones del mouse
                case "MOVE_MOUSE":
                    {
                        if (datos == null)
                            throw new Exception("Coordenadas no enviadas");

                        JsonElement elem = (JsonElement)datos;
                        int x = elem.GetProperty("x").GetInt32();
                        int y = elem.GetProperty("y").GetInt32();

                        AccionesControl.Mover(x, y);
                        return null;
                    }

                case "MOUSE_LEFT":
                    AccionesControl.ClickIzquierdo();
                    return null;

                case "MOUSE_RIGHT":
                    AccionesControl.ClickDerecho();
                    return null;

                case "MOUSE_DOUBLE":
                    AccionesControl.DobleClick();
                    return null;

                // Accion de tomar screenshot
                case "GET_SCREENSHOT":
                    return new { imagen = AccionesRemotas.TomarScreenshot() };

                // Acciones de mandar mensajes
                case "SHOW_MESSAGE":
                    string texto = datos?.ToString() ?? "";
                    return new { resultado = AccionesRemotas.MostrarMensaje(texto) };

                default:
                    throw new InvalidOperationException($"Comando no reconocido: {comando}");
            }
        }
    }
}
