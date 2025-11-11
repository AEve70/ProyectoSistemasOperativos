using System;

public class SystemInfo
{
	public SystemInfo()
	{
        using System;
        using System.Collections.Generic;
        using System.Diagnostics;
        using System.IO;
        using System.Linq;
        using System.Windows.Forms;

namespace AppServidor.Servicios
{
    public static class SysInfoService
    {
        public static object GetSysInfo()
        {
            var ci = new Microsoft.VisualBasic.Devices.ComputerInfo();
            var os = Environment.OSVersion;
            var bounds = Screen.PrimaryScreen.Bounds;

            return new
            {
                os_name = os.VersionString,
                platform = os.Platform.ToString(),
                version = os.Version.ToString(),
                machine_name = Environment.MachineName,
                user = Environment.UserName,
                processor = Environment.GetEnvironmentVariable("PROCESSOR_IDENTIFIER"),
                logical_processors = Environment.ProcessorCount,
                ram_total_gb = Math.Round(ci.TotalPhysicalMemory / 1024.0 / 1024.0 / 1024.0, 2),
                resolution = $"{bounds.Width}x{bounds.Height}",
                timezone = TimeZoneInfo.Local.DisplayName,
                datetime = DateTime.Now.ToString("G")
            };
        }

        public static object GetDisks()
        {
            return DriveInfo.GetDrives()
                .Where(d => d.IsReady)
                .Select(d => new
                {
                    d.Name,
                    d.DriveFormat,
                    d.DriveType,
                    total_gb = Math.Round(d.TotalSize / 1e9, 2),
                    free_gb = Math.Round(d.AvailableFreeSpace / 1e9, 2),
                    used_gb = Math.Round((d.TotalSize - d.AvailableFreeSpace) / 1e9, 2)
                }).ToList();
        }

        public static object GetProcesses()
        {
            return Process.GetProcesses()
                .OrderBy(p => p.ProcessName)
                .Select(p => new { p.Id, p.ProcessName })
                .Take(200)
                .ToList();
        }

        public static object GetTime()
        {
            return new { datetime = DateTime.Now.ToString("F"), timezone = TimeZoneInfo.Local.DisplayName };
        }

    }
}
