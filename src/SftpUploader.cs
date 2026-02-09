using AngleSharp.Text;
using Renci.SshNet;


namespace DiscordArchBot.src
{
    internal class SftpUploader
    {
        public delegate Task Callback(string title);
        static public void Upload(string fileName)
        {
            string host = Environment.GetEnvironmentVariable("SFTP_HOST") ?? "";
            int port = int.Parse(Environment.GetEnvironmentVariable("SFTP_PORT") ?? "22");
            string username = Environment.GetEnvironmentVariable("SFTP_USER") ?? "";
            string password = Environment.GetEnvironmentVariable("SFTP_PASSWORD") ?? "";

            // Ścieżki
            string localFilePath = $"tmp/{fileName}";
            string remoteFilePath = Environment.GetEnvironmentVariable("SFTP_ARCH_PATH") ?? "";

            using var sftp = new SftpClient(host, port, username, password);
            try
            {
                Console.WriteLine($"Łączenienie z  SFTP: {username}@{host} -p {port}");
                sftp.Connect();
                Console.WriteLine("Połączono z SFTP.");

                using var fileStream = new FileStream(localFilePath, FileMode.Open);
                sftp.UploadFile(fileStream, remoteFilePath);
                Console.WriteLine("Plik został wysłany.");

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Błąd połączenia SFTP {fileName}: " + ex.Message);
                throw new Exception($"Błąd podczas przesyłania pliku na serwer: {fileName}");
            }
            finally
            {
                if (sftp.IsConnected)
                    sftp.Disconnect();
                File.Delete(localFilePath);
            }

        }
    }
}