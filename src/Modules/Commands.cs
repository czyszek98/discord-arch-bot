using Discord.Commands;

namespace DiscrodBotArch.src.Modules
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

            await ReplyAsync("Ich fange schon damit an!");

            _ = Task.Run(async () =>
            {
                try
                {
                    string fileName = "";
                    await YTDownloadClient.DownloadYouTubeVideo(url, async (title) =>
                    {
                        fileName = $"{title}.mp4";
                        await ReplyAsync($"Ich archiviere: {title}");
                    });
                    SftpUploader.Upload(fileName);
                    await ReplyAsync("Fertig! :D");
                }
                catch (Exception ex)
                {
                    await ReplyAsync("Da ist etwas schiefgelaufen :(");
                    await ReplyAsync(ex.Message);
                }
            });
        }
    }
}