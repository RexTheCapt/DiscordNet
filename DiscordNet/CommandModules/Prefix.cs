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
        [Summary("Get the current prefix... you already know it...")]
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
        [Summary("Change the prefix (limited command)")]
        public async Task SetPrefix(string newPrefix)
        {
            if (Context.Message.Author.Id == BotUser.Default.OwnerID)
            {
                if (string.IsNullOrEmpty(newPrefix.Trim()))
                {
                    await ReplyAsync($"Please input a prefix");
                    return;
                }

                _prefix = newPrefix;
                await ReplyAsync($"New prefix: ``{_prefix}``");
                return;
            }

            await ReplyAsync($"Sorry {Context.Message.Author.Username}, your soul is too puny.");
        }
    }
}