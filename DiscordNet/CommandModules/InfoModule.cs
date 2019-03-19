#region usings

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;

#endregion

namespace DiscordNet.CommandModules
{
    public class InfoModule : ModuleBase<SocketCommandContext>
    {
        [Command("info")]
        [Summary("Get some information about this bot.")]
        public async Task Info()
        {
            IReadOnlyCollection<SocketGuild> guilds = Context.User.MutualGuilds;
            SocketGuildUser owner = null;
            TimeSpan timeActive = DateTime.Now - BotInfo.StartDateTime;

            foreach (SocketGuild socketGuild in guilds)
            {
                foreach (SocketGuildUser user in socketGuild.Users)
                    if (user.Id == BotUser.Default.OwnerID)
                    {
                        owner = user;
                        break;
                    }

                if (owner != null)
                    break;
            }

            await ReplyAsync("Bot info:\n" +
                             "```MARKUP\n" +
                             $"Version: {BotInfo.Version}\n" +
                             $"Instance ID: {BotInfo.InstanceId.Id()}{(BotInfo.BotIsPaused ? " (paused)" : "")}\n" +
                             $"Owner: {(owner == null ? "[NONE]#****" : $"{owner}")}\n" +
                             $"Start: {BotInfo.StartDateTime.ToLongDateString()} {BotInfo.StartDateTime.ToLongTimeString()}\n" +
                             $"Online for: {timeActive.Hours:00}:{timeActive.Minutes:00}:{timeActive.Seconds:00}.{timeActive.Milliseconds:000}\n" +
                             "```");
        }
    }
}