using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Web.Script.Serialization;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms; // Para obtener la resolución de pantalla
using Microsoft.VisualBasic; // Para consultar RAM Total (ComputerInfo)
using System.Web.Extensions;



// ESTOS COMENTARIOS DESPUES LOS PODEMOS O DEBEMOS BORRAR, PERO DEJÉ COMENTARIOS SOBRE TODO EN LOS PUNTOS QUE
// SOLICITA EL DOCUMENTO, DESDE EL 1 HATSA EL 13


namespace AppServidor
{
    class Program
    {
        static void Main(string[] args)
        {
            // (1) PUNTO DE ENTRADA: define el puerto del agente remoto y arranca el servidor TCP
            int port = 5000;
            var server = new SocketServer(port);
            Console.Title = $"RemoteAgent - puerto {port}";
            server.Start();
        }
    }

   
    /// Encapsula el ciclo de vida del servidor TCP: escuchar, aceptar clientes y despachar cada conexión.

    class SocketServer
    {
        private readonly int port;
        public SocketServer(int port) { this.port = port; }

        public void Start()
        {
            // (2) INICIALIZACIÓN TCP: escucha en cualquier interfaz de red (0.0.0.0) en el puerto indicado
            var listener = new TcpListener(IPAddress.Any, port);
            listener.Start();
            Console.WriteLine($"[Server] Oyendo en {port}...");

            // (3) LOOP PRINCIPAL: acepta conexiones entrantes y las atiende en un hilo del ThreadPool
            while (true)
            {
                var client = listener.AcceptTcpClient();
                ThreadPool.QueueUserWorkItem(HandleClient, client);
            }
        }

    
        /// Atiende un cliente: handshake, lectura por líneas (protocolo), enrutamiento y respuesta JSON.
   
        private void HandleClient(object state)
        {
            using (var client = (TcpClient)state)
            using (var ns = client.GetStream())
            using (var sr = new StreamReader(ns, Encoding.UTF8))
            using (var sw = new StreamWriter(ns, new UTF8Encoding(false)) { AutoFlush = true })
            {
                // (4) SERIALIZADOR JSON: simple y suficiente para el prototipo (puedes cambiar a System.Text.Json si prefieres)
                var jss = new JavaScriptSerializer();

                // (5) HANDSHAKE: saludo inicial para que el cliente sepa que el canal está vivo
                sw.WriteLine(jss.Serialize(new { ok = true, msg = "HELLO", server = "RemoteAgent", ver = "0.1" }));

                string line;
                // (6) PROTOCOLO POR LÍNEAS: cada línea es un JSON con una operación {"op":"..."} (evita framing complejo)
                while ((line = sr.ReadLine()) != null)
                {
                    try
                    {
                        // (7) PARSEO SEGURO: convierte la línea a diccionario y extrae 'op'
                        var req = jss.Deserialize<Dictionary<string, object>>(line) ?? new Dictionary<string, object>();
                        string op = req.ContainsKey("op") ? Convert.ToString(req["op"]) : "";
                        object data;

                        // (8) ENRUTADOR DE OPERACIONES: centraliza qué hace cada comando
                        switch (op)
                        {
                            case "PING":
                                data = new { pong = true, now = DateTime.Now.ToString("o") };
                                break;

                            case "GET_SYSINFO":
                                data = SysInfoService.GetSysInfo(); 
                                break;

                            case "GET_DISKS":
                                data = SysInfoService.GetDisks();   
                                break;

                            case "GET_PROCESSES":
                                data = SysInfoService.GetProcesses(); 
                                break;

                            case "GET_TIME":
                                data = SysInfoService.GetTime();     
                                break;

                            default:
                                // (9) VALIDACIÓN: si la "op" no existe, avisamos con un error controlado
                                throw new Exception("Op no soportada: " + op);
                        }

                        // (10) RESPUESTA ESTÁNDAR: siempre regresamos { ok, op, data } para facilitar el cliente
                        sw.WriteLine(jss.Serialize(new { ok = true, op, data }));
                    }
                    catch (Exception ex)
                    {
                        sw.WriteLine(jss.Serialize(new { ok = false, error = ex.Message }));
                    }
                }
            }
        }
    }

 
    /// Módulo de extracción de datos del sistema (mínimo viable). 
    /// Extiéndelo con captura de pantalla, mensaje remoto, audio, energía, etc.
  
    static class SysInfoService
    {
        // (11) Información general del sistema y sesión
        public static object GetSysInfo()
        {
            var ci = new Microsoft.VisualBasic.Devices.ComputerInfo();
            var os = Environment.OSVersion;
            var arch = Environment.Is64BitOperatingSystem ? "x64" : "x86";
            var tz = TimeZoneInfo.Local;
            var bounds = Screen.PrimaryScreen.Bounds; 

            return new
            {
                os_name = os.VersionString,                 
                platform = os.Platform.ToString(),          
                version = os.Version.ToString(),            
                machine_name = Environment.MachineName,     
                user = Environment.UserName,                
                arch,
                logical_processors = Environment.ProcessorCount,
                processor = Environment.GetEnvironmentVariable("PROCESSOR_IDENTIFIER"),
                ram_total_gb = Math.Round(ci.TotalPhysicalMemory / 1024.0 / 1024.0 / 1024.0, 2),
                resolution = $"{bounds.Width}x{bounds.Height}",
                timezone = tz.DisplayName,
                datetime = DateTime.Now.ToString("o")
            };
        }

        // (12) Inventario de discos listos (montados y accesibles)
        public static object GetDisks()
        {
            var list = new List<object>();
            foreach (var d in DriveInfo.GetDrives())
            {
                try
                {
                    if (d.IsReady)
                    {
                        list.Add(new
                        {
                            name = d.Name,
                            label = d.VolumeLabel,
                            format = d.DriveFormat,
                            type = d.DriveType.ToString(),
                            size_gb = Math.Round(d.TotalSize / 1024.0 / 1024.0 / 1024.0, 2),
                            free_gb = Math.Round(d.AvailableFreeSpace / 1024.0 / 1024.0 / 1024.0, 2),
                            used_gb = Math.Round((d.TotalSize - d.AvailableFreeSpace) / 1024.0 / 1024.0 / 1024.0, 2)
                        });
                    }
                }
                catch
                {
                }
            }
            return list;
        }

        public static object GetProcesses()
        {
            return Process.GetProcesses()
                          .OrderBy(p => p.ProcessName)
                          .Select(p => new { id = p.Id, name = p.ProcessName })
                          .Take(200)
                          .ToList();
        }



        public static object GetTime()
        {
            return new { timezone = TimeZoneInfo.Local.DisplayName, datetime = DateTime.Now.ToString("o") };
        }
    }
}
