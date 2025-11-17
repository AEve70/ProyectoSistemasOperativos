using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using Compartido;
using AppServidor.Servicios;

/* Proyecto Sistemas Operativos
 * Sebastián Alfaro Arias C4C212
 * Alejandro Córdoba Solís C4E463 
 * Evelyn Martinez Hernández C34617
 */

namespace AppServidor
{
    //El Handler es un manejador de las acciones del servidor
    //Lee las solicitudes del usuario y las atiende
    public class Handler
    {
        private readonly TcpClient cliente;

        public Handler(TcpClient cliente)
        {
            this.cliente = cliente;
        }

        //Como se usan hilos se hizo un metodo Run para la conexion
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

                    // ===========================
                    // VALIDACIÓN DE JSON SEGURO
                    // ===========================
                    try
                    {
                        solicitud = JsonSerializer.Deserialize<MensajesIO>(linea);
                    }
                    catch
                    {
                        EnviarError(writer, "JSON inválido");
                        continue;
                    }

                    // ===========================
                    // VALIDAR COMANDO VACÍO
                    // ===========================
                    if (solicitud == null || string.IsNullOrWhiteSpace(solicitud.Comando))
                    {
                        EnviarError(writer, "Comando vacío");
                        continue;
                    }

                    object datosRespuesta = null;

                    try
                    {
                        datosRespuesta = ProcesarComando(solicitud.Comando, solicitud.Datos);
                    }
                    catch (Exception ex)
                    {
                        //Error controlado del servidor
                        EnviarError(writer, ex.Message, solicitud.Comando);
                        continue;
                    }

                    // =====================================================================
                    // ⚠ SISTEMA ANTI-SATURACIÓN ⚠
                    // Estos comandos NO deben responder nada
                    // =====================================================================
                    if (solicitud.Comando.StartsWith("MOVE_") ||
                        solicitud.Comando.StartsWith("MOUSE_"))
                    {
                        // NO respondemos nada → evita saturación y desconexión
                        continue;
                    }

                    // ===========================
                    // RESPUESTA NORMAL
                    // ===========================
                    var respuesta = new MensajesIO(
                        solicitud.Comando,
                        true,
                        datosRespuesta,
                        "OK",
                        "Servidor"
                    );

                    writer.WriteLine(JsonSerializer.Serialize(respuesta));
                }
            }
            catch (IOException)
            {
                // Cliente cerró socket abruptamente → no es error crítico  
                Console.WriteLine("Cliente desconectado abruptamente.");
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

        //Enviar error estandarizado
        private void EnviarError(StreamWriter writer, string mensaje, string comando = "ERROR")
        {
            var err = new MensajesIO(comando, false, null, mensaje, "Servidor");
            writer.WriteLine(JsonSerializer.Serialize(err));
        }

        //Este metodo realiza las solicitudes del usuario
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

                // CONTROL REMOTO (SIN RESPUESTA)
                case "MOVE_MOUSE":
                    JsonElement e = (JsonElement)datos;
                    AccionesControl.Mover(
                        e.GetProperty("x").GetInt32(),
                        e.GetProperty("y").GetInt32()
                    );
                    return null;

                case "MOUSE_LEFT": AccionesControl.ClickIzquierdo(); return null;
                case "MOUSE_RIGHT": AccionesControl.ClickDerecho(); return null;
                case "MOUSE_DOUBLE": AccionesControl.DobleClick(); return null;

                // MENSAJE REMOTO
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
