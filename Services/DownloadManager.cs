using System.IO.Compression;

namespace SlaveBackend.Services
{
    public class DownloadManager
    {

        private static readonly HttpClient httpClient = new HttpClient();
        public async Task DownloadSteamCmdAsync(string downloadFolder,bool downloadtype)
        {
            string steamCmdUrl = "https://steamcdn-a.akamaihd.net/client/installer/steamcmd.zip";
            string zipFile = Path.Combine(downloadFolder, "steamcmd.zip");

            try
            {
                Console.WriteLine("Starting SteamCmd Download");
                using (HttpResponseMessage response = await httpClient.GetAsync(steamCmdUrl, HttpCompletionOption.ResponseHeadersRead))
                {
                    response.EnsureSuccessStatusCode();
                    long? totalBytes = response.Content.Headers.ContentLength;

                    await using (var stream = await response.Content.ReadAsStreamAsync())
                    await using (var fileStream = new FileStream(zipFile, FileMode.Create, FileAccess.Write, FileShare.None, 8192, true))
                    {
                        byte[] buffer = new byte[8192];
                        long totalRead = 0;
                        int bytesRead;
                        while ((bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                        {
                            await fileStream.WriteAsync(buffer, 0, bytesRead);
                            totalRead += bytesRead;
                        }
                    }
                }

                try
                {
                    Console.WriteLine("Download Complete, Decompressing now.");
                    ZipFile.ExtractToDirectory(zipFile, downloadFolder, true);
                }
                catch (Exception ex)
                {
                    //return error
                    Console.WriteLine(ex.ToString());
                }
                finally
                {
                    Console.WriteLine("Decompressing complete, removing unwanted files.");
                    File.Delete(zipFile);
                    Console.WriteLine("Removed unwanted files");
                }
            }
            catch (Exception ex)
            {
                //return error
                Console.WriteLine($"{ex.Message}");
            }
        }
    }
}
