using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace SlaveBackend
{
    public class UserHelper
    {
        public void CreateUser(string username, string homeDirectory)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                CreateWindowsUser(username, homeDirectory);
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                CreateLinuxUser(username, homeDirectory);
            }
            else
            {
                throw new PlatformNotSupportedException("Unsupported OS platform.");
            }
        }

        private void CreateWindowsUser(string username, string homeDirectory)
        {
            try
            {
                // Create a user using the `net user` command
                RunCommand($"net user {username} /add /active:yes");

                // Create the home directory if it doesn't exist
                if (!Directory.Exists(homeDirectory))
                {
                    Directory.CreateDirectory(homeDirectory);
                }

                // Set permissions for the user on the home directory
                RunCommand($"icacls {homeDirectory} /grant {username}:(F)");
                Console.WriteLine($"User '{username}' created with home directory '{homeDirectory}' on Windows.");
            }
            catch (UnauthorizedAccessException ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                Console.WriteLine("Please run the application as Administrator.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An unexpected error occurred during user setup: {ex.Message}");
            }
        }

        private void CreateLinuxUser(string username, string homeDirectory)
        {
            try
            {
                // Create the user with the `useradd` command
                RunCommand($"sudo useradd -m -d {homeDirectory} {username}");

                // Add the user to the 'sudo' group
                RunCommand($"sudo usermod -aG sudo {username}");

                // Set permissions for the user on the home directory
                RunCommand($"sudo chmod 777 {homeDirectory}");
                RunCommand($"sudo chown {username}:{username} {homeDirectory}");
                Console.WriteLine($"User '{username}' created with home directory '{homeDirectory}' and added to 'sudo' group on Linux.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating Linux user: {ex.Message}");
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
    }

}