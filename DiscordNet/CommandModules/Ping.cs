using System.Threading.Tasks;
using Discord.Commands;

namespace DiscordNet.CommandModules
{
    public class Ping : ModuleBase<SocketCommandContext>
    {
        [Command("ping")]
        [Summary("Check if the bot is responding")]
        public async Task PingAsync()
        {
            await ReplyAsync("Pong!");
        }
    }
}