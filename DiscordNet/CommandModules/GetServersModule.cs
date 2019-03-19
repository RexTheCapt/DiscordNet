#region usings

using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;

#endregion

namespace DiscordNet.CommandModules
{
    public class GetServersModule : ModuleBase<SocketCommandContext>
    {
        [Command("get-servers")]
        [Summary("Get a list of all the servers the bot is connected to.")]
        public async Task GetServers()
        {
            string s = "";

            foreach (SocketGuild guild in Context.Client.Guilds) s += $"{guild.Name}, ";

            await ReplyAsync(s);
        }
    }
}