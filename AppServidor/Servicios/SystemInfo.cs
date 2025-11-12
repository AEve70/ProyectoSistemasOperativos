using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;

namespace AppServidor.Servicios
{
    public static class SystemInfo
    {
        // 1. Nombre completo del SO / 2. Plataforma / 3. Versión
        public static object GetOSInfo()
        {
            var os = Environment.OSVersion;
            return new
            {
                os_name = os.VersionString,
                platform = os.Platform.ToString(),
                version = os.Version.ToString()
            };
        }

        // 4. Nombre del equipo
        public static object GetMachineName() =>
            new { machine_name = Environment.MachineName };

        // 9. Usuario actual
        public static object GetUserInfo() =>
            new { user = Environment.UserName };

        // 5. Información del procesador
        public static object GetProcessorInfo() =>
            new
            {
                processor = Environment.GetEnvironmentVariable("PROCESSOR_IDENTIFIER"),
                logical_processors = Environment.ProcessorCount
            };

        // 6. Total RAM (GB)
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

        // 7. Lista de unidades de disco
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

        // 8. Resolución de pantalla (sin usar Forms)
        public static object GetResolution()
        {
            try
            {
                var searcher = new ManagementObjectSearcher("SELECT ScreenWidth, ScreenHeight FROM Win32_DesktopMonitor");
                foreach (var obj in searcher.Get())
                {
                    if (obj["ScreenWidth"] != null && obj["ScreenHeight"] != null)
                    {
                        return new
                        {
                            resolution = $"{obj["ScreenWidth"]}x{obj["ScreenHeight"]}"
                        };
                    }
                }
            }
            catch
            {
                return new { resolution = "No disponible" };
            }
            return new { resolution = "No disponible" };
        }

        // 10. Zona horaria y 11. Fecha/Hora
        public static object GetTime()
        {
            return new
            {
                datetime = DateTime.Now.ToString("F"),
                timezone = TimeZoneInfo.Local.DisplayName
            };
        }

        // 12. Lista de procesos activos
        public static object GetProcesses()
        {
            return Process.GetProcesses()
                .OrderBy(p => p.ProcessName)
                .Select(p => new { p.Id, p.ProcessName })
                .ToList();
        }
    }
}
