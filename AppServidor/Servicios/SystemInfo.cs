using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Runtime.InteropServices;

/* Proyecto Sistemas Operativos
 * Sebastián Alfaro Arias C4C212
 * Alejandro Córdoba Solís C4E463 
 * Evelyn Martinez Hernández C34617
 */

namespace AppServidor.Servicios
{   //Clase que sirve para obtener datos del equipo remoto 
    //En la mayoria de los casos devuelve objetos 
    public static class SystemInfo
    {
        //Como algunas difieren de la version de Windows se usara User32 para obtener algunas funciones
        [DllImport("user32.dll")]
        private static extern int GetSystemMetrics(int nIndex);

        // 1. Nombre completo del SO / 2. Plataforma / 3. Versión
        public static object GetOSInfo()
        {
            try
            {
                var searcher = new ManagementObjectSearcher("SELECT Caption, Version FROM Win32_OperatingSystem"); // sirve para consultar información interna del sistema operativo Windows 
                foreach (var os in searcher.Get())
                {
                    return new
                    {
                        os_name = os["Caption"]?.ToString(),
                        version = os["Version"]?.ToString(),
                        platform = Environment.OSVersion.Platform.ToString()
                    };
                }
            }
            catch (Exception) { }

            // fallback
            return new
            {
                os_name = Environment.OSVersion.VersionString,
                version = Environment.OSVersion.Version.ToString(),
                platform = Environment.OSVersion.Platform.ToString()
            };
        }

        // 2. Nombre del equipo
        public static object GetMachineName()
        {
            return new { machine_name = Environment.MachineName };
        }
           

        // 3. Usuario actual
        public static object GetUserInfo()
        {
            return new { user = Environment.UserName };
        }
           

        // 4. Información del procesador
        public static object GetProcessorInfo()
        {
            return new
            {
                processor = Environment.GetEnvironmentVariable("PROCESSOR_IDENTIFIER"),
                logical_processors = Environment.ProcessorCount
            };
        }

        // 5. Total RAM (GB)
        public static object GetRAM()
        {
            double ramGB = 0;
            try
            {
                var searcher = new ManagementObjectSearcher("SELECT TotalVisibleMemorySize FROM Win32_OperatingSystem");
                foreach (var obj in searcher.Get())
                {
                    double totalKB = Convert.ToDouble(obj["TotalVisibleMemorySize"]);
                    ramGB = Math.Round(totalKB / 1024 / 1024, 2);
                }
            }
            catch
            {
                ramGB = -1; // si ocurre error
            }
            return new { ram_total_gb = ramGB };
        }

        // 6. Lista de unidades de disco
        public static object GetDisks()
        {
            return DriveInfo.GetDrives()
                .Where(d => d.IsReady)
                .Select(d => new
                {
                    unidad = d.Name,
                    tamano_total_gb = Math.Round(d.TotalSize / 1e9, 2),
                    usado_gb = Math.Round((d.TotalSize - d.AvailableFreeSpace) / 1e9, 2),
                    disponible_gb = Math.Round(d.AvailableFreeSpace / 1e9, 2),
                    formato = d.DriveFormat
                }).ToList();
        }

        // 7. Resolución de pantalla usando libreria nativa de Windows
        public static object GetResolution()
        {
            try
            {
                int width = GetSystemMetrics(0);  // SM_CXSCREEN
                int height = GetSystemMetrics(1); // SM_CYSCREEN

                return new
                {
                    resolution = $"{width}x{height}"
                };
            }
            catch (Exception ex)
            {
                return new { resolution = "No disponible", error = ex.Message };
            }
        }

        // 8. Zona horaria y 9. Fecha/Hora
        public static object GetTime()
        {
            return new
            {
                datetime = DateTime.Now.ToString("F"),
                timezone = TimeZoneInfo.Local.DisplayName
            };
        }

        // 10. Lista de procesos activos
        public static object GetProcesses()
        {
            return Process.GetProcesses()
                .OrderBy(p => p.ProcessName)
                .Select(p => new { p.Id, p.ProcessName })
                .ToList();
        }
    }
}
