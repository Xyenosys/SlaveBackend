using System.Diagnostics;
using System.Runtime.InteropServices;


namespace SlaveBackend.Services
{
    public class OSHelper
    {
        #region Machineinfo
        public bool IsWindows()
        {
            return RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
        }
        public bool IsLinux()
        {
            return RuntimeInformation.IsOSPlatform(OSPlatform.Linux);
        }
        public void GetHostInfo()
        {
            Console.WriteLine("Host Information:");
            Console.WriteLine("------------------");

            // OS Information
            Console.WriteLine($"Operating System: {GetOSDescription()}");
            Console.WriteLine($"Architecture: {GetArchitecture()}");

            // CPU Information
            Console.WriteLine($"Processor Count: {GetProcCount()}");

            // RAM Information
            var totalMemory = GetTotalMemory();
            Console.WriteLine($"Total RAM: {totalMemory } MB");

            // Disk Space
            foreach (var drive in DriveInfo.GetDrives().Where(d => d.IsReady))
            {
                Console.WriteLine($"Drive {drive.Name} - Total Size: {drive.TotalSize / (1024 * 1024 * 1024)} GB, Free Space: {drive.TotalFreeSpace / (1024 * 1024 * 1024)} GB");
            }

            // Additional Information
            Console.WriteLine($"Framework Description: {GetFramework()}");
            Console.WriteLine($"Machine Name: {GetMachineName()}");
            Console.WriteLine($"System Uptime: {GetSystemUptime()}");
        }
        public string GetOSDescription()
        {
            return RuntimeInformation.OSDescription;
        }
        public string GetArchitecture()
        {
            return RuntimeInformation.OSArchitecture.ToString();
        }
        public int GetProcCount()
        {
            return Environment.ProcessorCount;
        }
        public int GetDriveCount()
        {
            return DriveInfo.GetDrives().Length;
        }
        public string GetFramework()
        {
            return RuntimeInformation.FrameworkDescription;
        }
        public string GetMachineName()
        {
            return Environment.MachineName;
        }
        public ulong GetTotalMemory()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                var query = "SELECT TotalVisibleMemorySize FROM Win32_OperatingSystem";
                using var searcher = new System.Management.ManagementObjectSearcher(query);
                foreach (var result in searcher.Get())
                {
                    var sizeKb = (ulong)result["TotalVisibleMemorySize"];
                    return (sizeKb * 1024)/ (1024 * 1024); // Convert from KB to Bytes
                }
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) || RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                try
                {
                    var info = File.ReadAllText("/proc/meminfo")
                        .Split('\n')
                        .FirstOrDefault(line => line.StartsWith("MemTotal"));
                    if (info != null)
                    {
                        var memKb = ulong.Parse(info.Split(':')[1].Trim().Split(' ')[0]);
                        return (memKb * 1024) / (1024 * 1024); // Convert from KB to Bytes
                    }
                }
                catch
                {
                    return 0; // Handle errors gracefully
                }
            }
            return 0;
        }
        public string GetSystemUptime()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return $"{Environment.TickCount64 / (1000 * 60 * 60)} hours";
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                try
                {
                    var uptimeInfo = File.ReadAllText("/proc/uptime");
                    var seconds = double.Parse(uptimeInfo.Split(' ')[0]);
                    return $"{seconds / 3600:F1} hours";
                }
                catch
                {
                    return "Unknown";
                }
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                var uptimeOutput = RunCommand("sysctl", "-n kern.boottime");
                if (!string.IsNullOrWhiteSpace(uptimeOutput))
                {
                    var bootTime = uptimeOutput.Split(' ')[3].TrimEnd(',');
                    var bootDateTime = DateTimeOffset.FromUnixTimeSeconds(long.Parse(bootTime)).DateTime;
                    var uptime = DateTime.Now - bootDateTime;
                    return $"{uptime.TotalHours:F1} hours";
                }
            }
            return "Unknown";
        }
        #endregion

        #region helpers
        public string RunCommand(string command, string arguments)
        {
            try
            {
                var processStartInfo = new ProcessStartInfo
                {
                    FileName = command,
                    Arguments = arguments,
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using (var process = new Process { StartInfo = processStartInfo })
                {
                    process.Start();
                    return process.StandardOutput.ReadToEnd();
                }
            }
            catch
            {
                return string.Empty;
            }
        }

        public async void CreateFolder(string targetDirectory)
        {
            if (!Directory.Exists(targetDirectory))
            {
                Directory.CreateDirectory(targetDirectory);
            }
        }
        #endregion
    }
}