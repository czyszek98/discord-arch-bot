using Discord.Commands;

namespace DiscordArchBot.src.Modules
{
    public class Commands : ModuleBase<SocketCommandContext>
    {
        [Command("ping")]
        public async Task Ping()
        {
            await ReplyAsync("pong");
        }

        [Command("arch")]
        public async Task Arch(string url)
        {

            await ReplyAsync("Zaczynam!");

            _ = Task.Run(async () =>
            {
                string videoTitle = "";
                try
                {
                    string fileName = "";
                    await YTDownloadClient.DownloadYouTubeVideo(url, async (title) =>
                    {
                        fileName = $"{title}.mp4";
                        videoTitle = title;
                        await ReplyAsync($"Archiwizuje: {title}");
                    });
                    SftpUploader.Upload(fileName);
                    await ReplyAsync($"Pomyślnie zarchiwizowano film: {videoTitle}");
                }
                catch (Exception ex)
                {
                    await ReplyAsync($"Nie udało się zarchiwizować filmu: {videoTitle}");
                    await ReplyAsync(ex.Message);
                }
            });
        }
    }
}