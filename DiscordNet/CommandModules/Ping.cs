using System.Threading.Tasks;
using Discord.Commands;

namespace DiscordNet.CommandModules
{
    public class Ping : ModuleBase<SocketCommandContext>
    {
        [Command("ping")]
        [Summary("Check if the bot is responding.")]
        public async Task PingAsync()
        {
            await ReplyAsync("Pong!");
        }

        [Command("marco")]
        [Summary("Same as ping.")]
        public async Task Marco()
        {
            await ReplyAsync("Polo!");
        }

        [Command("mia")]
        [Summary("Same as ping.")]
        public async Task Mia()
        {
            await ReplyAsync("Maria!");
        }

        [Command("maria")]
        [Summary("Same as ping.")]
        public async Task Maria()
        {
            await ReplyAsync("Mia!");
        }
    }
}