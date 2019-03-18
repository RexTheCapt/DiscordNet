using System.Threading.Tasks;
using Discord.Commands;

namespace DiscordNet.CommandModules
{
    public class GetServersModule : ModuleBase<SocketCommandContext>
    {
        [Command("get-servers")]
        public async Task GetServers()
        {

        }
    }
}