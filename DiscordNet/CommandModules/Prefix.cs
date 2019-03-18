#region usings

using System.Threading.Tasks;
using Discord.Commands;

#endregion

namespace DiscordNet.CommandModules
{
    public class PrefixMain : ModuleBase<SocketCommandContext>
    {
        private string _prefix
        {
            get => BotUser.Default.Prefix;
            // ReSharper disable once UnusedMember.Local
            set
            {
                BotUser.Default.Prefix = value;
                BotUser.Default.Save();
            }
        }

        [Command("prefix")]
        public async Task Prefix()
        {
            await ReplyAsync($"Current: ``{_prefix}``");
        }
    }

    [Group("prefix")]
    public class PrefixSubs : ModuleBase<SocketCommandContext>
    {
        private string _prefix
        {
            get => BotUser.Default.Prefix;
            set
            {
                BotUser.Default.Prefix = value;
                BotUser.Default.Save();
            }
        }

        [Command("set")]
        public async Task SetPrefix(string newPrefix)
        {
            if (string.IsNullOrEmpty(newPrefix.Trim()))
            {
                await ReplyAsync($"Please input a prefix");
                return;
            }

            _prefix = newPrefix;
            await ReplyAsync($"New prefix: ``{_prefix}``");
        }
    }
}