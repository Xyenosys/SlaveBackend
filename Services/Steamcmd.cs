using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;

namespace SlaveBackend.Services
{
    public class SteamCMDHelper
    {
        UserHelper _userHelper;
        OSHelper _oSHelper;
        string baseWindowsDir = @"C:\GSAdmin\";
        string baseLinuxDir = $"/home/GSAdmin";


        public SteamCMDHelper()
        {
            _userHelper = new UserHelper();
            _oSHelper = new OSHelper();
        }

        public void SetupSteamCMD()
        {
            if (_oSHelper.IsWindows())
            {
                try
                {
                    _userHelper.CreateUser("GSAdmin", baseWindowsDir);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                finally
                {
                    string targetDirectory = Path.Combine(baseWindowsDir, "SteamCmd");
                    _oSHelper.CreateFolder(targetDirectory);
                    SetupSteamCMDWindows(targetDirectory);
                }
            }
            else if (_oSHelper.IsLinux())
            {
                try
                {
                    _userHelper.CreateUser("GSAdmin", baseLinuxDir);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                finally
                {
                    string targetDirectory = Path.Combine(baseWindowsDir, "SteamCmd");
                    _oSHelper.CreateFolder(targetDirectory);
                    if (!File.Exists(targetDirectory))
                    SetupSteamCMDLinux(targetDirectory);
                }
            }
            else
            {
                throw new PlatformNotSupportedException("Unsupported OS platform.");
            }
        }

        private void SetupSteamCMDWindows(string targetDirectory)
        {
            try
            {
                string steamCmdUrl = "https://steamcdn-a.akamaihd.net/client/installer/steamcmd.zip";
                string steamCmdZipPath = Path.Combine(targetDirectory, "steamcmd.zip");
                string steamCmdExePath = Path.Combine(targetDirectory, "steamcmd.exe");

                if (File.Exists(steamCmdExePath))
                {
                    Console.WriteLine("SteamCMD is already installed in the target directory.");
                    return;
                }

                // Download SteamCMD ZIP file
                Console.WriteLine("Downloading SteamCMD...");
                using (var client = new WebClient())
                {
                    client.DownloadFile(steamCmdUrl, steamCmdZipPath);
                }

                // Extract the ZIP file
                Console.WriteLine("Extracting SteamCMD...");
                System.IO.Compression.ZipFile.ExtractToDirectory(steamCmdZipPath, targetDirectory);

                // Delete the ZIP file
                File.Delete(steamCmdZipPath);

                InitializeSteamCmd(steamCmdExePath);

                Console.WriteLine("SteamCMD setup completed successfully on Windows.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error setting up SteamCMD on Windows: {ex.Message}");
            }
        }

        private void SetupSteamCMDLinux(string targetDirectory)
        {

            try
            {
                
                string steamCmdUrl = "https://steamcdn-a.akamaihd.net/client/installer/steamcmd_linux.tar.gz";
                string steamCmdTarPath = Path.Combine(targetDirectory, "steamcmd_linux.tar.gz");

                string steamCmdExecPath = Path.Combine(targetDirectory, "steamcmd.sh");

                if (File.Exists(steamCmdExecPath))
                {
                    Console.WriteLine("SteamCMD is already installed in the target directory.");
                    return;
                }

                // Download SteamCMD tar.gz file
                Console.WriteLine("Downloading SteamCMD...");
                using (var client = new WebClient())
                {
                    client.DownloadFile(steamCmdUrl, steamCmdTarPath);
                }

                // Extract the tar.gz file
                Console.WriteLine("Extracting SteamCMD...");
                RunCommand($"tar -xvzf {steamCmdTarPath} -C {targetDirectory}");

                // Delete the tar.gz file
                File.Delete(steamCmdTarPath);

                InitializeSteamCmd(steamCmdExecPath);

                Console.WriteLine("SteamCMD setup completed successfully on Linux.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error setting up SteamCMD on Linux: {ex.Message}");
            }
        }

        private void RunCommand(string command)
        {
            var processInfo = new ProcessStartInfo
            {
                FileName = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "cmd.exe" : "/bin/bash",
                Arguments = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? $"/C {command}" : $"-c \"{command}\"",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using (var process = Process.Start(processInfo))
            {
                if (process == null)
                {
                    throw new Exception("Failed to start process.");
                }

                process.WaitForExit();
                var output = process.StandardOutput.ReadToEnd();
                var error = process.StandardError.ReadToEnd();

                if (process.ExitCode != 0)
                {
                    throw new Exception($"Command failed with exit code {process.ExitCode}: {error}");
                }

                Console.WriteLine(output);
            }
        }

        public void InitializeSteamCmd(string steamCmdPath)
        {
            Console.WriteLine("Initializing SteamCmd for the first time!");
            if (string.IsNullOrWhiteSpace(steamCmdPath))
            {
                throw new ArgumentException("SteamCMD path cannot be null or empty.");
            }

            // Determine the executable name based on the OS


            // Check if the SteamCMD file exists
            if (!System.IO.File.Exists(steamCmdPath))
            {
                throw new InvalidOperationException($"SteamCMD executable not found at: {steamCmdPath}");
            }

            try
            {
                // Create the process start info
                var processInfo = new ProcessStartInfo
                {
                    FileName = steamCmdPath,
                    Arguments = "+quit", // Quit after downloading required files
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                // Start the process
                using (var process = new Process { StartInfo = processInfo })
                {
                    process.Start();

                    // Capture output for logging purposes
                    string error = process.StandardError.ReadToEnd();

                    process.WaitForExit();

                
                    if (!string.IsNullOrWhiteSpace(error))
                    {
                        Console.WriteLine($"SteamCMD Error: {error}");
                    }

                    if (process.ExitCode != 0)
                    {
                        if (process.ExitCode == 7)
                        {
                        }
                        else
                        {
                            throw new InvalidOperationException($"SteamCMD exited with code {process.ExitCode}. Please check the logs above.");
                        }
                    }
                }

                Console.WriteLine("SteamCMD successfully initialized and all necessary files downloaded.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to initialize SteamCMD: {ex.Message}");
                throw;
            }
        }


    }

}