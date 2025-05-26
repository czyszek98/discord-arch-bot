using FFMpegCore;
using System.Diagnostics;


namespace DiscrodBotArch.src
{
    internal class YTDownloadClient
    {
        public delegate Task Callback(string title);
        static public async Task DownloadYouTubeVideo(string videoUrl, Callback OnStart)
        {
            string outputDirectory = @"tmp/";
            string tmpDirectory = @"tmp/";
            var youtube = new YoutubeExplode.YoutubeClient();
            var video = await youtube.Videos.GetAsync(videoUrl);

            string sanitizedTitle = string.Join("_", video.Title.Split(Path.GetInvalidFileNameChars()));

            await OnStart(sanitizedTitle);

            Console.WriteLine($"Geting all available streams of: {videoUrl}");

            var streamManifest = await youtube.Videos.Streams.GetManifestAsync(video.Id);

            var muxedStreams = streamManifest.GetMuxedStreams().OrderByDescending(s => s.VideoQuality).ToList();

            var videoStream = streamManifest.GetVideoOnlyStreams().OrderByDescending(s => s.VideoQuality).ToList();
            var audioStream = streamManifest.GetAudioStreams().ToList();



            if (muxedStreams.Any())
            {
                await DownloadFile(muxedStreams.First(), outputDirectory, sanitizedTitle);
            }
            else if (videoStream.Any() && audioStream.Any())
            {
                try
                {
                    var videoTask = DownloadFile(videoStream.First(), tmpDirectory, $"{sanitizedTitle}_video");
                    var audioTask = DownloadFile(audioStream.First(), tmpDirectory, $"{sanitizedTitle}_audio");
                    await Task.WhenAll(videoTask, audioTask);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Błąd podczas pobierania plików: " + ex.Message);
                    throw new Exception($"Błąd podczas pobierania plików {sanitizedTitle}: {ex.Message}");
                }



                await MergeAudioAndVideo(tmpDirectory, outputDirectory, sanitizedTitle);
            }
            else
            {
                throw new Exception($"No suitable video stream found for {video.Title}.");
            }

        }

        private static async Task MergeAudioAndVideo(string tmpDirectory, string outputDirectory, string sanitizedTitle)
        {
            Console.WriteLine($"Merging files: {sanitizedTitle}_video.mp4 {sanitizedTitle}_audio.mp4");
            string videoPath = Path.Combine(tmpDirectory, $"{sanitizedTitle}_video.mp4");
            string audioPath = Path.Combine(tmpDirectory, $"{sanitizedTitle}_audio.mp4");
            string outputPath = Path.Combine(outputDirectory, $"{sanitizedTitle}.mp4");

            try
            {

                await CombineAudioAndVideoAsync(videoPath, audioPath, outputPath);

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Błąd mergowania plików {sanitizedTitle}: " + ex.Message);
                throw new Exception($"Błąd mergowania plików {sanitizedTitle}");
            }

            try
            {
                File.Delete(videoPath);
                File.Delete(audioPath);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Błąd podczas usuwania plików tymczasowych {sanitizedTitle} : " + ex.Message);
                throw new Exception($"Błąd podczas usuwania plików tymczasowych {sanitizedTitle}");
            }
        }


        private async static Task DownloadFile(YoutubeExplode.Videos.Streams.IStreamInfo streamInfo, string outputDirectory, string sanitizedTitle)
        {
            Console.WriteLine($"Downloading file: {sanitizedTitle}");
            var httpClient = new HttpClient();
            var stream = await httpClient.GetStreamAsync(streamInfo.Url);

            string outputFilePath = Path.Combine(outputDirectory, $"{sanitizedTitle}.{streamInfo.Container}");
            var outputStream = File.Create(outputFilePath);
            await stream.CopyToAsync(outputStream);
            stream.Dispose();
            outputStream.Dispose();
            Console.WriteLine($"File \"{sanitizedTitle}\" downloaded");
        }

        private static async Task CombineAudioAndVideoAsync(string videoPath, string audioPath, string outputPath)
        {

            var result = await FFMpegArguments
                .FromFileInput(videoPath)
                .AddFileInput(audioPath)
                .OutputToFile(outputPath, overwrite: true, options => options
                    .WithVideoCodec("copy")
                    .WithAudioCodec("aac")
                )
                .ProcessAsynchronously();
        }
    }
}